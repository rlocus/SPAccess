using System.Collections.Generic;

namespace SharePoint.Remote.Access.Caml.Interfaces
{
    internal interface ICamlMultiValue<T> : ICaml
    {
        IEnumerable<CamlValue<T>> Values { get; set; }
    }
}