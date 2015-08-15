using System;
using System.Xml.Linq;

namespace SharePoint.Remote.Access.Caml
{
    public abstract class QueryElement
    {
        protected QueryElement(string elementName)
        {
            ElementName = elementName;
        }

        protected QueryElement(string elementName, string existingElement)
        {
            ElementName = elementName;
            Parse(existingElement);
        }

        protected QueryElement(string elementName, XElement existingElement)
        {
            ElementName = elementName;
            Parse(existingElement);
        }

        public string ElementName { get; }

        public virtual XElement ToXElement()
        {
            return new XElement(ElementName);
        }

        protected abstract void OnParsing(XElement existingElement);

        private void Parse(XElement existingElement)
        {
            if (string.Equals(existingElement.Name.LocalName, ElementName, StringComparison.InvariantCultureIgnoreCase)
                && (existingElement.HasAttributes || existingElement.HasElements))
            {
                OnParsing(existingElement);
            }
        }

        private void Parse(string existingElement)
        {
            if (!string.IsNullOrEmpty(existingElement))
            {
                var el = XElement.Parse(existingElement, LoadOptions.None);
                Parse(el);
            }
        }

        public override string ToString()
        {
            return ToXElement().ToString();
        }
    }
}