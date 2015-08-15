using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SharePoint.Remote.Access.Helpers;

namespace SP2013Access.ViewModels
{
    public class SPListViewModel : TreeViewItemViewModel
    {
        private readonly SPClientList _list;

        public SPListViewModel(SPClientList list, SPListCollectionViewModel parent)
            : this(parent, false)
        {
            if (list == null) throw new ArgumentNullException("list");
            _list = list;
        }

        /// <summary>
        ///     Initializes a new instance of the SiteItemViewModel class.
        /// </summary>
        protected SPListViewModel(SPListCollectionViewModel parent, bool lazyLoadChildren)
            : base(parent, lazyLoadChildren)
        {
        }

        public override string ID
        {
            get { return string.Format("List_{0}_{1}", _list.ClientWeb.Web.Id, _list.List.Id); }
        }

        public override string Name
        {
            get
            {
                if (string.IsNullOrEmpty(base.Name))
                {
                    return string.Format("{0} ({1})", _list.List.Title, _list.List.ItemCount);
                }
                return base.Name;
            }
        }

        public override ImageSource ImageSource
        {
            get { return new BitmapImage(new Uri("pack://application:,,,/images/ITGEN.png")); }
        }

        protected override void LoadChildren()
        {
            if (IsLoaded) return;
            var contentTypesViewModel = new SPListContentTypeCollectionViewModel(_list, this);
            Children.Add(contentTypesViewModel);
            var fieldsViewModel = new SPFieldCollectionViewModel(_list, this);
            Children.Add(fieldsViewModel);
            var viewsViewModel = new SPViewCollectionViewModel(_list, this);
            Children.Add(viewsViewModel);
            base.LoadChildren();
        }

        public override void Refresh()
        {
            if (!IsLoaded) return;
            IsDirty = true;
            IsBusy = true;
            IsLoaded = false;
            _list.RefreshLoad();
            var promise = Utility.ExecuteAsync(_list.LoadAsync());
            promise.Done(() =>
            {
                Name = string.Format("{0} ({1})", _list.List.Title, _list.List.ItemCount);
                LoadChildren();
            });
            promise.Fail(OnFail);
        }
    }
}