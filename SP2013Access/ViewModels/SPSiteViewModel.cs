using SharePoint.Remote.Access.Helpers;
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SP2013Access.ViewModels
{
    public class SPSiteViewModel : TreeViewItemViewModel
    {
        private readonly SPClientSite _site;

        public override string ID
        {
            get { return string.Format("Site_{0}", _site.Site.Id); }
        }

        public override string Name
        {
            get { return _site.Site.Url; }
        }

        public override ImageSource ImageSource
        {
            get
            {
                return new BitmapImage(new Uri("pack://application:,,,/images/sharepointfoundation16.png"));
            }
        }

        public SPSiteViewModel(SPClientSite site, TreeViewItemViewModel parent)
            : this(parent, true)
        {
            if (site == null) throw new ArgumentNullException("site");
            _site = site;
        }

        /// <summary>
        /// Initializes a new instance of the SiteItemViewModel class.
        /// </summary>
        protected SPSiteViewModel(TreeViewItemViewModel parent, bool lazyLoadChildren)
            : base(parent, lazyLoadChildren)
        {
        }

        protected override IPromise<object, Exception> LoadChildrenAsync()
        {
            var promise = Utility.ExecuteAsync(_site.IncludeRootWeb().LoadAsync());
            promise.Done(() =>
            {
                var rootWeb = _site.GetRootWeb();
                var viewModel = new SPWebViewModel(rootWeb, this) {IsExpanded = true};
                this.Children.Add(viewModel);
            });
            promise.Fail(OnFail);
            return promise;
        }

        public override void Refresh()
        {
            base.Refresh();
            this.IsExpanded = true;
        }
    }
}