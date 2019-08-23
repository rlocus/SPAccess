using Microsoft.SharePoint.Client;
using SP.Client.Linq.Query;
using System;
using System.Linq;
namespace SP.Client.Linq
{
  public class SpContext : ISpContext
  {
    private ClientContext _context;

    /// <summary>
    /// 
    /// </summary>
    public string SiteUrl { get; private set; }

    public bool ReadOnly { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public ClientContext Context { get { return _context; } }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="siteUrl"></param>
    public SpContext(string siteUrl)
    {
      SiteUrl = siteUrl;
      _context = new ClientContext(siteUrl);
    }

    #region Methods

    public SpEntityQueryable<TListItem> List<TListItem>(string listTitle)
        where TListItem : IListItemEntity
    {
      return new SpEntityQueryable<TListItem>(new SpQueryArgs(_context, listTitle, "", default(Guid)));
    }

    public SpEntityQueryable<TListItem> List<TListItem>(Uri listUrl)
       where TListItem : IListItemEntity
    {
      return new SpEntityQueryable<TListItem>(new SpQueryArgs(_context, null, listUrl.ToString(), default(Guid)));
    }

    public SpEntityQueryable<TListItem> List<TListItem>(Guid listId)
      where TListItem : IListItemEntity
    {
      return new SpEntityQueryable<TListItem>(new SpQueryArgs(_context, null, null, listId));
    }

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

    ~SpContext()
    {
      Dispose(false);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (_context != null)
      {
        try
        {
          _context.Dispose();
        }
        catch { }
        _context = null;
      }
    }

    #endregion
  }
}
