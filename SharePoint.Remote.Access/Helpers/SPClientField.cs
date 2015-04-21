using Field = Microsoft.SharePoint.Client.Field;

namespace SharePoint.Remote.Access.Helpers
{
    public sealed class SPClientField
    {
        public Field Field { get; private set; }

        internal SPClientField(Field field)
        {
            this.Field = field;
        }

        public bool IsLoaded { get; internal set; }

        public bool IsSiteField { get; internal set; }

        public SPClientWeb ClientWeb { get; internal set; }

        public SPClientList ClientList { get; internal set; }

        public string GetRestUrl()
        {
            return null;
        }

        public void RefreshLoad()
        {
            if (!this.IsLoaded)
            {
                this.Field.RefreshLoad();
            }
        }

        internal static SPClientField FromField(Field field)
        {
            return new SPClientField(field);
        }
    }
}