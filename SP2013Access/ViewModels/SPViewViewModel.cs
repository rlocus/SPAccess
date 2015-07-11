using Microsoft.SharePoint.Client;
using SharePoint.Remote.Access.Helpers;
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SP2013Access.ViewModels
{
    public class SPViewViewModel : TreeViewItemViewModel
    {
        private readonly SPClientView _view;

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
            get
            {
                return new BitmapImage(new Uri("pack://application:,,,/images/ITGEN.png"));
            }
        }

        public SPViewViewModel(SPClientView view, SPViewCollectionViewModel parent)
            : this(parent, false)
        {
            if (view == null) throw new ArgumentNullException("view");
            _view = view;
        }

        /// <summary>
        /// Initializes a new instance of the SiteItemViewModel class.
        /// </summary>
        protected SPViewViewModel(SPViewCollectionViewModel parent, bool lazyLoadChildren)
            : base(parent, lazyLoadChildren)
        {
        }


        public override void Refresh()
        {
            //if (!IsLoaded) return;
            this.IsDirty = true;
            this.IsBusy = true;
            this.IsLoaded = false;
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