using System;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using SharePoint.Remote.Access.Extensions;

namespace SharePoint.Remote.Access.Helpers
{
    public sealed class SPClientField
    {
        private bool _executeQuery;

        internal SPClientField(Field field)
        {
            if (field == null) throw new ArgumentNullException(nameof(field));
            Field = field;
        }

        public Field Field { get; }
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
            if (IsLoaded)
            {
                IsLoaded = false;
                Field.RefreshLoad();
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
                (Field.Context as SPClientContext).Load(Field);
                _executeQuery = true;
            }

            if (_executeQuery)
            {
                Field.Context.ExecuteQuery();
                IsLoaded = true;
            }
            _executeQuery = false;
        }

        public async Task LoadAsync()
        {
            if (!IsLoaded)
            {
                await (Field.Context as SPClientContext).LoadAsync(Field);
                _executeQuery = true;
            }

            if (_executeQuery)
            {
                await (Field.Context as SPClientContext).ExecuteQueryAsync();
                IsLoaded = true;
            }
            _executeQuery = false;
        }
    }
}