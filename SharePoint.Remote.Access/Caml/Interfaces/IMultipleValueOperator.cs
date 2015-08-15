using System.Collections.Generic;

namespace SharePoint.Remote.Access.Caml.Interfaces
{
    internal interface IMultipleValueOperator<T>
    {
        IEnumerable<Value<T>> Values { get; set; }
    }
}