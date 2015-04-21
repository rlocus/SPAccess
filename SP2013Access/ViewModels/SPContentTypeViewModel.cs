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

        public SPClientField[] Fields { get; private set; }

        public override ImageSource ImageSource
        {
            get
            {
                return new BitmapImage(new Uri("pack://application:,,,/images/ContentType.png"));
            }
        }

        public SPContentTypeViewModel(SPClientContentType contentType, TreeViewItemViewModel parent)
            : this(parent, true)
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
            base.LoadChildren();
            var promise = Utility.ExecuteAsync(_contentType.IncludeFields().LoadAsync());

            promise.Done(() =>
            {
                Fields = _contentType.GetFields();
                var viewModel = new SPContentTypeFieldCollectionViewModel(_contentType, this)
                {
                    Name = string.Format("Fields ({0})", Fields.Length)
                };
                viewModel.LoadChildren();
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
            _contentType.RefreshLoad();
            base.Refresh();
        }
    }
}