using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using SharePoint.Remote.Access.Extensions;

namespace SharePoint.Remote.Access.Helpers
{
    public sealed class SPClientContext : ClientContext
    {
        private readonly ConcurrentDictionary<ClientObject, List<Action>> _retrievals;

        private SPClientContext(Uri webFullUrl)
            : base(webFullUrl)
        {
            ClientSite = SPClientSite.FromSite(Site);
            _retrievals = new ConcurrentDictionary<ClientObject, List<Action>>();
            RetryCount = 5;
            Delay = 500;
            MaxResourcesPerRequest = 30;
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
        public string UserName { get; }

        /// <summary>
        ///     Authentication mode used for authentication with the SharePoint.
        /// </summary>
        public AuthType Authentication { get; }

        //public uint BatchSize { get; set; }

        public uint MaxResourcesPerRequest { get; set; }

        public int RetryCount { get; set; }

        public int Delay { get; set; }

        private async Task BaseExecuteQueryAsync()
        {
            await Task.Run(() =>
            {
                //    if (this.HasPendingRequest)
                //    {
                //        base.ExecuteQuery();
                //    }
                BaseExecuteQueryRetry();
            });
        }

        private void BaseExecuteQueryRetry()
        {
            var clientTag = string.Empty;

            int retryAttempts = 0;
            if (RetryCount <= 0)
                RetryCount = 1;

            if (Delay <= 0)
                Delay = 1;

            // Do while retry attempt is less than retry count
            while (retryAttempts < RetryCount)
            {
                try
                {
                    // ClientTag property is limited to 32 chars
                    if (clientTag.Length > 32)
                    {
                        clientTag = clientTag.Substring(0, 32);
                    }
                    this.ClientTag = clientTag;

                    if (HasPendingRequest)
                    {
                        base.ExecuteQuery();
                    }
                    return;
                }
                catch (ServerException ex)
                {
                    //too many resources
                    if (ex.ServerErrorCode == -2146232832)
                    {
                        //TODO:
                    }
                    throw;
                }
                catch (WebException wex)
                {
                    var response = wex.Response as HttpWebResponse;
                    // Check if request was throttled - http status code 429
                    // Check is request failed due to server unavailable - http status code 503
                    if (response != null && (response.StatusCode == (HttpStatusCode)429 ||
                                             response.StatusCode == (HttpStatusCode)503))
                    {
                        //Add delay for retry
                        Thread.Sleep(Delay);

                        //Add to retry count and increase delay.
                        retryAttempts++;
                        Delay = Delay * 2;
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            throw new MaximumRetryAttemptedException($"Maximum retry attempts {RetryCount}, has be attempted.");
        }

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

        public new void Load<T>(T clientObject, params Expression<Func<T, object>>[] retrievals) where T : ClientObject
        {
            var actions = _retrievals.ContainsKey(clientObject) ? _retrievals[clientObject] : new List<Action>();
            actions.Add(() =>
            {
                base.Load(clientObject, retrievals);
            });
            _retrievals[clientObject] = actions;
            if (_retrievals.Count >= MaxResourcesPerRequest)
            {
                this.ExecuteQuery();
            }
        }

        public async Task LoadAsync<T>(T clientObject, params Expression<Func<T, object>>[] retrievals) where T : ClientObject
        {
            var actions = _retrievals.ContainsKey(clientObject) ? _retrievals[clientObject] : new List<Action>();
            actions.Add(() =>
            {
                base.Load(clientObject, retrievals);
            });
            _retrievals[clientObject] = actions;
            if (_retrievals.Count >= MaxResourcesPerRequest)
            {
                await this.ExecuteQueryAsync();
            }
        }

        public override void ExecuteQuery()
        {
            if (_retrievals.Count > 0)
            {
                int pendingRequest = 0;
                foreach (var retrieval in _retrievals)
                {
                    List<Action> actions;
                    if (_retrievals.TryRemove(retrieval.Key, out actions))
                    {
                        foreach (var action in actions)
                        {
                            action.Invoke();
                            pendingRequest++;
                            if (pendingRequest >= MaxResourcesPerRequest)
                            {
                                this.ExecuteQuery();
                                return;
                            }
                        }
                    }
                }
            }

            //if (this.HasPendingRequest)
            //{
            //    base.ExecuteQuery();
            //}
            BaseExecuteQueryRetry();
        }

        public async Task ExecuteQueryAsync()
        {
            if (_retrievals.Count > 0)
            {
                var tasks = new List<Task>();
                uint resourcesPerRequest = 0;
                foreach (var retrieval in _retrievals)
                {
                    List<Action> actions;
                    if (_retrievals.TryRemove(retrieval.Key, out actions))
                    {
                        foreach (var action in actions)
                        {
                            action.Invoke();
                            resourcesPerRequest++;
                            if (resourcesPerRequest >= MaxResourcesPerRequest)
                            {
                                tasks.Add(BaseExecuteQueryAsync());
                                resourcesPerRequest = 0;
                            }
                        }
                    }
                }
                if (resourcesPerRequest > 0)
                {
                    tasks.Add(BaseExecuteQueryAsync());
                }
                _retrievals.Clear();
                if (tasks.Count > 0)
                {
                    await Task.WhenAll(tasks);
                }
            }
            else
            {
                await BaseExecuteQueryAsync();
            }
        }

        public SPClientContext Clone(Uri siteUrl = null)
        {
            SPClientContext clonedClientContext = new SPClientContext(siteUrl ?? new Uri(this.Url))
            {
                AuthenticationMode = this.AuthenticationMode,
                ClientTag = this.ClientTag,
            };

            // In case of using networkcredentials in on premises or SharePointOnlineCredentials in Office 365
            if (this.Credentials != null)
            {
                clonedClientContext.Credentials = this.Credentials;
            }
            else
            {
                //Take over the form digest handling setting
                clonedClientContext.FormDigestHandlingEnabled = this.FormDigestHandlingEnabled;

                // In case of app only or SAML
                clonedClientContext.ExecutingWebRequest += delegate (object oSender, WebRequestEventArgs webRequestEventArgs)
                {
                    // Call the ExecutingWebRequest delegate method from the original ClientContext object, but pass along the webRequestEventArgs of 
                    // the new delegate method
                    MethodInfo methodInfo = this.GetType().GetMethod("OnExecutingWebRequest", BindingFlags.Instance | BindingFlags.NonPublic);
                    object[] parametersArray = { webRequestEventArgs };
                    methodInfo.Invoke(this, parametersArray);
                };
            }

            return clonedClientContext;
        }
    }
}