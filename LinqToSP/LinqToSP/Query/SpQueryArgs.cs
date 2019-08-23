using Microsoft.SharePoint.Client;
using SP.Client.Linq.Attributes;
using System;
using System.Collections.Generic;

namespace SP.Client.Linq.Query
{
  public class SpQueryArgs
  {
    public ClientContext Context { get; }
    public string ListTitle { get; }
    public string ListUrl { get; }
    public Guid ListId { get; private set; }
    internal Dictionary<string, FieldAttribute> ColumnMappings { get; }
    internal Caml.View SpView { get; }
    internal bool SkipResult { get; set; }

    public SpQueryArgs(ClientContext context, string listTitle, string listUrl, Guid listId)
    {
      Context = context;
      ListTitle = listTitle;
      ListUrl = listUrl;
      ListId = listId;
      ColumnMappings = new Dictionary<string, FieldAttribute>();
      SpView = new Caml.View();
    }
  }
}
