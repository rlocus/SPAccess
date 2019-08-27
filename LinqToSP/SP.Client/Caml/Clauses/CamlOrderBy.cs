using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using SP.Client.Caml.Interfaces;
using SP.Client.Extensions;

namespace SP.Client.Caml.Clauses
{
  public sealed class CamlOrderBy : CamlClause, ICamlMultiField
  {
    internal const string OrderByTag = "OrderBy";

    public CamlOrderBy() : base(OrderByTag)
    {

    }
    public CamlOrderBy(string fieldName, bool ascending)
        : this(new CamlFieldRef() { Name = fieldName, Ascending = ascending })
    {
    }
    public CamlOrderBy(Guid fieldId, bool ascending)
        : this(new CamlFieldRef() { Id = fieldId, Ascending = ascending })
    {
    }

    public CamlOrderBy(CamlFieldRef fieldRef)
        : this(new[] { fieldRef })
    {
    }

    public CamlOrderBy(IEnumerable<CamlFieldRef> fieldRefs)
        : base(OrderByTag)
    {
      if (fieldRefs == null) throw new ArgumentNullException("fieldRefs");
      FieldRefs = fieldRefs;
    }

    public CamlOrderBy(IEnumerable<string> fieldNames)
        : base(OrderByTag)
    {
      if (fieldNames == null) throw new ArgumentNullException("fieldNames");
      FieldRefs = fieldNames.Select(fieldName => new CamlFieldRef { Name = fieldName });
    }

    public CamlOrderBy(IEnumerable<Guid> fieldIds)
        : base(OrderByTag)
    {
      if (fieldIds == null) throw new ArgumentNullException("fieldIds");
      FieldRefs = fieldIds.Select(fieldId => new CamlFieldRef { Id = fieldId });
    }

    public CamlOrderBy(string existingOrderBy)
        : base(OrderByTag, existingOrderBy)
    {
    }

    public CamlOrderBy(XElement existingOrderBy)
        : base(OrderByTag, existingOrderBy)
    {
    }

    public IEnumerable<CamlFieldRef> FieldRefs { get; private set; }

    public void AddField(string fieldName, bool ascending)
    {
      var fieldRef = new CamlFieldRef() { Name = fieldName, Ascending = ascending };
      if (FieldRefs != null)
      {
        var field = FieldRefs.FirstOrDefault(fRef => fRef.Name == fieldName);
        if (field == null)
        {
          FieldRefs = FieldRefs.Concat(new[] { fieldRef });
        }
        else
        {
          field.Ascending = fieldRef.Ascending;
        }
      }
      else
      {
        FieldRefs = new[] { fieldRef }.AsEnumerable();
      }
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

    protected override void OnParsing(XElement existingOrderBy)
    {
      var existingFieldRefs = existingOrderBy.ElementsIgnoreCase(CamlFieldRef.FieldRefTag);
      FieldRefs = existingFieldRefs.Select(existingFieldRef => new CamlFieldRef(existingFieldRef));
    }

    public static CamlOrderBy Combine(CamlOrderBy firstOrderBy, CamlOrderBy secondOrderBy)
    {
      CamlOrderBy orderBy = null;
      var fieldRefs = new List<CamlFieldRef>();
      if (firstOrderBy != null && firstOrderBy.FieldRefs != null)
      {
        fieldRefs.AddRange(firstOrderBy.FieldRefs);
      }
      if (secondOrderBy != null && secondOrderBy.FieldRefs != null)
      {
        foreach (CamlFieldRef fieldRef in secondOrderBy.FieldRefs)
        {
          CamlFieldRef existingFieldRef =
              fieldRefs.Find(fr => fr.Name == fieldRef.Name || fr.Id == fieldRef.Id);
          if (existingFieldRef != null)
          {
            existingFieldRef.Ascending = fieldRef.Ascending;
          }
          else
          {
            fieldRefs.Add(fieldRef);
          }
        }
        //fieldRefs.AddRange(secondOrderBy.FieldRefs);
      }
      if (fieldRefs.Count > 0)
      {
        orderBy = new CamlOrderBy(fieldRefs);
      }
      return orderBy;
    }

    public static CamlOrderBy operator +(CamlOrderBy firstOrderBy, CamlOrderBy secondOrderBy)
    {
      return Combine(firstOrderBy, secondOrderBy);
    }
  }
}