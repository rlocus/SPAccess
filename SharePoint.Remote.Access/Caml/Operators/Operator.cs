using System.Xml.Linq;

namespace SharePoint.Remote.Access.Caml.Operators
{
    public abstract class Operator : QueryElement
    {
        protected Operator(string operatorName)
            : base(operatorName)
        {
        }

        protected Operator(string operatorName, string existingOperator)
            : base(operatorName, existingOperator)
        {
        }

        protected Operator(string operatorName, XElement existingOperator)
            : base(operatorName, existingOperator)
        {
        }

        protected Operator GetOperator(XElement existingOperator)
        {
            switch (existingOperator.Name.LocalName.ToUpper())
            {
                case "AND":
                    return new And(existingOperator);
                case "OR":
                    return new Or(existingOperator);
                case "BEGINSWITH":
                    return new BeginsWith(existingOperator);
                case "CONTAINS":
                    return new Contains(existingOperator);
                case "EQ":
                    return new Eq<object>(existingOperator);
                case "GEQ":
                    return new Geq<object>(existingOperator);
                case "GT":
                    return new Gt<object>(existingOperator);
                case "LEQ":
                    return new Leq<object>(existingOperator);
                case "LT":
                    return new Lt<object>(existingOperator);
                case "NEQ":
                    return new Neq<object>(existingOperator);
                case "ISNULL":
                    return new IsNull(existingOperator);
                case "ISNOTNULL":
                    return new IsNotNull(existingOperator);
                case "DATERANGESOVERLAP":
                    return new DateRangesOverlap(existingOperator);
                case "IN":
                    return new In<object>(existingOperator);
                default:
                    return null;
            }
        }
    }
}