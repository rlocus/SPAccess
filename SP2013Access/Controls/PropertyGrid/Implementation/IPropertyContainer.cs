using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace SP2013Access.Controls.PropertyGrid
{
    internal interface IPropertyContainer
    {
        Binding PropertyNameBinding
        {
            get;
        }

        Binding PropertyValueBinding
        {
            get;
        }

        EditorDefinitionBase DefaultEditorDefinition
        {
            get;
        }

        GroupDescription CategoryGroupDescription
        {
            get;
        }

        CategoryDefinitionCollection CategoryDefinitions
        {
            get;
        }

        ContainerHelperBase ContainerHelper
        {
            get;
        }

        Style PropertyContainerStyle
        {
            get;
        }

        EditorDefinitionCollection EditorDefinitions
        {
            get;
        }

        PropertyDefinitionCollection PropertyDefinitions
        {
            get;
        }

        bool IsCategorized
        {
            get;
        }

        //bool AutoGenerateProperties
        //{
        //    get;
        //}

        FilterInfo FilterInfo
        {
            get;
        }
    }
}