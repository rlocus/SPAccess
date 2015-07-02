using Microsoft.SharePoint.Client;
using SharePoint.Remote.Access.Extensions;
using SharePoint.Remote.Access.Helpers;
using System;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SP2013Access.ViewModels
{
    public class SPListContentTypeCollectionViewModel : TreeViewItemViewModel
    {
        private readonly SPClientList _list;

        public override string ID
        {
            get { return string.Format("ContentTypeCollection_{0}", _list.List.Id); }
        }

        public override ImageSource ImageSource
        {
            get
            {
                return new BitmapImage(new Uri("pack://application:,,,/images/ContentType.png"));
            }
        }

        public SPListContentTypeCollectionViewModel(SPClientList list, SPListViewModel parent)
            : this(parent, true)
        {
            if (list == null) throw new ArgumentNullException("list");
            _list = list;
        }

        /// <summary>
        /// Initializes a new instance of the SiteItemViewModel class.
        /// </summary>
        protected SPListContentTypeCollectionViewModel(SPListViewModel parent, bool lazyLoadChildren)
            : base(parent, lazyLoadChildren)
        {
        }

        public override void LoadChildren()
        {
            base.LoadChildren();

            //if (Parent != null)
            //{
            //    SPClientContentType[] contentTypes = (Parent as SPListViewModel).ContentTypes;
            //    foreach (SPClientContentType contentType in contentTypes)
            //    {
            //        this.Children.Add(new SPContentTypeViewModel(contentType, this));
            //    }
            //}

            var promise = Utility.ExecuteAsync(_list.IncludeContentTypes().LoadAsync());

            promise.Done(() =>
            {
                var contentTypes = _list.GetContentTypes();
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
        }

        public override void Refresh()
        {
            base.Refresh();

            //if (this.HasDummyChild)
            //{
            //    this.IsBusy = false;
            //    this.IsLoaded = true;
            //    return;
            //}

            //if (this.HasDummyChild)
            //{
            //    if (Parent != null)
            //    {
            //        var contentTypes = (Parent as SPListViewModel).ContentTypes;

            //        if (contentTypes != null)
            //        {
            //            contentTypes.RefreshLoad();

            //            var promise = Utility.ExecuteAsync(_list.Context.ExecuteQueryAsync());

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

            //    var promise = Utility.ExecuteAsync(_list.LoadContentTypesAsync());

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