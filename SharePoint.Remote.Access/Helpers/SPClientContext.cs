using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;

namespace SharePoint.Remote.Access.Helpers
{
    public sealed class SPClientContext : ClientContext
    {
        private SPClientContext(Uri webFullUrl)
            : base(webFullUrl)
        {
            ClientSite = SPClientSite.FromSite(Site);
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
    }
}