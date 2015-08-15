using Microsoft.SharePoint.Client;
using SharePoint.Remote.Access.Caml;

namespace SharePoint.Remote.Access.Extensions
{
    public static class CamlQueryExtensions
    {
        public static CamlQuery InScope(this CamlQuery camlQuery, ViewScope viewScope, bool hideUnapproved = false)
        {
            switch (viewScope)
            {
                case ViewScope.DefaultValue:
                    //camlQuery.ViewAttributes = hideUnapproved
                    //                             ? "ModerationType=\"HideUnapproved\""
                    //                             : null;
                    break;
                case ViewScope.FilesOnly:
                case ViewScope.Recursive:
                case ViewScope.RecursiveAll:
                    //camlQuery.ViewAttributes = hideUnapproved
                    //                             ? string.Format("Scope=\"{0}\" ModerationType=\"HideUnapproved\"",
                    //                                             viewScope)
                    //                             : string.Format("Scope=\"{0}\"", viewScope);
                    break;
            }

            return camlQuery;
        }

        public static CamlQuery Include(this CamlQuery camlQuery, params string[] viewFields)
        {
            //if (viewFields.Length > 0)
            //{
            //    camlQuery.ViewFieldsOnly = true;

            //    StringBuilder sb = new StringBuilder();

            //    foreach (string viewField in viewFields)
            //    {
            //        sb.Append(new FieldRef() { Name = viewField });
            //    }

            //    camlQuery.ViewFields = sb.ToString();
            //}

            return camlQuery;
        }

        public static CamlQuery Combine(this CamlQuery camlQuery, Query query)
        {
            if (camlQuery != null)
            {
                var existingQuery = Query.GetFromCamlQuery(camlQuery);
                //camlQuery.Query = Query.Combine(existingQuery, query).ToString(false);
            }
            return camlQuery;
        }
    }
}