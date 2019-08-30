using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.SharePoint.Client;
using SP.Client.Linq.Infrastructure;
using SP.Client.Linq.Query;

namespace SP.Client.Linq
{
    public interface ISpEntryDataContext : ISpDataContext
    {
        SpChangeTracker ChangeTracker { get; }

        void SaveChanges();
    }
}
