using SharePoint.Remote.Access.Helpers;
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SP2013Access.ViewModels
{
    public class SPWebContentTypeCollectionViewModel : TreeViewItemViewModel
    {
        private readonly SPClientWeb _web;

        public override string ID
        {
            get { return string.Format("ContentTypeCollection_{0}", _web.Web.Id); }
        }

        public override ImageSource ImageSource
        {
            get
            {
                return new BitmapImage(new Uri("pack://application:,,,/images/ContentType.png"));
            }
        }

        public override string Name
        {
            get
            {
                if (string.IsNullOrEmpty(base.Name))
                {
                    return "Content Types";
                }
                return base.Name;
            }
        }

        public SPWebContentTypeCollectionViewModel(SPClientWeb web, SPWebViewModel parent)
            : this(parent, true)
        {
            if (web == null) throw new ArgumentNullException("web");
            _web = web;
        }

        /// <summary>
        /// Initializes a new instance of the SiteItemViewModel class.
        /// </summary>
        protected SPWebContentTypeCollectionViewModel(SPWebViewModel parent, bool lazyLoadChildren)
            : base(parent, lazyLoadChildren)
        {
        }

        public override void LoadChildren()
        {
            base.LoadChildren();
            
            var promise = Utility.ExecuteAsync(_web.IncludeContentTypes().LoadAsync());

            promise.Done(() =>
            {
                var contentTypes = _web.GetContentTypes();
                Name = string.Format("Content Types ({0})", contentTypes.Length);

                foreach (SPClientContentType contentType in contentTypes)
                {
                    var viewModel = new SPContentTypeViewModel(contentType, this);
                    //viewModel.LoadChildren();
                    this.Children.Add(viewModel);
                }
            });
            promise.Fail((ex) =>
            {
            });
            promise.Always(() =>
            {
                this.IsBusy = false;
                this.IsLoaded = true;
            });

            //if (Parent != null)
            //{
            //    SPClientContentType[] contentTypes = (Parent as SPWebViewModel).ContentTypes;
            //    foreach (SPClientContentType contentType in contentTypes)
            //    {
            //        this.Children.Add(new SPContentTypeViewModel(contentType, this));
            //    }
            //}
        }

        public override void Refresh()
        {
            base.Refresh();

            //if (this.HasDummyChild)
            //{
            //    if (Parent != null)
            //    {
            //        var contentTypes = (Parent as SPWebViewModel).ContentTypes;

            //        if (contentTypes != null)
            //        {
            //            contentTypes.RefreshLoad();

            //            var promise = Utility.ExecuteAsync(_web.Context.ExecuteQueryAsync());

            //            promise.Done(() =>
            //            {
            //                int count = contentTypes.Count();
            //                this.Name = string.Format("Content Types ({0})", count);
            //                if (count == 0)
            //                {
            //                    this.IsExpanded = true;
            //                }
            //            }).Always(() =>
            //            {
            //                this.IsBusy = false;
            //                this.IsLoaded = true;
            //            });
            //        }
            //        else
            //        {
            //            this.IsBusy = false;
            //            this.IsLoaded = true;
            //        }
            //    }
            //    else
            //    {
            //        this.IsBusy = false;
            //        this.IsLoaded = true;
            //    }
            //}
            //else
            //{
            //    this.Children.Clear();

            //    var promise = Utility.ExecuteAsync(_web.LoadContentTypesAsync());

            //    promise.Done((contentTypes) =>
            //    {
            //        int count = contentTypes.Count();
            //        this.Name = string.Format("Content Types ({0})", count);
            //        if (count == 0)
            //        {
            //            this.IsExpanded = true;
            //        }

            //        foreach (ContentType contentType in contentTypes.OrderBy(ct => ct.Name))
            //        {
            //            var viewModel = new SPContentTypeViewModel(contentType, this);
            //            this.Children.Add(viewModel);
            //        }
            //    });

            //    promise.Fail((ex) =>
            //    {
            //    });

            //    promise.Always(() =>
            //    {
            //        this.IsBusy = false;
            //        this.IsLoaded = true;
            //    });
            //}
        }
    }
}