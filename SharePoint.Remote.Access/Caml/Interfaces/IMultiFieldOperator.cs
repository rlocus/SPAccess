using System.Collections.Generic;

namespace SharePoint.Remote.Access.Caml.Interfaces
{
    public interface IMultiFieldOperator
    {
        IEnumerable<FieldRef> FieldRefs { get; }
    }
}