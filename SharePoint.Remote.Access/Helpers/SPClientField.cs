using Microsoft.SharePoint.Client;
using Field = Microsoft.SharePoint.Client.Field;

namespace SharePoint.Remote.Access.Helpers
{
    public sealed class SPClientField : Field, IClientObject
    {
        internal SPClientField(SPClientContext context, ObjectPath objectPath)
            : base(context, objectPath)
        {
        }

        public bool IsLoaded { get; internal set; }

        public bool IsSiteField { get; internal set; }

        public SPClientWeb ClientWeb { get; internal set; }

        public SPClientList ClientList { get; internal set; }

        public string GetRestUrl()
        {
            return null;
        }

        public override void RefreshLoad()
        {
            if (!this.IsLoaded)
            {
                base.RefreshLoad();
            }
        }

        internal static SPClientField FromField(Field field)
        {
            return new SPClientField(field.Context as SPClientContext, field.Path);
        }
    }
}