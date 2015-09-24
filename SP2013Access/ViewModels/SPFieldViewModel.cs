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
            if (field == null) throw new ArgumentNullException(nameof(field));
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
            => _field.IsSiteField
                    ? $"SiteField_{_field.ClientWeb.Web.Id}_{_field.Field.Id}"
                    : $"Field_{_field.ClientWeb.Web.Id}_{_field.ClientList.List.Id}_{_field.Field.Id}";

        public override string Name => string.IsNullOrEmpty(base.Name) ? $"{_field.Field.Title} ({_field.Field.InternalName})" : base.Name;

        public override ImageSource ImageSource
            => new BitmapImage(new Uri("pack://application:,,,/images/SiteColumn.png"));

        public override void Refresh()
        {
            IsDirty = true;
            IsBusy = true;
            IsLoaded = false;
            _field.RefreshLoad();
            var promise = Utility.ExecuteAsync(_field.LoadAsync());
            promise.Done(() =>
            {
                Name = $"{_field.Field.Title} ({_field.Field.InternalName})";
                LoadChildren();
            });
            promise.Fail(OnFail);
        }
    }
}