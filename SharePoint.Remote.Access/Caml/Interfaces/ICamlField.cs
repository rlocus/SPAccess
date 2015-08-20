namespace SharePoint.Remote.Access.Caml.Interfaces
{
    public interface ICamlField : ICaml
    {
        CamlFieldRef FieldRef { get; }
    }
}