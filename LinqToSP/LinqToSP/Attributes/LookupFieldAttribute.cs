using Microsoft.SharePoint.Client;

namespace SP.Client.Linq.Attributes
{
  public class LookupFieldAttribute : FieldAttribute
  {
    public LookupFieldAttribute()
    {
      DataType = FieldType.Lookup;
    }

    public LookupFieldAttribute(string name) : base(name, FieldType.Lookup)
    {
    }

    public bool IsLookupId { get; set; }
  }
}
