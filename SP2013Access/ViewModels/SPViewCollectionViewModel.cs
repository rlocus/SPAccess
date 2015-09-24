using System;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using SharePoint.Remote.Access.Helpers;

namespace SP2013Access.ViewModels
{
    public class SPViewCollectionViewModel : TreeViewItemViewModel
    {
        private readonly SPClientList _list;

        public SPViewCollectionViewModel(SPClientList list, SPListViewModel parent)
            : this(parent, true)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            _list = list;
        }

        /// <summary>
        ///     Initializes a new instance of the SiteItemViewModel class.
        /// </summary>
        protected SPViewCollectionViewModel(SPListViewModel parent, bool lazyLoadChildren)
            : base(parent, lazyLoadChildren)
        {
        }

        public override string ID => $"ViewCollection_{_list.ClientWeb.Web.Id}_{_list.List.Id}";

        public override ImageSource ImageSource => new BitmapImage(new Uri("pack://application:,,,/images/ITGEN.png"));

        public override string Name => string.IsNullOrEmpty(base.Name) ? "Views" : base.Name;

        protected override IPromise<object, Exception> LoadChildrenAsync()
        {
            var promise = Utility.ExecuteAsync(_list.IncludeViews().LoadAsync());
            promise.Done(() =>
            {
                var views = _list.GetViews();
                Name = $"Views ({views.Length})";
                foreach (var view in views.Where(v => !string.IsNullOrEmpty(v.View.Title)).OrderBy(v => v.View.Title))
                {
                    var v = view;
                    Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                    {
                        var viewModel = new SPViewViewModel(v, this);
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