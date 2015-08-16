using System;
using System.Xml.Linq;

namespace SharePoint.Remote.Access.Caml
{
    public abstract class QueryElement
    {
        public string ElementName { get; }

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

        private void Parse(XElement existingElement)
        {
            if (existingElement == null) throw new ArgumentNullException(nameof(existingElement));
            if (string.Equals(existingElement.Name.LocalName, ElementName, StringComparison.OrdinalIgnoreCase)
                && (existingElement.HasAttributes || existingElement.HasElements))
            {
                OnParsing(existingElement);
            }
            else
            {
                throw new NotSupportedException(nameof(existingElement.Name));
            }
        }

        private void Parse(string existingElement)
        {
            if (!string.IsNullOrEmpty(existingElement))
            {
                XElement el = XElement.Parse(existingElement, LoadOptions.None);
                Parse(el);
            }
        }

        protected abstract void OnParsing(XElement existingElement);

        public virtual XElement ToXElement()
        {
            return new XElement(ElementName);
        }

        public override string ToString()
        {
            return ToXElement().ToString();
        }
    }
}