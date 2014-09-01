using Microsoft.SharePoint.Client;
using SharePoint.Remote.Access.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharePoint.Remote.Access.Extensions
{
    public static class ContentTypeExtensions
    {
        public static FieldCollection GetFieldCollection(this ContentType contentType)
        {
            FieldCollection fields = contentType.Fields;

            if (!fields.AreItemsAvailable)
            {
                contentType.Context.Load(fields);
                contentType.Context.ExecuteQuery();
            }
            return fields;
        }

        public static async Task<FieldCollection> GetFieldCollectionAsync(this ContentType contentType)
        {
            FieldCollection fields = contentType.Fields;

            if (!fields.AreItemsAvailable)
            {
                contentType.Context.Load(fields);
                await contentType.Context.ExecuteQueryAsync();
            }
            return fields;
        }

        public static IEnumerable<Field> LoadFields(this ContentType contentType)
        {
            int count;
            return LoadFields(contentType, out count);
        }

        public static IEnumerable<SPClientField> LoadFields(this ContentType contentType, out int count)
        {
            FieldCollection fields = GetFieldCollection(contentType);
            contentType.Context.ExecuteQuery();
            count = fields.Count;
            var clientFields = new List<SPClientField>();

            if (fields.Count > 0)
            {
                foreach (Field field in fields)
                {
                    SPClientField clientField = SPClientField.FromField(field);
                    contentType.Context.Load(clientField);
                    clientFields.Add(clientField);
                }
                contentType.Context.ExecuteQuery();
            }
            return clientFields.Where(clientField => clientField != null);
        }

        public static async Task<IEnumerable<Field>> LoadFieldsAsync(this ContentType contentType)
        {
            FieldCollection fields = await GetFieldCollectionAsync(contentType);
            contentType.Context.ExecuteQuery();
            var clientFields = new List<SPClientField>();

            if (fields.Count > 0)
            {
                foreach (Field field in fields)
                {
                    SPClientField clientField = SPClientField.FromField(field);
                    contentType.Context.Load(clientField);
                    clientFields.Add(clientField);
                }
                await contentType.Context.ExecuteQueryAsync();
            }
            return clientFields.Where(clientField => clientField != null);
        }
    }
}