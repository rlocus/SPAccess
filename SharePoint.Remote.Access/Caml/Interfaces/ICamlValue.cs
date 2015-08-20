namespace SharePoint.Remote.Access.Caml.Interfaces
{
    internal interface IValueOperator<T>
    {
        CamlValue<T> Value { get; }
    }
}