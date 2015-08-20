using System;
using System.Xml.Linq;
using SharePoint.Remote.Access.Caml.Interfaces;

namespace SharePoint.Remote.Access.Caml
{
    public abstract class CamlElement : ICaml
    {
        protected CamlElement(string elementName)
        {
            ElementName = elementName;
        }

        protected CamlElement(string elementName, string existingElement)
        {
            ElementName = elementName;
            Parse(existingElement);
        }

        protected CamlElement(string elementName, XElement existingElement)
        {
            ElementName = elementName;
            Parse(existingElement);
        }

        public string ElementName { get; }

        private void Parse(XElement existingElement)
        {
            if (existingElement == null) throw new ArgumentNullException(nameof(existingElement));
            if (string.Equals(existingElement.Name.LocalName, ElementName, StringComparison.OrdinalIgnoreCase))
            {
                if ((existingElement.HasAttributes || existingElement.HasElements))
                {
                    OnParsing(existingElement);
                }
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
                var el = XElement.Parse(existingElement, LoadOptions.None);
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
            return ToString(false);
        }

        public string ToString(bool disableFormatting)
        {
            return disableFormatting
                ? ToXElement().ToString(SaveOptions.DisableFormatting)
                : ToXElement().ToString(SaveOptions.None);
        }

        public static implicit operator string (CamlElement caml)
        {
            return caml?.ToString() ?? string.Empty;
        }
    }
}