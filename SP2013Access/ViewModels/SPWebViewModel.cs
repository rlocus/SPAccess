using SharePoint.Remote.Access.Helpers;
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SP2013Access.ViewModels
{
    public class SPWebViewModel : TreeViewItemViewModel
    {
        private readonly SPClientWeb _web;

        public override string ID
        {
            get { return string.Format("Web_{0}", _web.Web.Id); }
        }

        public override string Name
        {
            get { return _web.Web.Title; }
        }

        public override ImageSource ImageSource
        {
            get
            {
                return new BitmapImage(new Uri("pack://application:,,,/images/siteicon_16x16.png"));
            }
        }

        public SPClientWeb Web
        {
            get { return _web; }
        }

        public SPWebViewModel(SPClientWeb web, TreeViewItemViewModel parent)
            : this(parent, false)
        {
            if (web == null) throw new ArgumentNullException("web");
            _web = web;
        }

        /// <summary>
        /// Initializes a new instance of the SiteItemViewModel class.
        /// </summary>
        protected SPWebViewModel(TreeViewItemViewModel parent, bool lazyLoadChildren)
            : base(parent, lazyLoadChildren)
        {
        }

        protected override void LoadChildren()
        {
            if (IsLoaded) return;
            var websViewModel = new SPWebCollectionViewModel(_web, this);
            this.Children.Add(websViewModel);
            var listsViewModel = new SPListCollectionViewModel(_web, this);
            this.Children.Add(listsViewModel);
            var contentTypesViewModel = new SPWebContentTypeCollectionViewModel(_web, this);
            this.Children.Add(contentTypesViewModel);
            var fieldsViewModel = new SPSiteFieldCollectionViewModel(_web, this);
            this.Children.Add(fieldsViewModel);
            base.LoadChildren();
        }

        public override void Refresh()
        {
            if (!IsLoaded) return;
            this.IsDirty = true;
            this.IsBusy = true;
            this.IsLoaded = false;
            _web.RefreshLoad();
            var promise = Utility.ExecuteAsync(_web.LoadAsync());
            promise.Done(() =>
            {
                Name = _web.Web.Title;
                this.LoadChildren();
            });
            promise.Fail(OnFail);
        }
    }
}