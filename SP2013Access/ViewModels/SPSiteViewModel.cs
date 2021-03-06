﻿using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SharePoint.Remote.Access.Helpers;

namespace SP2013Access.ViewModels
{
    public class SPSiteViewModel : TreeViewItemViewModel
    {
        private readonly SPClientSite _site;

        public SPSiteViewModel(SPClientSite site, TreeViewItemViewModel parent)
            : this(parent, true)
        {
            if (site == null) throw new ArgumentNullException(nameof(site));
            _site = site;
        }

        /// <summary>
        ///     Initializes a new instance of the SiteItemViewModel class.
        /// </summary>
        protected SPSiteViewModel(TreeViewItemViewModel parent, bool lazyLoadChildren)
            : base(parent, lazyLoadChildren)
        {
        }

        public override string ID => $"Site_{_site.Site.Id}";

        public override string Name => _site.Site.Url;

        public override ImageSource ImageSource => new BitmapImage(new Uri("pack://application:,,,/images/sharepointfoundation16.png"));

        protected override IPromise<object, Exception> LoadChildrenAsync()
        {
            var promise = Utility.ExecuteAsync(_site.IncludeRootWeb().LoadAsync());
            promise.Done(() =>
            {
                var rootWeb = _site.GetRootWeb();
                var viewModel = new SPWebViewModel(rootWeb, this) {IsExpanded = true};
                Children.Add(viewModel);
            });
            promise.Fail(OnFail);
            return promise;
        }

        public override void Refresh()
        {
            base.Refresh();
            IsExpanded = true;
        }
    }
}