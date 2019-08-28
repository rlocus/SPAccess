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
        public int BatchSize { get; set; }
        public bool IncludeItemPermissions { get; set; }

        internal Dictionary<string, FieldAttribute> FieldMappings { get; }
        internal Caml.View SpView { get; set; }
        internal bool SkipResult { get; set; }
        internal bool IsAsync { get; set; }

        public SpQueryArgs(ClientContext context, string listTitle, string listUrl, Guid listId)
        {
            Context = context;
            ListTitle = listTitle;
            ListUrl = listUrl;
            ListId = listId;
            FieldMappings = new Dictionary<string, FieldAttribute>();
            BatchSize = 100;
            //SpView = new Caml.View();
            IncludeItemPermissions = true;
        }
    }
}
