using System.Collections.Generic;

namespace SharePoint.Remote.Access.Caml.Interfaces
{
    public interface ICamlMultiField
    {
        IEnumerable<CamlFieldRef> FieldRefs { get; }
    }
}