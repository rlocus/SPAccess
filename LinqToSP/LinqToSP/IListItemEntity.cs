using Microsoft.SharePoint.Client;
using SP.Client.Linq.Attributes;

namespace SP.Client.Linq
{
  public interface IListItemEntity
  {
    [Field(Name = "ID", Required = true, DataType = FieldType.Counter)]
    uint ID { get; }
  }
}
