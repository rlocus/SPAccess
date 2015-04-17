using SP2013Access.Controls.PropertyGrid.Editors;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace SP2013Access.Controls.PropertyGrid
{
    public class EditorComboBoxDefinition : EditorDefinitionBase
    {
        public static readonly DependencyProperty DisplayMemberPathProperty = ItemsControl.DisplayMemberPathProperty.AddOwner(typeof(EditorComboBoxDefinition));
        public static readonly DependencyProperty SelectedValuePathProperty = Selector.SelectedValuePathProperty.AddOwner(typeof(EditorComboBoxDefinition));
        public static readonly DependencyProperty EditingElementStyleProperty = EditorBoundDefinition.EditingElementStyleProperty.AddOwner(typeof(EditorComboBoxDefinition));
        public static readonly DependencyProperty ItemsSourceProperty = ItemsControl.ItemsSourceProperty.AddOwner(typeof(EditorComboBoxDefinition));

        public string DisplayMemberPath
        {
            get
            {
                return (string)base.GetValue(EditorComboBoxDefinition.DisplayMemberPathProperty);
            }
            set
            {
                base.SetValue(EditorComboBoxDefinition.DisplayMemberPathProperty, value);
            }
        }

        public string SelectedValuePath
        {
            get
            {
                return (string)base.GetValue(EditorComboBoxDefinition.SelectedValuePathProperty);
            }
            set
            {
                base.SetValue(EditorComboBoxDefinition.SelectedValuePathProperty, value);
            }
        }

        public Style EditingElementStyle
        {
            get
            {
                return (Style)base.GetValue(EditorComboBoxDefinition.EditingElementStyleProperty);
            }
            set
            {
                base.SetValue(EditorComboBoxDefinition.EditingElementStyleProperty, value);
            }
        }

        public IEnumerable ItemsSource
        {
            get
            {
                return (IEnumerable)base.GetValue(EditorComboBoxDefinition.ItemsSourceProperty);
            }
            set
            {
                base.SetValue(EditorComboBoxDefinition.ItemsSourceProperty, value);
            }
        }

        public BindingBase SelectedItemBinding
        {
            get;
            set;
        }

        public BindingBase SelectedValueBinding
        {
            get;
            set;
        }

        public BindingBase TextBinding
        {
            get;
            set;
        }

        //public EditorComboBoxDefinition()
        //{
        //}

        protected override FrameworkElement GenerateEditingElement(PropertyItemBase propertyItem)
        {
            PropertyGridEditorComboBox propertyGridEditorComboBox = new PropertyGridEditorComboBox();
            base.UpdateProperty(propertyGridEditorComboBox, ItemsControl.DisplayMemberPathProperty, EditorComboBoxDefinition.DisplayMemberPathProperty);
            base.UpdateProperty(propertyGridEditorComboBox, Selector.SelectedValuePathProperty, EditorComboBoxDefinition.SelectedValuePathProperty);
            base.UpdateProperty(propertyGridEditorComboBox, ItemsControl.ItemsSourceProperty, EditorComboBoxDefinition.ItemsSourceProperty);
            this.UpdateStyle(propertyGridEditorComboBox);
            if (this.SelectedItemBinding == null && this.SelectedValueBinding == null)
            {
                this.UpdateBinding(propertyGridEditorComboBox,
                    this.SelectedValuePath == null ? Selector.SelectedItemProperty : Selector.SelectedValueProperty,
                    PropertyGridUtilities.GetDefaultBinding(propertyItem));
            }
            else
            {
                this.UpdateBinding(propertyGridEditorComboBox, Selector.SelectedItemProperty, this.SelectedItemBinding);
                this.UpdateBinding(propertyGridEditorComboBox, Selector.SelectedValueProperty, this.SelectedValueBinding);
            }
            this.UpdateBinding(propertyGridEditorComboBox, ComboBox.TextProperty, this.TextBinding);
            return propertyGridEditorComboBox;
        }

        internal void UpdateStyle(FrameworkElement element)
        {
            if (this.EditingElementStyle != null)
            {
                element.Style = this.EditingElementStyle;
            }
        }

        private void UpdateBinding(FrameworkElement editor, DependencyProperty editorProperty, BindingBase binding)
        {
            if (binding == null)
            {
                BindingOperations.ClearBinding(editor, editorProperty);
                return;
            }
            BindingOperations.SetBinding(editor, editorProperty, binding);
        }
    }
}