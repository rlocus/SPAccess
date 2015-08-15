using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SharePoint.Remote.Access.Helpers;

namespace SP2013Access.ViewModels
{
    public class SPFieldViewModel : TreeViewItemViewModel
    {
        private readonly SPClientField _field;

        public SPFieldViewModel(SPClientField field, TreeViewItemViewModel parent)
            : this(parent, false)
        {
            if (field == null) throw new ArgumentNullException("field");
            _field = field;
        }

        /// <summary>
        ///     Initializes a new instance of the SiteItemViewModel class.
        /// </summary>
        protected SPFieldViewModel(TreeViewItemViewModel parent, bool lazyLoadChildren)
            : base(parent, lazyLoadChildren)
        {
        }

        public override string ID
        {
            get
            {
                if (_field.IsSiteField)
                {
                    return string.Format("SiteField_{0}_{1}", _field.ClientWeb.Web.Id, _field.Field.Id);
                }
                return string.Format("Field_{0}_{1}_{2}", _field.ClientWeb.Web.Id, _field.ClientList.List.Id,
                    _field.Field.Id);
            }
        }

        public override string Name
        {
            get
            {
                if (string.IsNullOrEmpty(base.Name))
                {
                    return string.Format("{0} ({1})", _field.Field.Title, _field.Field.InternalName);
                }
                return base.Name;
            }
        }

        public override ImageSource ImageSource
        {
            get { return new BitmapImage(new Uri("pack://application:,,,/images/SiteColumn.png")); }
        }

        public override void Refresh()
        {
            //if (!IsLoaded) return;
            IsDirty = true;
            IsBusy = true;
            IsLoaded = false;
            _field.RefreshLoad();
            var promise = Utility.ExecuteAsync(_field.LoadAsync());
            promise.Done(() =>
            {
                Name = string.Format("{0} ({1})", _field.Field.Title, _field.Field.InternalName);
                LoadChildren();
            });
            promise.Fail(OnFail);
        }
    }
}