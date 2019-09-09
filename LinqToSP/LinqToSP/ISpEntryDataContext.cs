﻿using SP.Client.Linq.Infrastructure;
using SP.Client.Linq.Query;
using System;

namespace SP.Client.Linq
{
    public interface ISpEntryDataContext : ISpDataContext
    {
        event Action<SpSaveArgs> OnBeforeSaveChanges;

        event Action<SpSaveArgs> OnAfterSaveChanges;

        bool SaveChanges();
        SpEntityQueryable<TListItem> List<TListItem>(SpQueryArgs<ISpEntryDataContext> args) where TListItem : class, IListItemEntity;
    }
}
