using Microsoft.SharePoint;
using Microsoft.SharePoint.Client;

namespace SharePoint.Remote.Access.Caml.Interfaces
{
    public interface ICamlSpecialValue : ICaml
    {
        bool IsSupported(FieldType fieldType);
    }
}