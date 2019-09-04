using SP.Client.Linq.Attributes;
using System;
using System.Collections.Generic;

namespace SP.Client.Linq.Query
{
    public class SpQueryArgs<TContext>
        where TContext: ISpDataContext
    {
        public TContext Context { get; }
        public string ListTitle { get; }
        public string ListUrl { get; }
        public Guid ListId { get; }
        public string Query { get; }
        public int BatchSize { get; set; }
        public bool IncludeItemPermissions { get; set; }

        internal Dictionary<string, FieldAttribute> FieldMappings { get; }
        internal Caml.View SpView { get; set; }
        internal bool SkipResult { get; set; }
        internal bool IsAsync { get; set; }

        public SpQueryArgs(TContext context, string listTitle, string listUrl, Guid listId, string query)
        {
            Context = context;
            ListTitle = listTitle;
            ListUrl = listUrl;
            ListId = listId;
            Query = query;
            FieldMappings = new Dictionary<string, FieldAttribute>();
            BatchSize = 100;
            //SpView = new Caml.View();
            IncludeItemPermissions = true;
        }
    }
}
