using Microsoft.SharePoint.Client;
using SharePoint.Remote.Access.Extensions;
using SharePoint.Remote.Access.Helpers;
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SP2013Access.ViewModels
{
    public class SPWebViewModel : TreeViewItemViewModel
    {
        private readonly SPClientWeb _web;
        private WebCollection _subWebs;
        private ListCollection _lists;
        private ContentTypeCollection _contentTypes;
        private FieldCollection _fields;

        public override string ID
        {
            get { return string.Format("Web_{0}", _web.Id); }
        }

        public override string Name
        {
            get { return _web.Title; }
        }

        public override ImageSource ImageSource
        {
            get
            {
                return new BitmapImage(new Uri("pack://application:,,,/images/siteicon_16x16.png"));
            }
        }

        public SPClientWeb Web
        {
            get { return _web; }
        }

        public WebCollection SubWebs
        {
            get { return _subWebs; }
        }

        public ListCollection Lists
        {
            get { return _lists; }
        }

        public ContentTypeCollection ContentTypes
        {
            get { return _contentTypes; }
        }

        public FieldCollection Fields
        {
            get { return _fields; }
        }

        public SPWebViewModel(SPClientWeb web, TreeViewItemViewModel parent)
            : this(parent, true)
        {
            if (web == null) throw new ArgumentNullException("web");
            _web = web;
        }

        /// <summary>
        /// Initializes a new instance of the SiteItemViewModel class.
        /// </summary>
        protected SPWebViewModel(TreeViewItemViewModel parent, bool lazyLoadChildren)
            : base(parent, lazyLoadChildren)
        {
        }

        public override void LoadChildren()
        {
            base.LoadChildren();

            _subWebs = _web.GetWebCollection();
            _lists = _web.GetListCollection();
            _contentTypes = _web.GetContentTypeCollection();
            _fields = _web.GetFieldCollection();

            var promise = Utility.ExecuteAsync(_web.Context.ExecuteQueryAsync());

            promise.Done(() =>
            {
                var websViewModel = new SPWebCollectionViewModel(_web, this);
                websViewModel.Name = string.Format("Webs ({0})", _subWebs.Count);

                if (_subWebs.Count == 0)
                {
                    websViewModel.IsExpanded = true;
                }

                this.Children.Add(websViewModel);

                var listsViewModel = new SPListCollectionViewModel(_web, this);
                listsViewModel.Name = string.Format("Lists ({0})", _lists.Count);

                if (_lists.Count == 0)
                {
                    listsViewModel.IsExpanded = true;
                }

                this.Children.Add(listsViewModel);

                var contentTypesViewModel = new SPWebContentTypeCollectionViewModel(_web, this);
                contentTypesViewModel.Name = string.Format("Content Types ({0})", _contentTypes.Count);

                if (_contentTypes.Count == 0)
                {
                    contentTypesViewModel.IsExpanded = true;
                }

                this.Children.Add(contentTypesViewModel);

                var fieldsViewModel = new SPSiteFieldCollectionViewModel(_web, this);
                fieldsViewModel.Name = string.Format("Site Fields ({0})", _fields.Count);

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

            _web.RefreshLoad();

            if (_subWebs != null)
            {
                _subWebs.RefreshLoad();
            }

            if (_lists != null)
            {
                _lists.RefreshLoad();
            }

            if (_contentTypes != null)
            {
                _contentTypes.RefreshLoad();
            }

            if (_fields != null)
            {
                _fields.RefreshLoad();
            }

            var promise = Utility.ExecuteAsync(_web.Context.ExecuteQueryAsync());

            promise.Done(() =>
            {
                if (!this.HasDummyChild)
                {
                    var websViewModel = new SPWebCollectionViewModel(_web, this);
                    websViewModel.Name = string.Format("Webs ({0})", _subWebs.Count);

                    if (_subWebs.Count == 0)
                    {
                        websViewModel.IsExpanded = true;
                    }

                    this.Children.Add(websViewModel);

                    var listsViewModel = new SPListCollectionViewModel(_web, this);
                    listsViewModel.Name = string.Format("Lists ({0})", _lists.Count);

                    if (_lists.Count == 0)
                    {
                        listsViewModel.IsExpanded = true;
                    }

                    this.Children.Add(listsViewModel);

                    var contentTypesViewModel = new SPWebContentTypeCollectionViewModel(_web, this);
                    contentTypesViewModel.Name = string.Format("Content Types ({0})", _contentTypes.Count);

                    if (_contentTypes.Count == 0)
                    {
                        contentTypesViewModel.IsExpanded = true;
                    }

                    this.Children.Add(contentTypesViewModel);

                    var fieldsViewModel = new SPSiteFieldCollectionViewModel(_web, this);
                    fieldsViewModel.Name = string.Format("Site Fields ({0})", _fields.Count);

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