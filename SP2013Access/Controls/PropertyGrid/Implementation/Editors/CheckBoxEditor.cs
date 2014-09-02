using System.Windows;
using System.Windows.Controls;

namespace SP2013Access.Controls.PropertyGrid.Editors
{
    public class CheckBoxEditor : TypeEditor<CheckBox>
    {
        protected override CheckBox CreateEditor()
        {
            return new PropertyGridEditorCheckBox();
        }

        protected override void SetControlProperties()
        {
            Editor.Margin = new Thickness(5, 0, 0, 0);
        }

        protected override void SetValueDependencyProperty()
        {
            ValueProperty = CheckBox.IsCheckedProperty;
        }
    }

    public class PropertyGridEditorCheckBox : CheckBox
    {
        static PropertyGridEditorCheckBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridEditorCheckBox), new FrameworkPropertyMetadata(typeof(PropertyGridEditorCheckBox)));
        }
    }
}