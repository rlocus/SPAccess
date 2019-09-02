using SP.Client.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace SP.Client.Caml
{
  public sealed class JoinsCamlElement : CamlElement
  {
    internal const string JoinsTag = "Joins";

    public JoinsCamlElement() : base(JoinsTag)
    {
    }

    public JoinsCamlElement(IEnumerable<Join> joins) : base(JoinsTag)
    {
      if (joins != null) Joins = joins;
    }

    public JoinsCamlElement(string existingJoins) : base(JoinsTag, existingJoins)
    {
    }

    public JoinsCamlElement(XElement existingJoins) : base(JoinsTag, existingJoins)
    {
    }

    internal IEnumerable<Join> Joins { get; set; }

    protected override void OnParsing(XElement existingJoins)
    {
      Joins = existingJoins.ElementsIgnoreCase(Join.JoinTag).Select(Join.GetJoin);
    }

    public override XElement ToXElement()
    {
      var el = base.ToXElement();
      if (Joins != null)
      {
        foreach (var join in Joins.Where(join => @join != null))
        {
          el.Add(@join.ToXElement());
        }
      }
      return el;
    }
  }
}