using SharePoint.Remote.Access.Helpers;
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SP2013Access.ViewModels
{
    public class SPListViewModel : TreeViewItemViewModel
    {
        private readonly SPClientList _list;

        public override string ID
        {
            get { return string.Format("List_{0}_{1}", _list.ClientWeb.Web.Id, _list.List.Id); }
        }

        public override string Name
        {
            get
            {
                if (string.IsNullOrEmpty(base.Name))
                {
                    return string.Format("{0} ({1})", _list.List.Title, _list.List.ItemCount);
                }
                return base.Name;
            }
        }

        public override ImageSource ImageSource
        {
            get
            {
                return new BitmapImage(new Uri("pack://application:,,,/images/ITGEN.png"));
            }
        }

        //public SPClientContentType[] ContentTypes { get; private set; }

        //public SPClientField[] Fields { get; private set; }

        public SPListViewModel(SPClientList list, SPListCollectionViewModel parent)
            : this(parent, true)
        {
            if (list == null) throw new ArgumentNullException("list");
            _list = list;
        }

        /// <summary>
        /// Initializes a new instance of the SiteItemViewModel class.
        /// </summary>
        protected SPListViewModel(SPListCollectionViewModel parent, bool lazyLoadChildren)
            : base(parent, lazyLoadChildren)
        {
        }

        public override void LoadChildren()
        {
            base.LoadChildren();

            //var promise = Utility.ExecuteAsync(_list.IncludeContentTypes().IncludeFields().LoadAsync());
            var promise = Utility.ExecuteAsync(_list.LoadAsync());

            promise.Done(() =>
            {
                //ContentTypes = _list.GetContentTypes();
                var contentTypesViewModel = new SPListContentTypeCollectionViewModel(_list, this)
                {
                    Name = string.Format("Content Types ({0})", /*ContentTypes.Length*/ 0)
                };

                //contentTypesViewModel.LoadChildren();

                //if (ContentTypes.Length == 0)
                //{
                //    contentTypesViewModel.IsExpanded = true;
                //}
                
                this.Children.Add(contentTypesViewModel);

                //Fields = _list.GetFields();

                var fieldsViewModel = new SPFieldCollectionViewModel(_list, this)
                {
                    Name = string.Format("Fields ({0})", /*Fields.Length*/ 0)
                };

                //fieldsViewModel.LoadChildren();

                //if (Fields.Length == 0)
                //{
                //    fieldsViewModel.IsExpanded = true;
                //}

                this.Children.Add(fieldsViewModel);
            });

            promise.Fail((ex) =>
            {
            });

            promise.Always(() =>
            {
                this.IsBusy = false;
                this.IsLoaded = true;
            });
        }

        public override void Refresh()
        {
            base.Refresh();

            //_list.RefreshLoad();

            //if (ContentTypes != null)
            //{
            //    ContentTypes.RefreshLoad();
            //}

            //if (Fields != null)
            //{
            //    Fields.RefreshLoad();
            //}

            //var promise = Utility.ExecuteAsync(_list.List.Context.ExecuteQueryAsync());

            //promise.Done(() =>
            //{
            //    if (!this.HasDummyChild)
            //    {
            //        var contentTypesViewModel = new SPListContentTypeCollectionViewModel(_list, this);
            //        contentTypesViewModel.Name = string.Format("Content Types ({0})", ContentTypes.Count);

            //        if (ContentTypes.Count == 0)
            //        {
            //            contentTypesViewModel.IsExpanded = true;
            //        }

            //        this.Children.Add(contentTypesViewModel);

            //        var fieldsViewModel = new SPFieldCollectionViewModel(_list, this);
            //        fieldsViewModel.Name = string.Format("Fields ({0})", Fields.Count);

            //        if (Fields.Count == 0)
            //        {
            //            fieldsViewModel.IsExpanded = true;
            //        }

            //        this.Children.Add(fieldsViewModel);
            //    }
            //});

            //promise.Fail((ex) =>
            //{
            //});

            //promise.Always(() =>
            //{
            //    this.IsBusy = false;
            //    this.IsLoaded = true;
            //});
        }
    }
}