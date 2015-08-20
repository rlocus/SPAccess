namespace SharePoint.Remote.Access.Caml.Interfaces
{
    internal interface ICamlValue<T> : ICaml
    {
        CamlValue<T> Value { get; }
    }
}