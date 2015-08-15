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
            if (list == null) throw new ArgumentNullException("list");
            _list = list;
        }

        /// <summary>
        ///     Initializes a new instance of the SiteItemViewModel class.
        /// </summary>
        protected SPListContentTypeCollectionViewModel(SPListViewModel parent, bool lazyLoadChildren)
            : base(parent, lazyLoadChildren)
        {
        }

        public override string ID
        {
            get { return string.Format("ContentTypeCollection_{0}", _list.List.Id); }
        }

        public override ImageSource ImageSource
        {
            get { return new BitmapImage(new Uri("pack://application:,,,/images/ContentType.png")); }
        }

        public override string Name
        {
            get
            {
                if (string.IsNullOrEmpty(base.Name))
                {
                    return "Content Types";
                }
                return base.Name;
            }
        }

        protected override IPromise<object, Exception> LoadChildrenAsync()
        {
            var promise = Utility.ExecuteAsync(_list.IncludeContentTypes().LoadAsync());
            promise.Done(() =>
            {
                var contentTypes = _list.GetContentTypes();
                Name = string.Format("Content Types ({0})", contentTypes.Length);

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