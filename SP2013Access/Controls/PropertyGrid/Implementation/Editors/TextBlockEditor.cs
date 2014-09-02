using System.Windows;
using System.Windows.Controls;

namespace SP2013Access.Controls.PropertyGrid.Editors
{
    public class TextBlockEditor : TypeEditor<TextBlock>
    {
        protected override TextBlock CreateEditor()
        {
            return new PropertyGridEditorTextBlock();
        }

        protected override void SetValueDependencyProperty()
        {
            ValueProperty = TextBlock.TextProperty;
        }

        protected override void SetControlProperties()
        {
            Editor.Margin = new Thickness(5, 0, 0, 0);
        }
    }

    public class PropertyGridEditorTextBlock : TextBlock
    {
        static PropertyGridEditorTextBlock()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridEditorTextBlock), new FrameworkPropertyMetadata(typeof(PropertyGridEditorTextBlock)));
        }
    }
}