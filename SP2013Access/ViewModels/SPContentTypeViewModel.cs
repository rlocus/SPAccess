using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SharePoint.Remote.Access.Helpers;

namespace SP2013Access.ViewModels
{
    public class SPContentTypeViewModel : TreeViewItemViewModel
    {
        private readonly SPClientContentType _contentType;

        public SPContentTypeViewModel(SPClientContentType contentType, TreeViewItemViewModel parent)
            : this(parent, false)
        {
            if (contentType == null) throw new ArgumentNullException(nameof(contentType));
            _contentType = contentType;
        }

        /// <summary>
        ///     Initializes a new instance of the SiteItemViewModel class.
        /// </summary>
        protected SPContentTypeViewModel(TreeViewItemViewModel parent, bool lazyLoadChildren)
            : base(parent, lazyLoadChildren)
        {
        }

        public override string ID => $"ContentType_{_contentType.ContentType.Id}";

        public override string Name => string.IsNullOrEmpty(base.Name) ? _contentType.ContentType.Name : base.Name;

        public override ImageSource ImageSource => new BitmapImage(new Uri("pack://application:,,,/images/ContentType.png"));

        protected override void LoadChildren()
        {
            if (IsLoaded) return;
            var viewModel = new SPContentTypeFieldCollectionViewModel(_contentType, this);
            Children.Add(viewModel);
            base.LoadChildren();
        }

        public override void Refresh()
        {
            if (!IsLoaded) return;
            IsDirty = true;
            IsBusy = true;
            IsLoaded = false;
            _contentType.RefreshLoad();
            var promise = Utility.ExecuteAsync(_contentType.LoadAsync());
            promise.Done(() =>
            {
                Name = _contentType.ContentType.Name;
                LoadChildren();
            });
            promise.Fail(OnFail);
        }
    }
}