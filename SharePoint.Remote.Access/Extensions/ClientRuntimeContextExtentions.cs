using Microsoft.SharePoint.Client;
using System;
using System.Threading.Tasks;

namespace SharePoint.Remote.Access.Extensions
{
    public static class ClientRuntimeContextExtentions
    {
        /// <summary>
        /// Returns true when connected site is at least the provided SharePoint version.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public static bool IsMinimalServerVersion(this ClientRuntimeContext context, ServerVersion version)
        {
            if (context.ServerSchemaVersion.Major < (int)version)
                return false;
            else
                return true;
        }

        public static Task ExecuteQueryAsync(this ClientRuntimeContext clientContext)
        {
            return Task.Run(new Action(clientContext.ExecuteQuery));
        }

        public static async void ExecuteQueryAsync(this ClientRuntimeContext clientContext, Action onSucceed, Action<Exception> onFailed)
        {
            try
            {
                await ExecuteQueryAsync(clientContext);
                if (onSucceed != null) onSucceed();
            }
            catch (Exception ex)
            {
                if (onFailed != null)
                {
                    onFailed(ex);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}