using System;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using SharePoint.Remote.Access.Helpers;

namespace SP2013Access.ViewModels
{
    public class SPSiteFieldCollectionViewModel : TreeViewItemViewModel
    {
        private readonly SPClientWeb _web;

        public SPSiteFieldCollectionViewModel(SPClientWeb web, SPWebViewModel parent)
            : this(parent, true)
        {
            if (web == null) throw new ArgumentNullException(nameof(web));
            _web = web;
        }

        /// <summary>
        ///     Initializes a new instance of the SiteItemViewModel class.
        /// </summary>
        protected SPSiteFieldCollectionViewModel(SPWebViewModel parent, bool lazyLoadChildren)
            : base(parent, lazyLoadChildren)
        {
        }

        public override string ID => $"SiteFieldCollection_{_web.Web.Id}";

        public override ImageSource ImageSource => new BitmapImage(new Uri("pack://application:,,,/images/SiteColumn.png"));

        public override string Name => string.IsNullOrEmpty(base.Name) ? "Fields" : base.Name;

        protected override IPromise<object, Exception> LoadChildrenAsync()
        {
            var promise = Utility.ExecuteAsync(_web.IncludeFields().LoadAsync());
            promise.Done(() =>
            {
                var fields = _web.GetFields();
                Name = string.Format("Fields ({0})", fields.Length);
                foreach (var field in fields.OrderBy(f => f.Field.Title))
                {
                    var f = field;
                    Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                    {
                        var viewModel = new SPFieldViewModel(f, this);
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