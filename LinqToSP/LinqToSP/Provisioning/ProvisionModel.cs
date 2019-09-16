using SP.Client.Linq.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.Client.Linq.Provisioning
{
    public sealed class ProvisionModel<TContext, TEntity>
        where TContext : class, ISpEntryDataContext
        where TEntity : class, IListItemEntity
    {
        public TContext Context { get; }

        private List<ProvisionHandler<TContext, TEntity>> ProvisionHandlers { get; set; }

        public ProvisionModel(TContext context)
        {
            Context = context;
            ProvisionHandlers = new List<ProvisionHandler<TContext, TEntity>>();
        }

        private void RetrieveHandlers()
        {
           var contentType = AttributeHelper.GetCustomAttribute<TEntity, ContentTypeAttribute>();
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
