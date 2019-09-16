using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.Client.Linq.Provisioning
{
    public abstract class ProvisionHandler<TContext, TEntity>
        where TContext : class, ISpEntryDataContext
        where TEntity : class, IListItemEntity
    {
        protected ProvisionHandler(ProvisionModel<TContext, TEntity> model)
        {
            Model = model;
        }

        public ProvisionModel<TContext, TEntity> Model { get; }

        public abstract void Provision();
    }
}
