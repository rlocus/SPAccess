using System;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using SharePoint.Remote.Access.Helpers;

namespace SP2013Access.ViewModels
{
    public class SPFieldCollectionViewModel : TreeViewItemViewModel
    {
        private readonly SPClientList _list;

        public SPFieldCollectionViewModel(SPClientList list, SPListViewModel parent)
            : this(parent, true)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            _list = list;
        }

        /// <summary>
        ///     Initializes a new instance of the SiteItemViewModel class.
        /// </summary>
        protected SPFieldCollectionViewModel(SPListViewModel parent, bool lazyLoadChildren)
            : base(parent, lazyLoadChildren)
        {
        }

        public override string ID => $"FieldCollection_{_list.ClientWeb.Web.Id}_{_list.List.Id}";

        public override ImageSource ImageSource => new BitmapImage(new Uri("pack://application:,,,/images/SiteColumn.png"));

        public override string Name => string.IsNullOrEmpty(base.Name) ? "Fields" : base.Name;

        protected override IPromise<object, Exception> LoadChildrenAsync()
        {
            var promise = Utility.ExecuteAsync(_list.IncludeFields().LoadAsync());
            promise.Done(() =>
            {
                var fields = _list.GetFields();
                Name = $"Fields ({fields.Length})";
                foreach (var field in fields.OrderBy(f => f.Field.Title))
                {
                    var f = field;
                    Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                    {
                        var viewModel = new SPFieldViewModel(f, this) {IsExpanded = true};
                        Children.Add(viewModel);
                    }));
                }
            });
            return promise;
        }

        public override void Refresh()
        {
            base.Refresh();
            IsExpanded = true;
        }
    }
}