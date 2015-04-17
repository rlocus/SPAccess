using SP2013Access.Controls.PropertyGrid.Editors;
using System;
using System.Collections;
using System.Windows;
using System.Windows.Data;

namespace SP2013Access.Controls.PropertyGrid
{
    internal class PropertiesSourceContainerHelper : PropertiesContainerHelperBase
    {
        private const string ReadOnlyCollectionExceptionMessage = "You cannot modify this collection directly when using PropertiesSource with the PropertyGrid. Have the content of PropertiesSource implement IList and INotifyCollectionChanged then modify the orignial source";


        public PropertiesSourceContainerHelper(IPropertyContainer propertyContainer)
            : base(propertyContainer)
        {
            base.CollectionView = new PropertiesCollectionView();
        }

        public PropertiesSourceContainerHelper(IPropertyContainer propertyContainer, IEnumerable propertiesSource)
            : base(propertyContainer)
        {
            if (propertiesSource == null)
            {
                throw new ArgumentNullException("propertiesSource");
            }
            IList list = propertiesSource as IList;
            if (list == null)
            {
                list = new ArrayList();

                foreach (object current in propertiesSource)
                {
                    list.Add(current);
                }
            }
            list = new WeakCollectionChangedWrapper(list);
            base.CollectionView = new PropertiesCollectionView(list, PropertiesSourceContainerHelper.ReadOnlyCollectionExceptionMessage);
        }

        public override void ClearHelper()
        {
            WeakCollectionChangedWrapper weakCollectionChangedWrapper = base.CollectionView.SourceCollection as WeakCollectionChangedWrapper;
            if (weakCollectionChangedWrapper != null)
            {
                weakCollectionChangedWrapper.ReleaseEvents();
            }
            base.ClearHelper();
        }

        public override void PrepareChildrenPropertyItem(PropertyItemBase propertyItem, object item)
        {
            base.PrepareChildrenPropertyItem(propertyItem, item);
            if (propertyItem.Editor == null)
            {
                FrameworkElement frameworkElement = this.GenerateChildrenEditorElement((CustomPropertyItem)propertyItem);
                if (frameworkElement != null)
                {
                    ContainerHelperBase.SetIsGenerated(frameworkElement, true);
                    propertyItem.Editor = frameworkElement;
                }
            }
        }

        public override void ClearChildrenPropertyItem(PropertyItemBase propertyItem, object item)
        {
            if (propertyItem.Editor != null && ContainerHelperBase.GetIsGenerated(propertyItem.Editor))
            {
                propertyItem.Editor = null;
            }
            base.ClearChildrenPropertyItem(propertyItem, item);
        }

        public override Binding CreateChildrenDefaultBinding(PropertyItemBase propertyItem)
        {
            return new Binding("Value")
            {
                Mode = ((CustomPropertyItem)propertyItem).IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay
            };
        }

        private FrameworkElement GenerateChildrenEditorElement(CustomPropertyItem propertyItem)
        {
            FrameworkElement frameworkElement = null;
            object definitionKey = propertyItem.DefinitionKey;
            Type type = definitionKey as Type;
            ITypeEditor typeEditor = null;
            if (propertyItem.IsReadOnly)
            {
                typeEditor = new TextBlockEditor();
            }
            if (typeEditor != null)
            {
                frameworkElement = typeEditor.ResolveEditor(propertyItem);
            }
            if (frameworkElement == null && definitionKey != null)
            {
                frameworkElement = base.GenerateCustomEditingElement(definitionKey, propertyItem);
            }
            if (frameworkElement == null && type != null)
            {
                frameworkElement = base.GenerateCustomEditingElement(type, propertyItem);
            }
            if (frameworkElement == null && definitionKey == null)
            {
                frameworkElement = base.GenerateCustomEditingElement(propertyItem.Name, propertyItem);
            }
            if (frameworkElement == null && type == null)
            {
                frameworkElement = base.GenerateCustomEditingElement(propertyItem.Value.GetType(), propertyItem);
            }
            if (frameworkElement == null)
            {
                if (typeEditor == null)
                {
                    typeEditor = ((type != null) ? PropertyGridUtilities.CreateDefaultEditor(type, null) : null);
                }
                if (typeEditor != null) frameworkElement = typeEditor.ResolveEditor(propertyItem);
            }
            return frameworkElement;
        }

        //private void SetupDefinitionBinding<T>(PropertyItemBase propertyItem, DependencyProperty itemProperty, DescriptorPropertyDefinitionBase pd, Expression<Func<T>> definitionProperty, BindingMode bindingMode)
        //{
        //    string propertyOrFieldName = ReflectionHelper.GetPropertyOrFieldName(definitionProperty);
        //    Binding binding = new Binding(propertyOrFieldName)
        //    {
        //        Source = pd,
        //        Mode = bindingMode
        //    };
        //    propertyItem.SetBinding(itemProperty, binding);
        //}
    }
}