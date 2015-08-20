using System.Collections.Generic;

namespace SharePoint.Remote.Access.Caml.Interfaces
{
    internal interface IMultiValueOperator<T>
    {
        IEnumerable<CamlValue<T>> Values { get; set; }
    }
}