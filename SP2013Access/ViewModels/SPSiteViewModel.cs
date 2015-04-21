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
            get { return string.Format("Site_{0}", _site.Id); }
        }

        public override string Name
        {
            get { return _site.Url; }
        }

        public override ImageSource ImageSource
        {
            get
            {
                return new BitmapImage(new Uri("pack://application:,,,/images/sharepointfoundation16.png"));
            }
        }

        public SPSiteViewModel(SPClientSite site)
            : this(null, false)
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

        public override void LoadChildren()
        {
            base.LoadChildren();

            var promise = Utility.ExecuteAsync(_site.IncludeRootWeb().LoadAsync());

            promise.Done(() =>
            {
                var rootWeb = _site.GetRootWeb();
                var viewModel = new SPWebViewModel(rootWeb, this);
                //viewModel.LoadChildren();
                this.Children.Add(viewModel);
            });

            promise.Fail((ex) =>
            {
            });

            promise.Always(() =>
            {
                this.IsBusy = false;
                this.IsLoaded = true;
            });
        }

        public override void Refresh()
        {
            base.Refresh();

            //Deferred.When(Utility.ExecuteAsync((_site.Context as SPClientContext).ConnectAsync())).Done(() =>
            //{
            //    var promise = Utility.ExecuteAsync(_site.LoadRootWebAsync());

            //    promise.Done((rootWeb) => this.Children.Add(new SPWebViewModel(rootWeb, this)));

            //    promise.Fail((ex) =>
            //    {
            //    });

            //    promise.Always(() =>
            //    {
            //        this.IsBusy = false;
            //        this.IsLoaded = true;
            //    });

            //}).Always(() =>
            //{
            //    this.IsBusy = false;
            //    this.IsLoaded = false;
            //});
        }
    }
}