using Microsoft.SharePoint.Client;
using SP.Client.Linq;
using SP.Client.Linq.Provisioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqToSP.Test
{
    internal class EmailProvisionModel<TContext> : SpProvisionModel<TContext, Email>
        where TContext : class, ISpEntryDataContext
    {
        public EmailProvisionModel(TContext context)
           : base(context)
        {

        }

        protected override void ContentTypeHandler_OnProvisioning(ContentTypeProvisionHandler<TContext, Email> handler, ContentType contentType)
        {
            base.ContentTypeHandler_OnProvisioning(handler, contentType);
        }

        protected override void FieldHandler_OnProvisioning(FieldProvisionHandler<TContext, Email> handler, Field field)
        {
            base.FieldHandler_OnProvisioning(handler, field);

            if (field.FieldTypeKind == FieldType.Lookup)
            {

            }
        }

        protected override void FieldHandler_OnProvisioned(FieldProvisionHandler<TContext, Email> handler, Field field)
        {
            base.FieldHandler_OnProvisioned(handler, field);
        }
    }
}
