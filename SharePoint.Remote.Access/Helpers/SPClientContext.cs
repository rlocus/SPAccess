using Microsoft.SharePoint.Client;
using SharePoint.Remote.Access.Extensions;
using System;
using System.Net;
using System.Threading.Tasks;
using ClientContext = Microsoft.SharePoint.Client.ClientContext;

namespace SharePoint.Remote.Access.Helpers
{
    public sealed class SPClientContext : ClientContext
    {
        private readonly SPClientSite _site;

        public SPClientSite ClientSite
        {
            get { return _site; }
        }

        /// <summary>
        /// Gets and sets the username for authentication with the site collection.
        /// </summary>
        public string UserName { get; private set; }

        /// <summary>
        /// Authentication mode used for authentication with the SharePoint.
        /// </summary>
        public AuthType Authentication { get; private set; }

        public SPClientContext(string webFullUrl, AuthType authType = AuthType.Default, string userName = null, string password = null)
            : this(Utility.RemoveTrailingSlash(new Uri(webFullUrl)), authType, userName, password)
        {
        }

        public SPClientContext(Uri webFullUrl, AuthType authType = AuthType.Default, string userName = null, string password = null)
            : base(webFullUrl)
        {
            this.Authentication = authType;
            this.UserName = userName;

            switch (this.Authentication)
            {
                case AuthType.Default:
                    this.AuthenticationMode = ClientAuthenticationMode.Default;
                    this.Credentials = string.IsNullOrEmpty(this.UserName) || string.IsNullOrEmpty(password)
                        ? CredentialCache.DefaultNetworkCredentials
                        : new NetworkCredential(this.UserName, password);
                    break;

                case AuthType.SharePointOnline:
                    this.AuthenticationMode = ClientAuthenticationMode.Default;
                    this.Credentials = new SharePointOnlineCredentials(this.UserName, Utility.GetSecureString(password));
                    break;

                case AuthType.Anonymous:
                    this.AuthenticationMode = ClientAuthenticationMode.Anonymous;
                    break;

                case AuthType.Forms:
                    this.AuthenticationMode = ClientAuthenticationMode.FormsAuthentication;
                    this.FormsAuthenticationLoginInfo = new FormsAuthenticationLoginInfo(this.UserName, password);
                    break;
            }

            // Try connection, to ensure site is available
            _site = SPClientSite.FromSite(this.Site);
        }

        public void Connect()
        {
            if (!_site.IsLoaded)
            {
                this.Load(_site);
                this.ExecuteQuery();
                _site.IsLoaded = true;
            }
            else
            {
                _site.IsLoaded = false;
                _site.RefreshLoad();
                this.ExecuteQuery();
                _site.IsLoaded = true;
            }
        }

        public async Task ConnectAsync()
        {
            if (!_site.IsLoaded)
            {
                this.Load(_site);
                await this.ExecuteQueryAsync();
                _site.IsLoaded = true;
            }
            else
            {
                _site.IsLoaded = false;
                _site.RefreshLoad();
                await this.ExecuteQueryAsync();
                _site.IsLoaded = true;
            }
        }
    }
}