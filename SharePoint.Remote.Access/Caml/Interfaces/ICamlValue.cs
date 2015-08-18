namespace SharePoint.Remote.Access.Caml.Interfaces
{
    internal interface IValueOperator<T>
    {
        Value<T> Value { get; }
    }
}