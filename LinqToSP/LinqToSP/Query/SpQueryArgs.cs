using SP.Client.Linq.Attributes;
using System.Collections.Generic;

namespace SP.Client.Linq.Query
{
  public class SpQueryArgs
  {
    public string ListName { get; }
    internal Dictionary<string, FieldAttribute> ColumnMappings { get; }
    internal Caml.View SpView { get; }

    public SpQueryArgs(string listName)
    {
      ListName = listName;
      ColumnMappings = new Dictionary<string, FieldAttribute>();
      SpView = new Caml.View();
    }
  }
}
