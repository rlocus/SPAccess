using SharePoint.Remote.Access.Helpers;
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SP2013Access.ViewModels
{
    public class SPContentTypeViewModel : TreeViewItemViewModel
    {
        private readonly SPClientContentType _contentType;

        public override string ID
        {
            get { return string.Format("ContentType_{0}", _contentType.ContentType.Id); }
        }

        public override string Name
        {
            get
            {
                if (string.IsNullOrEmpty(base.Name))
                {
                    return _contentType.ContentType.Name;
                }
                return base.Name;
            }
        }

        public override ImageSource ImageSource
        {
            get
            {
                return new BitmapImage(new Uri("pack://application:,,,/images/ContentType.png"));
            }
        }

        public SPContentTypeViewModel(SPClientContentType contentType, TreeViewItemViewModel parent)
            : this(parent, false)
        {
            if (contentType == null) throw new ArgumentNullException("contentType");
            _contentType = contentType;
        }

        /// <summary>
        /// Initializes a new instance of the SiteItemViewModel class.
        /// </summary>
        protected SPContentTypeViewModel(TreeViewItemViewModel parent, bool lazyLoadChildren)
            : base(parent, lazyLoadChildren)
        {
        }

        public override void LoadChildren()
        {
            var viewModel = new SPContentTypeFieldCollectionViewModel(_contentType, this);
            this.Children.Add(viewModel);
            base.LoadChildren();
        }

        public override void Refresh()
        { 
            base.Refresh();
            this.IsBusy = true;
            this.IsLoaded = false;
            _contentType.RefreshLoad();
            var promise = Utility.ExecuteAsync(_contentType.LoadAsync());
            promise.Done(() =>
            {
                IsExpanded = true;
                Name = _contentType.ContentType.Name;
            });
            promise.Fail(OnFail);
            promise.Always(() =>
            {
                this.IsBusy = false;
                this.IsLoaded = true;
            });
        }
    }
}