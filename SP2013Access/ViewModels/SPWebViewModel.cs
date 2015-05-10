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

        public SPClientWeb[] SubWebs { get; private set; }

        public SPClientList[] Lists { get; private set; }

        public SPClientContentType[] ContentTypes { get; private set; }

        public SPClientField[] Fields { get; private set; }

        public SPWebViewModel(SPClientWeb web, TreeViewItemViewModel parent)
            : this(parent, true)
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

        public override void LoadChildren()
        {
            base.LoadChildren();

            //_web.RefreshLoad();
            var promise = Utility.ExecuteAsync(_web.LoadAsync());

            promise.Done(() =>
            {
                //_web.RefreshLoad();
                var websViewModel = new SPWebCollectionViewModel(_web, this);
                this.Children.Add(websViewModel);

                var listsViewModel = new SPListCollectionViewModel(_web, this);
                this.Children.Add(listsViewModel);

                var contentTypesViewModel = new SPWebContentTypeCollectionViewModel(_web, this);
                //contentTypesViewModel.LoadChildren();
                this.Children.Add(contentTypesViewModel);

                var fieldsViewModel = new SPSiteFieldCollectionViewModel(_web, this);
                //fieldsViewModel.LoadChildren();
                this.Children.Add(fieldsViewModel);
            });

            promise.Fail((ex) =>
            {
            });

            promise.Always(() =>
            {
                this.IsBusy = false;
                this.IsLoaded = true;
            });

            //var ctx = this.Web.Web.Context;
            //var newCtx = new ClientContext(ctx.Url)
            //{
            //    AuthenticationMode = ctx.AuthenticationMode,
            //    Credentials = ctx.Credentials
            //};

            //var w = newCtx.Site.OpenWebById(_web.Web.Id);

            //newCtx.Load(w);

            //_web = SPClientWeb.FromWeb(w);

            //var promise = Utility.ExecuteAsync(_web.IncludeWebs().IncludeLists().IncludeContentTypes().IncludeFields().LoadAsync());

            //promise.Done(() =>
            //{
            //    SubWebs = _web.GetWebs();
            //    Lists = _web.GetLists();
            //    ContentTypes = _web.GetContentTypes();
            //    Fields = _web.GetFields();

            //    var websViewModel = new SPWebCollectionViewModel(_web, this)
            //    {
            //        Name = string.Format("Webs ({0})", SubWebs.Length)
            //    };

            //    websViewModel.LoadChildren();

            //    if (SubWebs.Length == 0)
            //    {
            //        websViewModel.IsExpanded = true;
            //    }

            //    this.Children.Add(websViewModel);

            //    var listsViewModel = new SPListCollectionViewModel(_web, this)
            //    {
            //        Name = string.Format("Lists ({0})", Lists.Length)
            //    };

            //    listsViewModel.LoadChildren();

            //    if (Lists.Length == 0)
            //    {
            //        listsViewModel.IsExpanded = true;
            //    }

            //    this.Children.Add(listsViewModel);

            //    var contentTypesViewModel = new SPWebContentTypeCollectionViewModel(_web, this)
            //    {
            //        Name = string.Format("Content Types ({0})", ContentTypes.Length)
            //    };

            //    contentTypesViewModel.LoadChildren();

            //    if (ContentTypes.Length == 0)
            //    {
            //        contentTypesViewModel.IsExpanded = true;
            //    }

            //    this.Children.Add(contentTypesViewModel);

            //    var fieldsViewModel = new SPSiteFieldCollectionViewModel(_web, this)
            //    {
            //        Name = string.Format("Site Fields ({0})", Fields.Length)
            //    };

            //    fieldsViewModel.LoadChildren();

            //    if (Fields.Length == 0)
            //    {
            //        fieldsViewModel.IsExpanded = true;
            //    }

            //    this.Children.Add(fieldsViewModel);
            //});

            //promise.Fail((ex) =>
            //{
            //});

            //promise.Always(() =>
            //{
            //    this.IsBusy = false;
            //    this.IsLoaded = true;
            //});
        }

        public override void Refresh()
        {
            _web.RefreshLoad();
            base.Refresh();

            //var context = _web.Context;
            //_web.RefreshLoad();
            //context.Load(_web);

            //if (SubWebs != null)
            //{
            //    SubWebs.RefreshLoad();
            //    context.Load(SubWebs);
            //}

            //if (Lists != null)
            //{
            //    Lists.RefreshLoad();
            //    context.Load(Lists);
            //}

            //if (ContentTypes != null)
            //{
            //    ContentTypes.RefreshLoad();
            //    context.Load(ContentTypes);
            //}

            //if (Fields != null)
            //{
            //    Fields.RefreshLoad();
            //    context.Load(Fields);
            //}

            //var promise = Utility.ExecuteAsync(_web.Context.ExecuteQueryAsync());

            //promise.Done(() =>
            //{
            //});

            //promise.Fail((ex) =>
            //{
            //});

            //promise.Always(() =>
            //{
            //    this.IsBusy = false;
            //    this.IsLoaded = true;
            //});
        }
    }
}