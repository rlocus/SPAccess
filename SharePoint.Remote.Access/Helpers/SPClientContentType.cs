using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using System;
using System.Linq;
using System.Linq.Expressions;
using SharePoint.Remote.Access.Extensions;

namespace SharePoint.Remote.Access.Helpers
{
    public sealed class SPClientContentType
    {
        private bool _executeQuery;
        public ContentType ContentType { get; private set; }

        internal SPClientContentType(ContentType contentType)
        {
            this.ContentType = contentType;
        }

        public bool IsLoaded { get; internal set; }

        public bool IsSiteContentType { get; internal set; }

        public SPClientWeb ClientWeb { get; internal set; }

        public SPClientList ClientList { get; internal set; }

        public string GetRestUrl()
        {
            return null;
        }

        public SPClientContentType IncludeFields(params Expression<Func<FieldCollection, object>>[] retrievals)
        {
            FieldCollection fields = this.ContentType.Fields;
            this.ContentType.Context.Load(fields, retrievals);
            _executeQuery = true;
            return this;
        }

        public SPClientField[] GetFields()
        {
            FieldCollection fields = this.ContentType.Fields;
            if (fields.AreItemsAvailable)
            {
                return fields.ToList().Select(field =>
                {
                    var clientField = SPClientField.FromField(field);
                    clientField.ClientWeb = this.ClientWeb;
                    clientField.ClientList = this.ClientList;
                    clientField.IsSiteField = this.IsSiteContentType;
                    return clientField;
                }).ToArray();
            }
            throw new SPAccessException("Field collection are not available.");
        }

        public void Load()
        {
            if (!IsLoaded)
            {
                this.ContentType.Context.Load(this.ContentType);
                _executeQuery = true;
            }

            if (_executeQuery)
            {
                this.ContentType.Context.ExecuteQuery();
                this.IsLoaded = true;
            }
            _executeQuery = false;
        }

        public async Task LoadAsync()
        {
            if (!IsLoaded)
            {
                this.ContentType.Context.Load(this.ContentType);
                _executeQuery = true;
            }

            if (_executeQuery)
            {
                await this.ContentType.Context.ExecuteQueryAsync();
                this.IsLoaded = true;
            }
            _executeQuery = false;
        }

        public void RefreshLoad()
        {
            if (this.IsLoaded)
            {
                this.IsLoaded = false;
                this.ContentType.RefreshLoad();
            }
        }

        internal static SPClientContentType FromContentType(ContentType contentType)
        {
            return new SPClientContentType(contentType);
        }
    }
}