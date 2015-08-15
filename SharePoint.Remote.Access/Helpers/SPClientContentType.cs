using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using SharePoint.Remote.Access.Extensions;

namespace SharePoint.Remote.Access.Helpers
{
    public sealed class SPClientContentType
    {
        private bool _executeQuery;

        internal SPClientContentType(ContentType contentType)
        {
            ContentType = contentType;
        }

        public ContentType ContentType { get; }
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
            var fields = ContentType.Fields;
            ContentType.Context.Load(fields, retrievals);
            _executeQuery = true;
            return this;
        }

        public SPClientField[] GetFields()
        {
            var fields = ContentType.Fields;
            if (fields != null && fields.AreItemsAvailable)
            {
                return fields.ToList().Select(field =>
                {
                    var clientField = SPClientField.FromField(field);
                    clientField.ClientWeb = ClientWeb;
                    clientField.ClientList = ClientList;
                    clientField.IsSiteField = IsSiteContentType;
                    return clientField;
                }).ToArray();
            }
            throw new SPAccessException("Field collection is not available.");
        }

        public void Load()
        {
            if (!IsLoaded)
            {
                ContentType.Context.Load(ContentType);
                _executeQuery = true;
            }

            if (_executeQuery)
            {
                ContentType.Context.ExecuteQuery();
                IsLoaded = true;
            }
            _executeQuery = false;
        }

        public async Task LoadAsync()
        {
            if (!IsLoaded)
            {
                ContentType.Context.Load(ContentType);
                _executeQuery = true;
            }

            if (_executeQuery)
            {
                await ContentType.Context.ExecuteQueryAsync();
                IsLoaded = true;
            }
            _executeQuery = false;
        }

        public void RefreshLoad()
        {
            if (IsLoaded)
            {
                IsLoaded = false;
                ContentType.RefreshLoad();
            }
        }

        internal static SPClientContentType FromContentType(ContentType contentType)
        {
            return new SPClientContentType(contentType);
        }
    }
}