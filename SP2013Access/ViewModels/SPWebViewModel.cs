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

        public WebCollection SubWebs { get; private set; }

        public ListCollection Lists { get; private set; }

        public ContentTypeCollection ContentTypes { get; private set; }

        public FieldCollection Fields { get; private set; }

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

            SubWebs = _web.GetWebCollection();
            Lists = _web.GetListCollection();
            ContentTypes = _web.GetContentTypeCollection();
            Fields = _web.GetFieldCollection();

            var promise = Utility.ExecuteAsync(_web.Context.ExecuteQueryAsync());

            promise.Done(() =>
            {
                var websViewModel = new SPWebCollectionViewModel(_web, this)
                {
                    Name = string.Format("Webs ({0})", SubWebs.Count)
                };

                if (SubWebs.Count == 0)
                {
                    websViewModel.IsExpanded = true;
                }

                this.Children.Add(websViewModel);

                var listsViewModel = new SPListCollectionViewModel(_web, this)
                {
                    Name = string.Format("Lists ({0})", Lists.Count)
                };

                if (Lists.Count == 0)
                {
                    listsViewModel.IsExpanded = true;
                }

                this.Children.Add(listsViewModel);

                var contentTypesViewModel = new SPWebContentTypeCollectionViewModel(_web, this)
                {
                    Name = string.Format("Content Types ({0})", ContentTypes.Count)
                };

                if (ContentTypes.Count == 0)
                {
                    contentTypesViewModel.IsExpanded = true;
                }

                this.Children.Add(contentTypesViewModel);

                var fieldsViewModel = new SPSiteFieldCollectionViewModel(_web, this)
                {
                    Name = string.Format("Site Fields ({0})", Fields.Count)
                };

                if (Fields.Count == 0)
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

            var context = _web.Context;
            _web.RefreshLoad();
            context.Load(_web);

            if (SubWebs != null)
            {
                SubWebs.RefreshLoad();
                context.Load(SubWebs);
            }

            if (Lists != null)
            {
                Lists.RefreshLoad();
                context.Load(Lists);
            }

            if (ContentTypes != null)
            {
                ContentTypes.RefreshLoad();
                context.Load(ContentTypes);
            }

            if (Fields != null)
            {
                Fields.RefreshLoad();
                context.Load(Fields);
            }

            var promise = Utility.ExecuteAsync(_web.Context.ExecuteQueryAsync());

            promise.Done(() =>
            {
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