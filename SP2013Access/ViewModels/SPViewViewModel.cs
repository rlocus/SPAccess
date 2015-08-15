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
            if (view == null) throw new ArgumentNullException("view");
            _view = view;
        }

        /// <summary>
        ///     Initializes a new instance of the SiteItemViewModel class.
        /// </summary>
        protected SPViewViewModel(SPViewCollectionViewModel parent, bool lazyLoadChildren)
            : base(parent, lazyLoadChildren)
        {
        }

        public override string ID
        {
            get { return string.Format("View_{0}_{1}", _view.ClientList.List.Id, _view.View.Id); }
        }

        public override string Name
        {
            get
            {
                if (string.IsNullOrEmpty(base.Name))
                {
                    return string.Format("{0}", _view.View.Title);
                }
                return base.Name;
            }
        }

        public override ImageSource ImageSource
        {
            get { return new BitmapImage(new Uri("pack://application:,,,/images/ITGEN.png")); }
        }

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
                    Name = string.Format("{0}", _view.View.Title);
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