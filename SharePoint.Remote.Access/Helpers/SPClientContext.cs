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
        private int _resourcesPerRequest;

        private SPClientContext(Uri webFullUrl)
            : base(webFullUrl)
        {
            ClientSite = SPClientSite.FromSite(Site);
            //RetryCount = 5;
            //Delay = 500;
            MaxResourcesPerRequest = 30;
            _resourcesPerRequest = 0;
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

        public uint MaxResourcesPerRequest { get; set; }

        //public int RetryCount { get; set; }

        //public int Delay { get; set; }
        
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
            _resourcesPerRequest++;
            base.Load(clientObject, retrievals);
            if (MaxResourcesPerRequest > 0 && _resourcesPerRequest >= MaxResourcesPerRequest)
            {
                _resourcesPerRequest = 0;
                this.ExecuteQuery();
            }
        }

        public async Task LoadAsync<T>(T clientObject, params Expression<Func<T, object>>[] retrievals) where T : ClientObject
        {
            _resourcesPerRequest++;
            base.Load(clientObject, retrievals);

            if (MaxResourcesPerRequest > 0 && _resourcesPerRequest >= MaxResourcesPerRequest)
            {
                _resourcesPerRequest = 0;
                await this.ExecuteQueryAsync();
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