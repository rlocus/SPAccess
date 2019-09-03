using Microsoft.SharePoint.Client;
using SP.Client.Linq.Query;
using System;
using System.Linq;

namespace SP.Client.Linq
{
    /// <summary>
    /// SharePoint context
    /// IDisposable
    /// ******************************************************************************************************************
    /// Usage: var spContext = new SpDataContext("https://sp-site")
    /// spContext.Context.Credentials = new SharePointOnlineCredentials("user@domain", ConvertToSecureString("password"));
    /// ******************************************************************************************************************
    /// Examples:
    /// var items = spContext.List<Email>("Emails")
    ///        .Where(i => (i.Title.Contains("Test") || i.Title.StartsWith("Test")) &&
    ///                    (i.Includes(x => x.Account, 1, 2)) &&
    ///                    (i.LookupIdIncludes(x => x.Contact, 1)) &&
    ///                    (i.Created > DateTime.Today || (i.Id > 1 && i.Id < 100)) ||                           
    ///                    (i.Account == null && i.Contact != null) &&
    ///                     i.IsMembership(x => x.AssignedTo, SP.Client.Caml.Operators.MembershipType.AllUsers))
    ///        .Include(i => i.Id, i=> i.Title).GroupBy<Email>(i => i.Title).OrderBy(i => i.Id);
    /// --------------------------------------------------------------------------------------------------------       
    /// var events = spContext.List<Event>("Calendar")
    ///                    .Where(e => e.StartTime < DateTime.Today.AddMonths(-1) &&
    ///                                e.DateRangesOverlap(x => x.StartTime, x => x.EndTime, x => x.RecurrenceId, CamlValue.Month));
    ///</summary>
    public class SpDataContext : ISpDataContext
    {
        #region Properties
        /// <summary>
        /// Site Url.
        /// </summary>
        public string SiteUrl { get; private set; }

        /// <summary>
        /// CSOM context
        /// </summary>
        public ClientContext Context { get; private set; }

        #endregion

        #region Constructor
        /// <summary>
        /// SharePoint context
        /// </summary>
        /// <param name="siteUrl">Site Url: https://sp-site
        /// </param>
        public SpDataContext(string siteUrl)
        {
            SiteUrl = siteUrl;
            Context = new ClientContext(siteUrl);
        }

        #endregion

        #region Methods

        /// <summary>
        /// SP List
        /// </summary>
        /// <typeparam name="TListItem"></typeparam>
        /// <param name="listTitle">List title</param>
        /// <returns></returns>
        public SpEntityQueryable<TListItem> List<TListItem>(string listTitle, string query = null)
            where TListItem : IListItemEntity
        {
            return new SpEntityQueryable<TListItem>(new SpQueryArgs(Context, listTitle, "", default(Guid), query));
        }

        /// <summary>
        /// SP List
        /// </summary>
        /// <typeparam name="TListItem"></typeparam>
        /// <param name="listUrl">List url</param>
        /// <returns></returns>
        public SpEntityQueryable<TListItem> List<TListItem>(Uri listUrl, string query = null)
           where TListItem : IListItemEntity
        {
            return new SpEntityQueryable<TListItem>(new SpQueryArgs(Context, null, listUrl.ToString(), default, query));
        }

        /// <summary>
        /// SP List
        /// </summary>
        /// <typeparam name="TListItem"></typeparam>
        /// <param name="listId">List id</param>
        /// <returns></returns>
        public SpEntityQueryable<TListItem> List<TListItem>(Guid listId, string query = null)
          where TListItem : IListItemEntity
        {
            return new SpEntityQueryable<TListItem>(new SpQueryArgs(Context, null, null, listId, query));
        }

        /// <summary>
        /// SP Query (Caml)
        /// </summary>
        /// <typeparam name="TListItem"></typeparam>
        /// <param name="items">Linq query</param>
        /// <param name="disableFormatting">Disable formatting</param>
        /// <returns></returns>
        public virtual string GenerateQuery<TListItem>(IQueryable<TListItem> items, bool disableFormatting = false)
           where TListItem : IListItemEntity
        {
            if (items is SpEntityQueryable<TListItem>)
            {
                return GenerateQuery(items as SpEntityQueryable<TListItem>, disableFormatting);
            }
            return null;
        }
        protected virtual string GenerateQuery<TListItem>(SpEntityQueryable<TListItem> items, bool disableFormatting = false)
            where TListItem : IListItemEntity
        {
            return items.GetQueryInternal(disableFormatting);
        }

        #endregion

        #region IDisposable Methods

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~SpDataContext()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (Context != null)
            {
                try
                {
                    Context.Dispose();
                }
                catch { }
                Context = null;
            }
        }

        #endregion
    }
}
