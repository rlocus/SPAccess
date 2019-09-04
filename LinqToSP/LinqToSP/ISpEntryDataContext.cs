using SP.Client.Linq.Infrastructure;
using System;

namespace SP.Client.Linq
{
    public interface ISpEntryDataContext : ISpDataContext
    {
        bool HasChanges { get; }

        event Action<SpSaveArgs> OnSaveChanges;
        void SaveChanges();
    }
}
