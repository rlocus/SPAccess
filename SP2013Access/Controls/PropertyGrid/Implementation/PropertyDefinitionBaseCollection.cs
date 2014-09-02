using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace SP2013Access.Controls.PropertyGrid
{
    public class PropertyDefinitionCollection : PropertyDefinitionBaseCollection<PropertyDefinition> { }

    public class EditorDefinitionCollection : PropertyDefinitionBaseCollection<EditorDefinitionBase> { }

    public abstract class PropertyDefinitionBaseCollection<T> : DefinitionCollectionBase<T> where T : PropertyDefinitionBase
    {
        public T this[object propertyId]
        {
            get
            {
                return base.Items.FirstOrDefault(current => current.TargetProperties.Contains(propertyId));
            }
        }
        internal PropertyDefinitionBaseCollection()
        {
        }
        internal T GetRecursiveBaseTypes(Type type)
        {
            T t = default(T);
            while (t == null && type != null)
            {
                t = this[type];
                type = type.BaseType;
            }
            return t;
        }
    }

    public abstract class DefinitionCollectionBase<T> : ObservableCollection<T> where T : DefinitionBase
    {
        internal DefinitionCollectionBase()
        {
        }
        protected override void InsertItem(int index, T item)
        {
            if (item == null)
            {
                throw new InvalidOperationException("Cannot insert null items in the collection.");
            }
            item.Lock();
            base.InsertItem(index, item);
        }
        protected override void SetItem(int index, T item)
        {
            if (item == null)
            {
                throw new InvalidOperationException("Cannot insert null items in the collection.");
            }
            item.Lock();
            base.SetItem(index, item);
        }
    }
}