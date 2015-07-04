using System.Linq;
using SharePoint.Remote.Access.Helpers;
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SP2013Access.ViewModels
{
    public class SPListCollectionViewModel : TreeViewItemViewModel
    {
        private readonly SPClientWeb _web;

        public override string ID
        {
            get { return string.Format("ListCollection_{0}", _web.Web.Id); }
        }

        public override ImageSource ImageSource
        {
            get
            {
                return new BitmapImage(new Uri("pack://application:,,,/images/ITGEN.png"));
            }
        }

        public override string Name
        {
            get
            {
                if (string.IsNullOrEmpty(base.Name))
                {
                    return "Lists";
                }
                return base.Name;
            }
        }

        public SPListCollectionViewModel(SPClientWeb web, SPWebViewModel parent)
            : this(parent, true)
        {
            if (web == null) throw new ArgumentNullException("web");
            _web = web;
        }

        /// <summary>
        /// Initializes a new instance of the SiteItemViewModel class.
        /// </summary>
        protected SPListCollectionViewModel(SPWebViewModel parent, bool lazyLoadChildren)
            : base(parent, lazyLoadChildren)
        {
        }

        public override void LoadChildren()
        {
            base.LoadChildren();
            var promise = Utility.ExecuteAsync(_web.IncludeLists().LoadAsync());
            promise.Done(() =>
            {
                var lists = _web.GetLists();
                Name = string.Format("Lists ({0})", lists.Length);
                foreach (SPClientList list in lists.OrderBy(l => l.List.Title))
                {
                    var viewModel = new SPListViewModel(list, this);
                    viewModel.LoadChildren();
                    this.Children.Add(viewModel);
                }
            });
            promise.Fail((ex) => { if (OnExceptionCommand != null) OnExceptionCommand.Execute(ex); });
            promise.Always(() =>
            {
                this.IsBusy = false;
                this.IsLoaded = true;
            });
        }


        public override void Refresh()
        {
            base.Refresh();
            base.IsExpanded = true;
        }
    }
}