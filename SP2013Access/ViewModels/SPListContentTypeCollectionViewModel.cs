using System;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using SharePoint.Remote.Access.Helpers;

namespace SP2013Access.ViewModels
{
    public class SPListContentTypeCollectionViewModel : TreeViewItemViewModel
    {
        private readonly SPClientList _list;

        public SPListContentTypeCollectionViewModel(SPClientList list, SPListViewModel parent)
            : this(parent, true)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            _list = list;
        }

        /// <summary>
        ///     Initializes a new instance of the SiteItemViewModel class.
        /// </summary>
        protected SPListContentTypeCollectionViewModel(SPListViewModel parent, bool lazyLoadChildren)
            : base(parent, lazyLoadChildren)
        {
        }

        public override string ID => $"ContentTypeCollection_{_list.List.Id}";

        public override ImageSource ImageSource
            => new BitmapImage(new Uri("pack://application:,,,/images/ContentType.png"));

        public override string Name => string.IsNullOrEmpty(base.Name) ? "Content Types" : base.Name;

        protected override IPromise<object, Exception> LoadChildrenAsync()
        {
            var promise = Utility.ExecuteAsync(_list.IncludeContentTypes().LoadAsync());
            promise.Done(() =>
            {
                var contentTypes = _list.GetContentTypes();
                Name = $"Content Types ({contentTypes.Length})";

                foreach (var contentType in contentTypes.OrderBy(ct => ct.ContentType.Name))
                {
                    var ct = contentType;
                    Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                    {
                        var viewModel = new SPContentTypeViewModel(ct, this);
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