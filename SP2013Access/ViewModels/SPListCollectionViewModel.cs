using System;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using SharePoint.Remote.Access.Helpers;

namespace SP2013Access.ViewModels
{
    public class SPListCollectionViewModel : TreeViewItemViewModel
    {
        private readonly SPClientWeb _web;

        public SPListCollectionViewModel(SPClientWeb web, SPWebViewModel parent)
            : this(parent, true)
        {
            if (web == null) throw new ArgumentNullException(nameof(web));
            _web = web;
        }

        /// <summary>
        ///     Initializes a new instance of the SiteItemViewModel class.
        /// </summary>
        protected SPListCollectionViewModel(SPWebViewModel parent, bool lazyLoadChildren)
            : base(parent, lazyLoadChildren)
        {
        }

        public override string ID => $"ListCollection_{_web.Web.Id}";
        public override ImageSource ImageSource => new BitmapImage(new Uri("pack://application:,,,/images/ITGEN.png"));

        public override string Name => string.IsNullOrEmpty(base.Name) ? "Lists" : base.Name;

        protected override IPromise<object, Exception> LoadChildrenAsync()
        {
            var promise = Utility.ExecuteAsync(_web.IncludeLists().LoadAsync());
            promise.Done(() =>
            {
                var lists = _web.GetLists();
                Name = $"Lists ({lists.Length})";
                foreach (var list in lists.OrderBy(l => l.List.Title))
                {
                    var l = list;
                    Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                    {
                        var viewModel = new SPListViewModel(l, this);
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