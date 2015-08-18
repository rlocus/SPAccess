using System.Collections.Generic;

namespace SharePoint.Remote.Access.Caml.Interfaces
{
    internal interface IMultiValueOperator<T>
    {
        IEnumerable<Value<T>> Values { get; set; }
    }
}