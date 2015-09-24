using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SharePoint.Remote.Access.Helpers;

namespace SP2013Access.ViewModels
{
    public class SPViewViewModel : TreeViewItemViewModel
    {
        private readonly SPClientView _view;

        public SPViewViewModel(SPClientView view, SPViewCollectionViewModel parent)
            : this(parent, false)
        {
            if (view == null) throw new ArgumentNullException(nameof(view));
            _view = view;
        }

        /// <summary>
        ///     Initializes a new instance of the SiteItemViewModel class.
        /// </summary>
        protected SPViewViewModel(SPViewCollectionViewModel parent, bool lazyLoadChildren)
            : base(parent, lazyLoadChildren)
        {
        }

        public override string ID => $"View_{_view.ClientList.List.Id}_{_view.View.Id}";

        public override string Name => string.IsNullOrEmpty(base.Name) ? $"{_view.View.Title}" : base.Name;

        public override ImageSource ImageSource => new BitmapImage(new Uri("pack://application:,,,/images/ITGEN.png"));

        public override void Refresh()
        {
            //if (!IsLoaded) return;
            IsDirty = true;
            IsBusy = true;
            IsLoaded = false;
            _view.RefreshLoad();
            var promise = Utility.ExecuteAsync(_view /*.IncludeItems()*/.LoadAsync());
            promise.Done(() =>
            {
                try
                {
                    Name = $"{_view.View.Title}";
                    //ListItemCollectionPosition pos;
                    //var items = _view.GetItems(out pos);
                    LoadChildren();
                }
                catch (Exception ex)
                {
                    OnFail(ex);
                }
            });
            promise.Fail(OnFail);
        }
    }
}