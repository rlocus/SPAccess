using System;
using System.Security;
using System.Threading.Tasks;

namespace SharePoint.Remote.Access.Helpers
{
    public static class Utility
    {
        /// <summary>
        /// Adds a relative URL to this URL.
        /// </summary>
        /// <param name="baseUri">Extended URI.</param>
        /// <param name="relativeUrl">Relative URL to add to current URI.</param>
        /// <returns>Returns full URL.</returns>
        public static string CombineUrls(Uri baseUri, string relativeUrl)
        {
            if (baseUri == null) throw new ArgumentNullException("baseUri");
            string siteCollectionUrl = string.Format("{0}://{1}{2}/",
                baseUri.Scheme,
                baseUri.DnsSafeHost,
                baseUri.LocalPath.Equals("/") ? string.Empty : baseUri.LocalPath);

            string webUrl = relativeUrl.TrimEnd('/').Replace(new Uri(siteCollectionUrl).LocalPath.TrimEnd('/'), "");
            string url = string.Format("{0}/{1}", siteCollectionUrl.TrimEnd('/'), webUrl.TrimStart('/'));
            return url;
        }

        /// <summary>
        /// Removes the trailing forward-slash at the end.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static Uri RemoveTrailingSlash(Uri url)
        {
            if (url == null) throw new ArgumentNullException("url");
            // Remove trailing forward-slash
            if (url.OriginalString.EndsWith("/"))
                url = new Uri(url.OriginalString.Remove(url.OriginalString.Length - 1));

            return url;
        }

        /// <summary>
        /// Returns the <see cref="SecureString"/> for use of passwords.
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static SecureString GetSecureString(string password)
        {
            if (password == null) throw new ArgumentNullException("password");
            var securePassWord = new SecureString();
            foreach (char c in password) securePassWord.AppendChar(c);
            return securePassWord;
        }

        public static IPromise<object, Exception> ExecuteAsync(Task asyncTask)
        {
            var deferred = new Deferred<object, Exception>();
            new Action(async () =>
            {
                try
                {
                    await asyncTask;
                    deferred.Resolve();
                }
                catch (Exception ex)
                {
                    deferred.Reject(ex);
                }
            }).Invoke();
            return deferred.Promise();
        }

        public static IPromise<T, Exception> ExecuteAsync<T>(Task<T> asyncTask)
        {
            var deferred = new Deferred<T, Exception>();
            new Action(async () =>
            {
                try
                {
                    T result = await asyncTask;
                    deferred.Resolve(result);
                }
                catch (Exception ex)
                {
                    deferred.Reject(ex);
                }
            }).Invoke();
            return deferred.Promise();
        }
    }
}