using System.Windows;
using System.Windows.Controls;

namespace SP2013Access.Controls.PropertyGrid.Editors
{
    public class TextBoxEditor : TypeEditor<TextBox>
    {
        protected override TextBox CreateEditor()
        {
            return new PropertyGridEditorTextBox();
        }

        protected override void SetValueDependencyProperty()
        {
            ValueProperty = TextBox.TextProperty;
        }
    }

    public class PropertyGridEditorTextBox : TextBox
    {
        static PropertyGridEditorTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridEditorTextBox), new FrameworkPropertyMetadata(typeof(PropertyGridEditorTextBox)));
        }
    }
}