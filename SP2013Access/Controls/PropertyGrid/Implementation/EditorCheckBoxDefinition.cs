using SP2013Access.Controls.PropertyGrid.Editors;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace SP2013Access.Controls.PropertyGrid
{
    public class EditorCheckBoxDefinition : EditorBoundDefinition
    {
        public bool IsThreeState
        {
            get;
            set;
        }

        protected override FrameworkElement GenerateEditingElement(PropertyItemBase propertyItem)
        {
            PropertyGridEditorCheckBox propertyGridEditorCheckBox = new PropertyGridEditorCheckBox
            {
                IsThreeState = this.IsThreeState
            };
            this.UpdateStyle(propertyGridEditorCheckBox);
            base.UpdateBinding(propertyGridEditorCheckBox, ToggleButton.IsCheckedProperty, propertyItem);
            return propertyGridEditorCheckBox;
        }
    }
}