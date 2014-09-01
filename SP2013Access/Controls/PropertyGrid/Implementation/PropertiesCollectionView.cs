using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace SP2013Access.Controls.PropertyGrid
{
    internal class PropertiesCollectionView : ListCollectionView, IList
    {
        private readonly string _invalidOperationMessage;

        private IList SourceList
        {
            get
            {
                return (IList)this.SourceCollection;
            }
        }

        private bool IsReadOnly
        {
            get
            {
                return this._invalidOperationMessage != null;
            }
        }

        bool IList.IsFixedSize
        {
            get
            {
                return false;
            }
        }

        bool IList.IsReadOnly
        {
            get
            {
                return this.IsReadOnly;
            }
        }

        object IList.this[int index]
        {
            get
            {
                return this.SourceList[index];
            }
            set
            {
                this.ThrowIfReadOnly();
                this.SourceList[index] = value;
            }
        }

        int ICollection.Count
        {
            get
            {
                return this.SourceList.Count;
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return this.SourceList.IsSynchronized;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                return this.SourceList.SyncRoot;
            }
        }

        public PropertiesCollectionView()
            : base(new ObservableCollection<object>())
        {
            this._invalidOperationMessage = null;
        }

        public PropertiesCollectionView(IList sourceList, string invalidOperationMessage)
            : base(sourceList)
        {
            this._invalidOperationMessage = ((invalidOperationMessage != null) ? invalidOperationMessage : string.Empty);
        }

        private void ThrowIfReadOnly()
        {
            if (this.IsReadOnly)
            {
                throw new InvalidOperationException(this._invalidOperationMessage);
            }
        }

        int IList.Add(object value)
        {
            this.ThrowIfReadOnly();
            return this.SourceList.Add(value);
        }

        void IList.Clear()
        {
            this.ThrowIfReadOnly();
            this.SourceList.Clear();
        }

        bool IList.Contains(object value)
        {
            return this.SourceList.Contains(value);
        }

        int IList.IndexOf(object value)
        {
            return this.SourceList.IndexOf(value);
        }

        void IList.Insert(int index, object value)
        {
            this.ThrowIfReadOnly();
            this.SourceList.Insert(index, value);
        }

        void IList.Remove(object value)
        {
            this.ThrowIfReadOnly();
            this.SourceList.Remove(value);
        }

        void IList.RemoveAt(int index)
        {
            this.ThrowIfReadOnly();
            this.SourceList.RemoveAt(index);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            this.SourceList.CopyTo(array, index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.SourceList.GetEnumerator();
        }
    }
}