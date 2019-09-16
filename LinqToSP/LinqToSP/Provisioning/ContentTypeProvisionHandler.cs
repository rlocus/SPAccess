using Microsoft.SharePoint.Client;
using SP.Client.Linq.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SP.Client.Linq.Provisioning
{
    public class ContentTypeProvisionHandler<TContext, TEntity> : ProvisionHandler<TContext, TEntity>
        where TContext : class, ISpEntryDataContext
        where TEntity : class, IListItemEntity
    {
        private readonly ContentTypeAttribute _contentType;
        private readonly ListAttribute _list;

        public event Action<ContentTypeCreationInformation> OnProvisioning;

        public event Action<ContentType> OnProvisioned;

        public ContentTypeProvisionHandler(ContentTypeAttribute contentType, ProvisionModel<TContext, TEntity> model) : this(contentType, null, model)
        {
            _contentType = contentType;
        }

        public ContentTypeProvisionHandler(ContentTypeAttribute contentType, ListAttribute list, ProvisionModel<TContext, TEntity> model) : base(model)
        {
            _contentType = contentType;
            _list = list;
        }

        public override void Provision()
        {
            if (_contentType != null && Model != null && Model.Context != null && Model.Context.Context != null)
            {
                var context = Model.Context.Context;
                Web web = context.Web;
                List list = null;
                ContentType parentContentType = null;
                ContentType contentType = null;

                if (!string.IsNullOrEmpty(_contentType.ParentId))
                {
                    parentContentType = web.AvailableContentTypes.GetById(_contentType.ParentId);
                }

                if (_list != null)
                {
                    list = _list.Title != null
                        ? context.Web.Lists.GetByTitle(_list.Title)
                        : (_list.Url != null ? context.Web.GetList($"{ Model.Context.SiteUrl.TrimEnd('/')}/{_list.Url.TrimStart('/')}") : null);
                }

                var newContentType = new ContentTypeCreationInformation()
                {
                    Id = _contentType.Id,
                    Name = _contentType.Name,
                    Group = _contentType.Group,
                    ParentContentType = parentContentType
                };

                IEnumerable<ContentType> webContentTypes = context.LoadQuery(web.AvailableContentTypes.Where(ct => ct.Name == _contentType.Name));
                IEnumerable<ContentType> listContentTypes = null;
                if (list != null)
                {
                    listContentTypes = context.LoadQuery(list.ContentTypes.Where(ct => ct.Name == _contentType.Name));
                }

                context.ExecuteQuery();
                ContentType webContentType = webContentTypes.FirstOrDefault();

                if (listContentTypes != null)
                {
                    ContentType listContentType = listContentTypes.FirstOrDefault();
                    if (listContentType != null)
                    {
                        OnProvisioned?.Invoke(listContentType);
                        return;
                    }
                }

                if (webContentType != null)
                {
                    contentType = list.ContentTypes.AddExistingContentType(webContentType);
                }
                else
                {
                    OnProvisioning?.Invoke(newContentType);
                    contentType = list.ContentTypes.Add(newContentType);
                }

                context.Load(contentType);
                context.ExecuteQuery();
                OnProvisioned?.Invoke(contentType);
            }
        }
    }
}

