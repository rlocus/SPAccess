using System.Windows;

namespace SP2013Access.Controls.PropertyGrid
{
    public class EditorTemplateDefinition : EditorDefinitionBase
    {
        public static readonly DependencyProperty EditingTemplateProperty = DependencyProperty.Register("EditingTemplate", typeof(DataTemplate), typeof(EditorTemplateDefinition), new UIPropertyMetadata(null));
        public DataTemplate EditingTemplate
        {
            get
            {
                return (DataTemplate)base.GetValue(EditorTemplateDefinition.EditingTemplateProperty);
            }
            set
            {
                base.SetValue(EditorTemplateDefinition.EditingTemplateProperty, value);
            }
        }
        protected override FrameworkElement GenerateEditingElement(PropertyItemBase propertyItem)
        {
            if (this.EditingTemplate == null)
            {
                return null;
            }
            return this.EditingTemplate.LoadContent() as FrameworkElement;
        }
    }
}