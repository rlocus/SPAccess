using Microsoft.SharePoint.Client;
using SharePoint.Remote.Access.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using List = Microsoft.SharePoint.Client.List;

namespace SharePoint.Remote.Access.Helpers
{
    public sealed class SPClientList : List, IClientObject
    {
        internal SPClientList(SPClientContext context, ObjectPath objectPath)
            : base(context, objectPath)
        {
        }

        public bool IsLoaded { get; internal set; }

        public SPClientWeb ClientWeb { get; internal set; }

        private IEnumerable<ContentType> LoadContentTypes(ContentTypeCollection contentTypes)
        {
            var cts = new List<ContentType>();
            if (contentTypes != null && contentTypes.Count > 0)
            {
                foreach (ContentType contentType in contentTypes)
                {
                    this.Context.Load(contentType);
                    cts.Add(contentType);
                }

                this.Context.ExecuteQuery();
            }
            return cts;
        }

        private async Task<List<ContentType>> LoadContentTypesAsync(ContentTypeCollection contentTypes)
        {
            var cts = new List<ContentType>();
            if (contentTypes != null && contentTypes.Count > 0)
            {
                foreach (ContentType contentType in contentTypes)
                {
                    this.Context.Load(contentType);
                    cts.Add(contentType);
                }

                await this.Context.ExecuteQueryAsync();
            }
            return cts;
        }

        private SPClientField LoadField(Field field, bool executeQuery = false)
        {
            SPClientField clientField = SPClientField.FromField(field);
            this.Context.Load(clientField);
            clientField.ClientWeb = this.ClientWeb;
            clientField.ClientList = this;
            if (executeQuery)
            {
                this.Context.ExecuteQuery();
                clientField.IsLoaded = true;
            }
            return clientField;
        }

        private IEnumerable<SPClientField> LoadFields(FieldCollection fields)
        {
            var clientFields = new List<SPClientField>();
            if (fields != null && fields.Count > 0)
            {
                foreach (Field field in fields)
                {
                    SPClientField clientField = LoadField(field, false);
                    clientFields.Add(clientField);
                }

                this.Context.ExecuteQuery();
            }
            return clientFields;
        }

        private async Task<List<SPClientField>> LoadFieldsAsync(FieldCollection fields)
        {
            var clientFields = new List<SPClientField>();
            if (fields != null && fields.Count > 0)
            {
                foreach (Field field in fields)
                {
                    SPClientField clientField = LoadField(field, false);
                    clientFields.Add(clientField);
                }

                await this.Context.ExecuteQueryAsync();
            }
            return clientFields;
        }

        public ContentTypeCollection GetContentTypeCollection()
        {
            ContentTypeCollection contentTypes = this.ContentTypes;

            if (!contentTypes.AreItemsAvailable)
            {
                this.Context.Load(contentTypes);
                //this.Context.ExecuteQuery();
            }
            return contentTypes;
        }

        public async Task<ContentTypeCollection> GetContentTypeCollectionAsync()
        {
            ContentTypeCollection contentTypes = this.ContentTypes;

            if (!contentTypes.AreItemsAvailable)
            {
                this.Context.Load(contentTypes);
                await this.Context.ExecuteQueryAsync();
            }
            return contentTypes;
        }

        public IEnumerable<ContentType> LoadContentTypes(out int count)
        {
            ContentTypeCollection contentTypes = this.GetContentTypeCollection();
            this.Context.ExecuteQuery();
            count = contentTypes.Count;
            return LoadContentTypes(contentTypes).Where(ct => ct != null);
        }

        public async Task<IEnumerable<ContentType>> LoadContentTypesAsync()
        {
            ContentTypeCollection contentTypes = await this.GetContentTypeCollectionAsync();
            //int count = lists.Count;
            List<ContentType> cts = await LoadContentTypesAsync(contentTypes);
            return cts.Where(clientList => clientList != null);
        }

        public async Task<FieldCollection> GetFieldCollectionAsync()
        {
            FieldCollection fields = this.Fields;

            if (!fields.AreItemsAvailable)
            {
                this.Context.Load(fields);
                await this.Context.ExecuteQueryAsync();
            }
            return fields;
        }

        public IEnumerable<SPClientField> LoadFields()
        {
            int count;
            return LoadFields(out count);
        }

        public IEnumerable<SPClientField> LoadFields(out int count)
        {
            FieldCollection fields = this.GetFieldCollection();
            this.Context.ExecuteQuery();
            count = fields.Count;
            return LoadFields(fields).Where(clientField => clientField != null);
        }

        public async Task<IEnumerable<SPClientField>> LoadFieldsAsync()
        {
            FieldCollection fields = await this.GetFieldCollectionAsync();
            List<SPClientField> clientFields = await LoadFieldsAsync(fields);
            return clientFields.Where(clientField => clientField != null);
        }

        public string GetListUrl()
        {
            return null; //this.RootFolder.GetFolderUrl();
        }

        public string GetSettingsUrl()
        {
            return string.Format("{0}/_layouts/{1}/listedit.aspx?List={2}", this.ClientWeb.GetUrl().TrimEnd('/'), this.ClientWeb.UIVersion, this.Id);
        }

        public string GetRestUrl()
        {
            return string.Format("{0}/_api/web/lists(guid'{1}')", this.ClientWeb.GetUrl().TrimEnd('/'), this.Id);
        }

        public string GetUrl()
        {
            return Utility.CombineUrls(new Uri(this.Context.Url.ToLower()), this.ParentWebUrl.ToLower());
        }

        public FieldCollection GetFieldCollection()
        {
            FieldCollection fields = this.Fields;

            if (!fields.AreItemsAvailable)
            {
                this.Context.Load(fields);
                //this.Context.ExecuteQuery();
            }
            return fields;
        }

        internal static SPClientList FromList(List list)
        {
            return new SPClientList(list.Context as SPClientContext, list.Path);
        }

        public override void RefreshLoad()
        {
            if (!this.IsLoaded)
            {
                base.RefreshLoad();
            }
        }
    }
}