﻿using System;
using System.Linq;
using System.Xml.Linq;
using SharePoint.Remote.Access.Extensions;

namespace SharePoint.Remote.Access.Caml
{
    public sealed class LeftJoin : Join
    {
        internal const string Left = "LEFT";

        public LeftJoin(string fieldName, string primaryListAlias, string listAlias) : base(fieldName, primaryListAlias, listAlias)
        {
        }

        public LeftJoin(string fieldName, string listAlias) : base(fieldName, null, listAlias)
        {
        }

        public LeftJoin(string existingElement) : base(existingElement)
        {
        }

        public LeftJoin(XElement existingElement) : base(existingElement)
        {
        }

        public override XElement ToXElement()
        {
            var el = base.ToXElement();
            el.ReplaceAttributes(new XAttribute(TypeAttr, Left), el.Attributes());
            return el;
        }
    }

    public sealed class InnerJoin : Join
    {
        internal const string Inner = "INNER";

        public InnerJoin(string fieldName, string primaryListAlias, string listAlias) : base(fieldName, primaryListAlias, listAlias)
        {
        }

        public InnerJoin(string fieldName, string listAlias) : base(fieldName, null, listAlias)
        {
        }

        public InnerJoin(string existingElement) : base(existingElement)
        {
        }

        public InnerJoin(XElement existingElement) : base(existingElement)
        {
        }

        public override XElement ToXElement()
        {
            var el = base.ToXElement();
            el.ReplaceAttributes(new XAttribute(TypeAttr, Inner), el.Attributes());
            return el;
        }
    }

    public abstract class Join : CamlElement
    {
        internal const string JoinTag = "Join";
        internal const string TypeAttr = "Type";
        internal const string ListAliasAttr = "ListAlias";

        internal JoinComparison JoinComparison { get; private set; }
        public string ListAlias { get; private set; }

        protected Join(string fieldName, string primaryListAlias, string listAlias) : base(JoinTag)
        {
            if (string.IsNullOrWhiteSpace(listAlias)) throw new ArgumentNullException(nameof(listAlias));
            if (string.IsNullOrWhiteSpace(fieldName)) throw new ArgumentNullException(nameof(fieldName));
            ListAlias = listAlias;
            JoinComparison = new EqJoinComparison(new[]
            {
                new CamlFieldRef
                {
                    List = !string.IsNullOrWhiteSpace(primaryListAlias) ? primaryListAlias : null,
                    Name = fieldName,
                    RefType = "Id"

                },
                new CamlFieldRef
                {
                    List = ListAlias,
                    Name = "ID"

                }
            });
        }
        
        protected Join(string existingElement) : base(JoinTag, existingElement)
        {
        }

        protected Join(XElement existingElement) : base(JoinTag, existingElement)
        {
        }

        protected override void OnParsing(XElement existingElement)
        {
            XAttribute listAlias = existingElement.AttributeIgnoreCase(ListAliasAttr);
            if (listAlias != null)
            {
                ListAlias = listAlias.Value;
            }
            JoinComparison = existingElement.Elements().Select(JoinComparison.GetComparison).FirstOrDefault();
        }

        public override XElement ToXElement()
        {
            XElement el = base.ToXElement();
            if (!string.IsNullOrWhiteSpace(ListAlias))
            {
                el.Add(new XAttribute(ListAliasAttr, ListAlias));
            }
            if (JoinComparison != null)
            {
                el.Add(JoinComparison.ToXElement());
            }
            return el;
        }

        public static Join GetJoin(XElement existingJoin)
        {
            if (existingJoin == null) throw new ArgumentNullException(nameof(existingJoin));
            var tag = existingJoin.Name.LocalName;
            if (string.Equals(tag, JoinTag, StringComparison.OrdinalIgnoreCase))
            {
                XAttribute type = existingJoin.AttributeIgnoreCase(TypeAttr);
                string typeValue = type.Value.Trim();
                if (string.Equals(typeValue, LeftJoin.Left))
                {
                    return new LeftJoin(existingJoin);
                }
                if (string.Equals(typeValue, InnerJoin.Inner))
                {
                    return new InnerJoin(existingJoin);
                }
            }
            throw new NotSupportedException(nameof(tag));
        }
    }
}