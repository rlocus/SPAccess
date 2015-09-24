using System;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using SharePoint.Remote.Access.Helpers;

namespace SP2013Access.ViewModels
{
    public class SPWebContentTypeCollectionViewModel : TreeViewItemViewModel
    {
        private readonly SPClientWeb _web;

        public SPWebContentTypeCollectionViewModel(SPClientWeb web, SPWebViewModel parent)
            : this(parent, true)
        {
            if (web == null) throw new ArgumentNullException(nameof(web));
            _web = web;
        }

        /// <summary>
        ///     Initializes a new instance of the SiteItemViewModel class.
        /// </summary>
        protected SPWebContentTypeCollectionViewModel(SPWebViewModel parent, bool lazyLoadChildren)
            : base(parent, lazyLoadChildren)
        {
        }

        public override string ID => $"ContentTypeCollection_{_web.Web.Id}";

        public override ImageSource ImageSource => new BitmapImage(new Uri("pack://application:,,,/images/ContentType.png"));

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
            var promise = Utility.ExecuteAsync(_web.IncludeContentTypes().LoadAsync());
            promise.Done(() =>
            {
                var contentTypes = _web.GetContentTypes();
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