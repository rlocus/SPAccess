using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using SharePoint.Remote.Access.Extensions;

namespace SharePoint.Remote.Access.Helpers
{
    public sealed class SPClientContext : ClientContext
    {
        private readonly object _lock = new object();
        private static readonly uint LoadsLimit = 1;

        private readonly ConcurrentDictionary<ClientObject, List<Expression<Func<ClientObject, object>>>> _retrievals;
        private SPClientContext(Uri webFullUrl)
            : base(webFullUrl)
        {
            ClientSite = SPClientSite.FromSite(Site);
            _retrievals = new ConcurrentDictionary<ClientObject, List<Expression<Func<ClientObject, object>>>>();
        }

        public SPClientContext(string webFullUrl, AuthType authType = AuthType.Default, string userName = null,
            string password = null)
            : this(Utility.RemoveTrailingSlash(new Uri(webFullUrl)), authType, userName, password)
        {
        }

        public SPClientContext(Uri webFullUrl, AuthType authType = AuthType.Default, string userName = null,
            string password = null)
            : this(webFullUrl)
        {
            Authentication = authType;
            UserName = userName;

            switch (Authentication)
            {
                case AuthType.Default:
                    AuthenticationMode = ClientAuthenticationMode.Default;
                    Credentials = string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(password)
                        ? CredentialCache.DefaultNetworkCredentials
                        : new NetworkCredential(UserName, password);
                    break;

                case AuthType.SharePointOnline:
                    AuthenticationMode = ClientAuthenticationMode.Default;
                    Credentials = new SharePointOnlineCredentials(UserName, Utility.GetSecureString(password));
                    break;

                case AuthType.Anonymous:
                    AuthenticationMode = ClientAuthenticationMode.Anonymous;
                    break;

                case AuthType.Forms:
                    AuthenticationMode = ClientAuthenticationMode.FormsAuthentication;
                    FormsAuthenticationLoginInfo = new FormsAuthenticationLoginInfo(UserName, password);
                    break;
            }
        }

        public SPClientSite ClientSite { get; }

        /// <summary>
        ///     Gets and sets the username for authentication with the site collection.
        /// </summary>
        public string UserName { get; private set; }

        /// <summary>
        ///     Authentication mode used for authentication with the SharePoint.
        /// </summary>
        public AuthType Authentication { get; private set; }

        public void Connect()
        {
            if (!ClientSite.IsLoaded)
            {
                ClientSite.Load();
            }
            else
            {
                ClientSite.RefreshLoad();
                ClientSite.Load();
            }
        }

        public async Task ConnectAsync()
        {
            if (!ClientSite.IsLoaded)
            {
                await ClientSite.LoadAsync();
            }
            else
            {
                ClientSite.RefreshLoad();
                await ClientSite.LoadAsync();
            }
        }

        public SPClientContext Clone()
        {
            return new SPClientContext(Url)
            {
                Authentication = Authentication,
                UserName = UserName,
                AuthenticationMode = AuthenticationMode,
                Credentials = Credentials,
                FormsAuthenticationLoginInfo = FormsAuthenticationLoginInfo
            };
        }

        public new void Load<T>(T clientObject, params Expression<Func<T, object>>[] retrievals) where T : ClientObject
        {
            lock (_lock)
            {
                var allRetrievals = _retrievals.ContainsKey(clientObject) ? _retrievals[clientObject] : new List<Expression<Func<ClientObject, object>>>();
                foreach (var retrieval in retrievals)
                {
                    allRetrievals.Add(retrieval as Expression<Func<ClientObject, object>>);
                }
                _retrievals[clientObject] = allRetrievals;
            }
        }

        public override void ExecuteQuery()
        {
            if (_retrievals.Count > 0)
            {
                lock (_lock)
                {
                    int pendingRequest = 0;
                    foreach (var retrieval in _retrievals)
                    {
                        List<Expression<Func<ClientObject, object>>> value;
                        if (_retrievals.TryRemove(retrieval.Key, out value))
                        {
                            base.Load(retrieval.Key, retrieval.Value.ToArray());
                            pendingRequest++;
                            if (pendingRequest > LoadsLimit)
                            {
                                base.ExecuteQuery();
                                pendingRequest = 0;
                            }
                        }
                    }
                    _retrievals.Clear();
                }
            }
            if (this.HasPendingRequest)
            {
                lock (_lock)
                {
                    base.ExecuteQuery();
                }
            }
        }

        public async Task ExecuteQueryAsync()
        {
            var tasks = new List<Task>();
            if (_retrievals.Count > 0)
            {
                lock (_lock)
                {
                    int pendingRequest = 0;
                    foreach (var retrieval in _retrievals)
                    {
                        List<Expression<Func<ClientObject, object>>> value;
                        if (_retrievals.TryRemove(retrieval.Key, out value))
                        {
                            base.Load(retrieval.Key, retrieval.Value.ToArray());
                            pendingRequest++;
                            if (pendingRequest > LoadsLimit)
                            {
                                tasks.Add(ClientRuntimeContextExtentions.ExecuteQueryAsync(this));
                                pendingRequest = 0;
                            }
                        }
                    }
                    if (pendingRequest > 0)
                    {
                        tasks.Add(ClientRuntimeContextExtentions.ExecuteQueryAsync(this));
                    }
                    _retrievals.Clear();
                }
            }

            if (tasks.Count > 0)
            {
                await Task.WhenAll(tasks);
            }

            if (this.HasPendingRequest)
            {
                await ClientRuntimeContextExtentions.ExecuteQueryAsync(this);
            }
        }
    }
}