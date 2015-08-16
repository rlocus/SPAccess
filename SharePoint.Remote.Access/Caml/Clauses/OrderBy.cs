using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using SharePoint.Remote.Access.Caml.Interfaces;
using SharePoint.Remote.Access.Extensions;

namespace SharePoint.Remote.Access.Caml.Clauses
{
    public sealed class OrderBy : Clause, IMultiFieldOperator
    {
        internal const string OrderByTag = "OrderBy";

        public IEnumerable<FieldRef> FieldRefs { get; private set; }

        public OrderBy(IEnumerable<FieldRef> fieldRefs)
            : base(OrderByTag)
        {
            FieldRefs = fieldRefs;
        }

        public OrderBy(string existingGroupBy)
            : base(OrderByTag, existingGroupBy)
        {
        }

        public OrderBy(XElement existingOrderBy)
            : base(OrderByTag, existingOrderBy)
        {
        }

        //public OrderBy(Guid fieldId, bool? ascending = null)
        //    : base(OrderByTag)
        //{
        //    FieldRefs = (new[] { new FieldRef { FieldId = fieldId, Ascending = ascending } }).AsEnumerable();
        //}

        //public OrderBy(string fieldName, bool? ascending = null)
        //    : base(OrderByTag)
        //{
        //    FieldRefs = (new[] { new FieldRef { Name = fieldName, Ascending = ascending } }).AsEnumerable();
        //}

        public OrderBy(FieldRef field)
        : base(OrderByTag)
        {
            FieldRefs = new[] { field }.AsEnumerable();
        }

        protected override void OnParsing(XElement existingOrderBy)
        {
            var existingFieldRefs = existingOrderBy.ElementsIgnoreCase(FieldRef.FieldRefTag);
            FieldRefs = existingFieldRefs.Select(existingFieldRef => new FieldRef(existingFieldRef));
        }

        public override XElement ToXElement()
        {
            var el = base.ToXElement();
            if (FieldRefs != null)
            {
                foreach (var fieldRef in FieldRefs.Where(fieldRef => fieldRef != null))
                {
                    el.Add(fieldRef.ToXElement());
                }
            }
            return el;
        }

        public static OrderBy Combine(OrderBy firstOrderBy, OrderBy secondOrderBy)
        {
            OrderBy orderBy = null;
            var fieldRefs = new List<FieldRef>();
            if (firstOrderBy?.FieldRefs != null)
            {
                fieldRefs.AddRange(firstOrderBy.FieldRefs);
            }
            if (secondOrderBy?.FieldRefs != null)
            {
                fieldRefs.AddRange(secondOrderBy.FieldRefs);
            }
            if (fieldRefs.Count > 0)
            {
                orderBy = new OrderBy(fieldRefs);
            }
            return orderBy;
        }

        public static OrderBy operator +(OrderBy firstOrderBy, OrderBy secondOrderBy)
        {
            return Combine(firstOrderBy, secondOrderBy);
        }
    }
}