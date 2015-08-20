using System.Xml.Linq;

namespace SharePoint.Remote.Access.Caml.Interfaces
{
    public interface ICaml
    {
        string ElementName { get; }
        XElement ToXElement();
    }
}