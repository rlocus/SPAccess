using System.Collections.Generic;

namespace SharePoint.Remote.Access.Caml.Interfaces
{
    public interface ICamlMultiField : ICaml
    {
        IEnumerable<CamlFieldRef> FieldRefs { get; }
    }
}