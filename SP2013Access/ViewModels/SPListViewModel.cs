using Microsoft.SharePoint.Client;
using SharePoint.Remote.Access.Extensions;
using SharePoint.Remote.Access.Helpers;
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SP2013Access.ViewModels
{
    public class SPListViewModel : TreeViewItemViewModel
    {
        private readonly SPClientList _list;
        private ContentTypeCollection _contentTypes;
        private FieldCollection _fields;

        public override string ID
        {
            get { return string.Format("List_{0}_{1}", _list.ClientWeb.Id, _list.Id); }
        }

        public override string Name
        {
            get
            {
                if (string.IsNullOrEmpty(base.Name))
                {
                    return _list.Title;
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

        public ContentTypeCollection ContentTypes
        {
            get { return _contentTypes; }
        }

        public FieldCollection Fields
        {
            get { return _fields; }
        }

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

            _contentTypes = _list.GetContentTypeCollection();
            _fields = _list.GetFieldCollection();

            var promise = Utility.ExecuteAsync(_list.Context.ExecuteQueryAsync());

            promise.Done(() =>
            {
                var contentTypesViewModel = new SPListContentTypeCollectionViewModel(_list, this);
                contentTypesViewModel.Name = string.Format("Content Types ({0})", _contentTypes.Count);

                if (_contentTypes.Count == 0)
                {
                    contentTypesViewModel.IsExpanded = true;
                }

                this.Children.Add(contentTypesViewModel);

                var fieldsViewModel = new SPFieldCollectionViewModel(_list, this);
                fieldsViewModel.Name = string.Format("Fields ({0})", _fields.Count);

                if (_fields.Count == 0)
                {
                    fieldsViewModel.IsExpanded = true;
                }

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

            _list.RefreshLoad();

            if (_contentTypes != null)
            {
                _contentTypes.RefreshLoad();
            }

            if (_fields != null)
            {
                _fields.RefreshLoad();
            }

            var promise = Utility.ExecuteAsync(_list.Context.ExecuteQueryAsync());

            promise.Done(() =>
            {
                if (!this.HasDummyChild)
                {
                    var contentTypesViewModel = new SPListContentTypeCollectionViewModel(_list, this);
                    contentTypesViewModel.Name = string.Format("Content Types ({0})", _contentTypes.Count);

                    if (_contentTypes.Count == 0)
                    {
                        contentTypesViewModel.IsExpanded = true;
                    }

                    this.Children.Add(contentTypesViewModel);

                    var fieldsViewModel = new SPFieldCollectionViewModel(_list, this);
                    fieldsViewModel.Name = string.Format("Fields ({0})", _fields.Count);

                    if (_fields.Count == 0)
                    {
                        fieldsViewModel.IsExpanded = true;
                    }

                    this.Children.Add(fieldsViewModel);
                }
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
    }
}