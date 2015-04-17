namespace SharePoint.Remote.Access
{
    public interface IClientObject
    {
        bool IsLoaded { get; }
        string GetRestUrl();
    }
}