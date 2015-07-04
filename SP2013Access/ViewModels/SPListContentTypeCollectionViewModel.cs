using System.Linq;
using SharePoint.Remote.Access.Helpers;
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SP2013Access.ViewModels
{
    public class SPListContentTypeCollectionViewModel : TreeViewItemViewModel
    {
        private readonly SPClientList _list;

        public override string ID
        {
            get { return string.Format("ContentTypeCollection_{0}", _list.List.Id); }
        }

        public override ImageSource ImageSource
        {
            get
            {
                return new BitmapImage(new Uri("pack://application:,,,/images/ContentType.png"));
            }
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

        public SPListContentTypeCollectionViewModel(SPClientList list, SPListViewModel parent)
            : this(parent, true)
        {
            if (list == null) throw new ArgumentNullException("list");
            _list = list;
        }

        /// <summary>
        /// Initializes a new instance of the SiteItemViewModel class.
        /// </summary>
        protected SPListContentTypeCollectionViewModel(SPListViewModel parent, bool lazyLoadChildren)
            : base(parent, lazyLoadChildren)
        {
        }

        public override void LoadChildren()
        {
            base.LoadChildren();
            var promise = Utility.ExecuteAsync(_list.IncludeContentTypes().LoadAsync());
            promise.Done(() =>
            {
                var contentTypes = _list.GetContentTypes();
                Name = string.Format("Content Types ({0})", contentTypes.Length);

                foreach (SPClientContentType contentType in contentTypes.OrderBy(ct => ct.ContentType.Name))
                {
                    var viewModel = new SPContentTypeViewModel(contentType, this);
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