using SharePoint.Remote.Access.Extensions;
using SharePoint.Remote.Access.Helpers;
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SP2013Access.ViewModels
{
    public class SPFieldViewModel : TreeViewItemViewModel
    {
        private readonly SPClientField _field;

        public override string ID
        {
            get
            {
                if (_field.IsSiteField)
                {
                    return string.Format("SiteField_{0}_{1}", _field.ClientWeb.Id, _field.Id);
                }
                return string.Format("Field_{0}_{1}_{2}", _field.ClientWeb.Id, _field.ClientList.Id, _field.Id);
            }
        }

        public override string Name
        {
            get { return string.Format("{0} ({1})", _field.Title, _field.InternalName); }
        }

        public override ImageSource ImageSource
        {
            get
            {
                return new BitmapImage(new Uri("pack://application:,,,/images/SiteColumn.png"));
            }
        }

        public SPFieldViewModel(SPClientField field, TreeViewItemViewModel parent)
            : this(parent, false)
        {
            if (field == null) throw new ArgumentNullException("field");
            _field = field;
        }

        /// <summary>
        /// Initializes a new instance of the SiteItemViewModel class.
        /// </summary>
        protected SPFieldViewModel(TreeViewItemViewModel parent, bool lazyLoadChildren)
            : base(parent, lazyLoadChildren)
        {
        }

        public override void LoadChildren()
        {
            base.LoadChildren();
            this.IsBusy = false;
            this.IsLoaded = true;
            this.IsExpanded = true;
        }

        public override void Refresh()
        {
            base.Refresh();

            _field.RefreshLoad();

            var promise = Utility.ExecuteAsync(_field.Context.ExecuteQueryAsync());

            promise.Done(() =>
            {
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
    }
}