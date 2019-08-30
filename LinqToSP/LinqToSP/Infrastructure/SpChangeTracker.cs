using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.Client.Linq.Infrastructure
{ 
    public class SpChangeTracker
    {
        public void DetectChanges() { }

        public IEnumerable<SpEntityEntry> Entries() { return null; }

        //public IEnumerable<SpEntityEntry<TEntity>> Entries<TEntity>() where TEntity : class
        //{
        //    return null;
        //}

        public bool HasChanges() { return false; }
    }
}
