using System.Threading.Tasks;
using SharePoint.Remote.Access.Extensions;
using Field = Microsoft.SharePoint.Client.Field;

namespace SharePoint.Remote.Access.Helpers
{
    public sealed class SPClientField
    {
        private bool _executeQuery;

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
            if (this.IsLoaded)
            {
                this.IsLoaded = false;
                this.Field.RefreshLoad();
            }
        }

        internal static SPClientField FromField(Field field)
        {
            return new SPClientField(field);
        }

        public void Load()
        {
            if (!IsLoaded)
            {
                this.Field.Context.Load(this.Field);
                _executeQuery = true;
            }

            if (_executeQuery)
            {
                this.Field.Context.ExecuteQuery();
                this.IsLoaded = true;
            }
            _executeQuery = false;
        }

        public async Task LoadAsync()
        {
            if (!IsLoaded)
            {
                //this.Web.RefreshLoad();
                this.Field.Context.Load(this.Field);
                _executeQuery = true;
            }

            if (_executeQuery)
            {
                await this.Field.Context.ExecuteQueryAsync();
                this.IsLoaded = true;
            }
            _executeQuery = false;
        }
    }
}