using System.Collections;
using System.Windows;

namespace SP2013Access.Controls.PropertyGrid.Editors
{
    public abstract class ComboBoxEditor : TypeEditor<System.Windows.Controls.ComboBox>
    {
        protected override void SetValueDependencyProperty()
        {
            ValueProperty = System.Windows.Controls.ComboBox.SelectedItemProperty;
        }

        protected override System.Windows.Controls.ComboBox CreateEditor()
        {
            return new PropertyGridEditorComboBox();
        }

        protected override void ResolveValueBinding(PropertyItemBase propertyItem)
        {
            SetItemsSource(propertyItem);
            base.ResolveValueBinding(propertyItem);
        }

        protected abstract IEnumerable CreateItemsSource(PropertyItemBase propertyItem);

        private void SetItemsSource(PropertyItemBase propertyItem)
        {
            Editor.ItemsSource = CreateItemsSource(propertyItem);
        }
    }

    public class PropertyGridEditorComboBox : System.Windows.Controls.ComboBox
    {
        static PropertyGridEditorComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridEditorComboBox), new FrameworkPropertyMetadata(typeof(PropertyGridEditorComboBox)));
        }
    }
}