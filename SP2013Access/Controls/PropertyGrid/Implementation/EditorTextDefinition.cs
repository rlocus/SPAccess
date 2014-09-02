using SP2013Access.Controls.PropertyGrid.Editors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SP2013Access.Controls.PropertyGrid
{
    public class EditorTextDefinition : EditorBoundDefinition
    {
        public static readonly DependencyProperty FontFamilyProperty = Control.FontFamilyProperty.AddOwner(typeof(EditorTextDefinition));
        public static readonly DependencyProperty FontSizeProperty = Control.FontSizeProperty.AddOwner(typeof(EditorTextDefinition));
        public static readonly DependencyProperty FontStyleProperty = Control.FontStyleProperty.AddOwner(typeof(EditorTextDefinition));
        public static readonly DependencyProperty FontWeightProperty = Control.FontWeightProperty.AddOwner(typeof(EditorTextDefinition));
        public static readonly DependencyProperty ForegroundProperty = Control.ForegroundProperty.AddOwner(typeof(EditorTextDefinition));

        public FontFamily FontFamily
        {
            get
            {
                return (FontFamily)base.GetValue(EditorTextDefinition.FontFamilyProperty);
            }
            set
            {
                base.SetValue(EditorTextDefinition.FontFamilyProperty, value);
            }
        }

        public double FontSize
        {
            get
            {
                return (double)base.GetValue(EditorTextDefinition.FontSizeProperty);
            }
            set
            {
                base.SetValue(EditorTextDefinition.FontSizeProperty, value);
            }
        }

        public FontStyle FontStyle
        {
            get
            {
                return (FontStyle)base.GetValue(EditorTextDefinition.FontStyleProperty);
            }
            set
            {
                base.SetValue(EditorTextDefinition.FontStyleProperty, value);
            }
        }

        public FontWeight FontWeight
        {
            get
            {
                return (FontWeight)base.GetValue(EditorTextDefinition.FontWeightProperty);
            }
            set
            {
                base.SetValue(EditorTextDefinition.FontWeightProperty, value);
            }
        }

        public Brush Foreground
        {
            get
            {
                return (Brush)base.GetValue(EditorTextDefinition.ForegroundProperty);
            }
            set
            {
                base.SetValue(EditorTextDefinition.ForegroundProperty, value);
            }
        }

        protected override FrameworkElement GenerateEditingElement(PropertyItemBase propertyItem)
        {
            PropertyGridEditorTextBox propertyGridEditorTextBox = new PropertyGridEditorTextBox();
            base.UpdateProperty(propertyGridEditorTextBox, Control.FontFamilyProperty, EditorTextDefinition.FontFamilyProperty);
            base.UpdateProperty(propertyGridEditorTextBox, Control.FontSizeProperty, EditorTextDefinition.FontSizeProperty);
            base.UpdateProperty(propertyGridEditorTextBox, Control.FontStyleProperty, EditorTextDefinition.FontStyleProperty);
            base.UpdateProperty(propertyGridEditorTextBox, Control.FontWeightProperty, EditorTextDefinition.FontWeightProperty);
            base.UpdateProperty(propertyGridEditorTextBox, Control.ForegroundProperty, EditorTextDefinition.ForegroundProperty);
            this.UpdateStyle(propertyGridEditorTextBox);
            base.UpdateBinding(propertyGridEditorTextBox, TextBox.TextProperty, propertyItem);
            return propertyGridEditorTextBox;
        }
    }
}