using System.Linq;
using System.Reflection;

namespace SP2013Access.Controls.PropertyGrid
{
    public class CategoryDefinitionCollection : DefinitionCollectionBase<CategoryDefinition>
    {
        public CategoryDefinition this[object categoryId]
        {
            get
            {
                return base.Items.FirstOrDefault(current => object.Equals(current.Name, categoryId));
            }
        }
    }
}