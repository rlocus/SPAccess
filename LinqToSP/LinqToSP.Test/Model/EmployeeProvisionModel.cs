using Microsoft.SharePoint.Client;
using SP.Client.Linq;
using SP.Client.Linq.Provisioning;

namespace LinqToSP.Test.Model
{
  internal class EmployeeProvisionModel<TContext> : SpProvisionModel<TContext, Employee>
        where TContext : class, ISpEntryDataContext
    {
        public EmployeeProvisionModel(TContext context)
           : base(context)
        {

        }

        protected override void ContentTypeHandler_OnProvisioning(ContentTypeProvisionHandler<TContext, Employee> handler, ContentType contentType)
        {
            base.ContentTypeHandler_OnProvisioning(handler, contentType);
        }

        protected override void FieldHandler_OnProvisioning(FieldProvisionHandler<TContext, Employee> handler, Field field)
        {
            base.FieldHandler_OnProvisioning(handler, field);

            if (field.FieldTypeKind == FieldType.Lookup)
            {

            }
        }

        protected override void FieldHandler_OnProvisioned(FieldProvisionHandler<TContext, Employee> handler, Field field)
        {
            base.FieldHandler_OnProvisioned(handler, field);
        }
    }
}
