using Microsoft.SharePoint.Client;
using SP.Client.Linq.Attributes;

namespace SP.Client.Linq
{
  public class ListItemEntity : IListItemEntity
  {
    public ListItemEntity()
    {

    }

    protected ListItemEntity(int id)
    {
      Id = id;
    }

    //[Field(Name = "ID", Required = true, DataType = FieldType.Counter)]
    public int Id { get; internal set; }

    [Field("Title", FieldType.Text, Required = false)]
    public string Title { get; set; }
  }
}
