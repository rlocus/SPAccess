using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SharePoint.Remote.Access.Helpers;

namespace SP2013Access.ViewModels
{
    public class SPWebViewModel : TreeViewItemViewModel
    {
        public SPWebViewModel(SPClientWeb web, TreeViewItemViewModel parent)
            : this(parent, false)
        {
            if (web == null) throw new ArgumentNullException(nameof(web));
            Web = web;
        }

        /// <summary>
        ///     Initializes a new instance of the SiteItemViewModel class.
        /// </summary>
        protected SPWebViewModel(TreeViewItemViewModel parent, bool lazyLoadChildren)
            : base(parent, lazyLoadChildren)
        {
        }

        public override string ID => $"Web_{Web.Web.Id}";

        public override string Name => Web.Web.Title;

        public override ImageSource ImageSource => new BitmapImage(new Uri("pack://application:,,,/images/siteicon_16x16.png"));

        public SPClientWeb Web { get; }

        protected override void LoadChildren()
        {
            if (IsLoaded) return;
            var websViewModel = new SPWebCollectionViewModel(Web, this);
            Children.Add(websViewModel);
            var listsViewModel = new SPListCollectionViewModel(Web, this);
            Children.Add(listsViewModel);
            var contentTypesViewModel = new SPWebContentTypeCollectionViewModel(Web, this);
            Children.Add(contentTypesViewModel);
            var fieldsViewModel = new SPSiteFieldCollectionViewModel(Web, this);
            Children.Add(fieldsViewModel);
            base.LoadChildren();
        }

        public override void Refresh()
        {
            if (!IsLoaded) return;
            IsDirty = true;
            IsBusy = true;
            IsLoaded = false;
            Web.RefreshLoad();
            var promise = Utility.ExecuteAsync(Web.LoadAsync());
            promise.Done(() =>
            {
                Name = Web.Web.Title;
                LoadChildren();
            });
            promise.Fail(OnFail);
        }
    }
}