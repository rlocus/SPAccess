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
            if (list == null) throw new ArgumentNullException(nameof(list));
            _list = list;
        }

        /// <summary>
        ///     Initializes a new instance of the SiteItemViewModel class.
        /// </summary>
        protected SPListViewModel(SPListCollectionViewModel parent, bool lazyLoadChildren)
            : base(parent, lazyLoadChildren)
        {
        }

        public override string ID => $"List_{_list.ClientWeb.Web.Id}_{_list.List.Id}";

        public override string Name
            => string.IsNullOrEmpty(base.Name) ? $"{_list.List.Title} ({_list.List.ItemCount})" : base.Name;

        public override ImageSource ImageSource => new BitmapImage(new Uri("pack://application:,,,/images/ITGEN.png"));

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
                Name = $"{_list.List.Title} ({_list.List.ItemCount})";
                LoadChildren();
            });
            promise.Fail(OnFail);
        }
    }
}