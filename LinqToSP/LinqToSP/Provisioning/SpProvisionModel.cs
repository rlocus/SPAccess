using Microsoft.SharePoint.Client;
using SP.Client.Linq.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SP.Client.Linq.Provisioning
{
    public class SpProvisionModel<TContext, TEntity>
        where TContext : class, ISpEntryDataContext
        where TEntity : class, IListItemEntity
    {
        public TContext Context { get; }

        private List<SpProvisionHandler<TContext, TEntity>> ProvisionHandlers { get; set; }

        public SpProvisionModel(TContext context)
        {
            Context = context;
            RetrieveHandlers();
        }

        private void RetrieveHandlers()
        {
            ProvisionHandlers = new List<SpProvisionHandler<TContext, TEntity>>();
            var contentTypes = AttributeHelper.GetCustomAttributes<TEntity, ContentTypeAttribute>(true);
            var list = AttributeHelper.GetCustomAttributes<TEntity, ListAttribute>(false).FirstOrDefault();
            var fields = AttributeHelper.GetFieldAttributes<TEntity, FieldAttribute>().Concat(AttributeHelper.GetPropertyAttributes<TEntity, FieldAttribute>());

            if (list != null)
            {
                var listHandler = new ListProvisionHandler<TContext, TEntity>(list, this);
                listHandler.OnProvisioning += ListHandler_OnProvisioning;
                listHandler.OnProvisioned += ListHandler_OnProvisioned;
                ProvisionHandlers.Add(listHandler);
            }
            foreach (var contentType in contentTypes)
            {
                if (contentType != null)
                {
                    var contentTypeHandler = new ContentTypeProvisionHandler<TContext, TEntity>(contentType, list, this);
                    contentTypeHandler.OnProvisioned += ContentTypeHandler_OnProvisioned;
                    contentTypeHandler.OnProvisioning += ContentTypeHandler_OnProvisioning;
                    ProvisionHandlers.Add(contentTypeHandler);
                }
            }

            foreach (var field in fields)
            {
                if (typeof(DependentLookupFieldAttribute).IsAssignableFrom(field.Value.GetType()))
                {
                    continue;
                }

                if (typeof(LookupFieldAttribute).IsAssignableFrom(field.Value.GetType()))
                {
                }

                Type valueType = null;
                if (field.Key is PropertyInfo)
                {
                    valueType = (field.Key as PropertyInfo).PropertyType;
                }
                else if (field.Key is FieldInfo)
                {
                    valueType = (field.Key as FieldInfo).FieldType;
                }
                var fieldHandler = new FieldProvisionHandler<TContext, TEntity>(field.Value,
                    AttributeHelper.GetCustomAttributes<TEntity, ContentTypeAttribute>(false).FirstOrDefault(), list, this, valueType);
                fieldHandler.OnProvisioned += FieldHandler_OnProvisioned;
                fieldHandler.OnProvisioning += FieldHandler_OnProvisioning;



                ProvisionHandlers.Add(fieldHandler);
            }
        }

        protected virtual void FieldHandler_OnProvisioning(FieldProvisionHandler<TContext, TEntity> handler, Field field)
        {
        }

        protected virtual void FieldHandler_OnProvisioned(FieldProvisionHandler<TContext, TEntity> handler, Field field)
        {
        }

        protected virtual void ListHandler_OnProvisioning(ListProvisionHandler<TContext, TEntity> handler, List list)
        {
            if (ProvisionHandlers != null && ProvisionHandlers.Any(h => typeof(ContentTypeProvisionHandler<TContext, TEntity>).IsAssignableFrom(h.GetType())))
            {
                list.ContentTypesEnabled = true;
            }
        }

        protected virtual void ListHandler_OnProvisioned(ListProvisionHandler<TContext, TEntity> handler, List list)
        {
        }

        protected virtual void ContentTypeHandler_OnProvisioning(ContentTypeProvisionHandler<TContext, TEntity> handler, ContentType contentType)
        {
        }

        protected virtual void ContentTypeHandler_OnProvisioned(ContentTypeProvisionHandler<TContext, TEntity> handler, ContentType contentType)
        {
        }

        public void Provision()
        {
            if (ProvisionHandlers != null)
            {
                foreach (var provisionHandler in ProvisionHandlers)
                {
                    if (provisionHandler != null)
                        provisionHandler.Provision();
                }
            }
        }
    }
}
