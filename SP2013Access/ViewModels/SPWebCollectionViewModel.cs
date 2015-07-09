using System.Linq;
using System.Windows.Threading;
using SharePoint.Remote.Access.Helpers;
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SP2013Access.ViewModels
{
    public class SPWebCollectionViewModel : TreeViewItemViewModel
    {
        private readonly SPClientWeb _web;

        public override string ID
        {
            get { return string.Format("WebCollection_{0}", _web.Web.Id); }
        }

        public override ImageSource ImageSource
        {
            get
            {
                return new BitmapImage(new Uri("pack://application:,,,/images/SubSite.png"));
            }
        }

        public override string Name
        {
            get
            {
                if (string.IsNullOrEmpty(base.Name))
                {
                    return "Webs";
                }
                return base.Name;
            }
        }

        public SPWebCollectionViewModel(SPClientWeb web, SPWebViewModel parent)
            : this(parent, true)
        {
            if (web == null) throw new ArgumentNullException("web");
            _web = web;
        }

        /// <summary>
        /// Initializes a new instance of the SiteItemViewModel class.
        /// </summary>
        protected SPWebCollectionViewModel(SPWebViewModel parent, bool lazyLoadChildren)
            : base(parent, lazyLoadChildren)
        {
        }

        protected override IPromise<object, Exception> LoadChildrenAsync()
        {
            var promise = Utility.ExecuteAsync(_web.IncludeWebs().LoadAsync());
            promise.Done(() =>
            {
                var webs = _web.GetWebs();
                Name = string.Format("Webs ({0})", webs.Length);
                foreach (SPClientWeb web in webs.OrderBy(w => w.Web.Title))
                {
                    SPClientWeb w = web;
                    Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                    {
                        var viewModel = new SPWebViewModel(w, this);
                        this.Children.Add(viewModel);
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