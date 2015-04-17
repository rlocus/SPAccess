using System.Windows;

namespace SP2013Access.Controls.PropertyGrid.Editors
{
    public interface ITypeEditor
    {
        FrameworkElement ResolveEditor(PropertyItemBase propertyItem);
    }
}