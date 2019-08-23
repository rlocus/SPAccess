using Microsoft.SharePoint.Client;
using SP.Client.Linq.Attributes;

namespace SP.Client.Linq
{
    public class ListItemEntity : IListItemEntity
    {
        public ListItemEntity()
        {

        }
        public ListItemEntity(uint id)
        {
            ID = id;
        }

        //[Field(Name = "ID", Required = true, DataType = FieldType.Counter)]
        public uint ID { get; protected set; }

        [Field("Title", FieldType.Text, Required = false)]
        public string Title { get; set; }
    }
}
