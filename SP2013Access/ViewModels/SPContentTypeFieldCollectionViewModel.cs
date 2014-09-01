using SharePoint.Remote.Access.Extensions;
using SharePoint.Remote.Access.Helpers;
using System;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ContentType = Microsoft.SharePoint.Client.ContentType;

namespace SP2013Access.ViewModels
{
    public class SPContentTypeFieldCollectionViewModel : TreeViewItemViewModel
    {
        private readonly ContentType _contentType;

        public override string ID
        {
            get { return string.Format("ContentTypeFieldCollection_{0}", _contentType.Id); }
        }

        public override ImageSource ImageSource
        {
            get
            {
                return new BitmapImage(new Uri("pack://application:,,,/images/SiteColumn.png"));
            }
        }

        public SPContentTypeFieldCollectionViewModel(ContentType contentType, SPContentTypeViewModel parent)
            : this(parent, true)
        {
            if (contentType == null) throw new ArgumentNullException("contentType");
            _contentType = contentType;
        }

        /// <summary>
        /// Initializes a new instance of the SiteItemViewModel class.
        /// </summary>
        protected SPContentTypeFieldCollectionViewModel(SPContentTypeViewModel parent, bool lazyLoadChildren)
            : base(parent, lazyLoadChildren)
        {
        }

        public override void LoadChildren()
        {
            base.LoadChildren();

            var promise = Utility.ExecuteAsync(_contentType.LoadFieldsAsync());

            promise.Done((fields) =>
            {
                int count = fields.Count();
                this.Name = string.Format("Fields ({0})", count);
                if (count == 0)
                {
                    this.IsExpanded = true;
                }
                foreach (SPClientField field in fields)
                {
                    var viewModel = new SPFieldViewModel(field, this);
                    viewModel.IsExpanded = true;
                    this.Children.Add(viewModel);
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

        public override void Refresh()
        {
            base.Refresh();

            if (this.HasDummyChild)
            {
                if (Parent != null)
                {
                    var fields = (Parent as SPContentTypeViewModel).Fields;

                    if (fields != null)
                    {
                        fields.RefreshLoad();

                        var promise = Utility.ExecuteAsync(_contentType.Context.ExecuteQueryAsync());

                        promise.Done(() =>
                        {
                            int count = fields.Count();
                            this.Name = string.Format("Fields ({0})", count);
                            if (count == 0)
                            {
                                this.IsExpanded = true;
                            }
                        }).Always(() =>
                        {
                            this.IsBusy = false;
                            this.IsLoaded = true;
                        });
                    }
                    else
                    {
                        this.IsBusy = false;
                        this.IsLoaded = true;
                    }
                }
                else
                {
                    this.IsBusy = false;
                    this.IsLoaded = true;
                }
            }
            else
            {
                this.Children.Clear();

                var promise = Utility.ExecuteAsync(_contentType.LoadFieldsAsync());

                promise.Done((fields) =>
                {
                    int count = fields.Count();
                    this.Name = string.Format("Fields ({0})", count);
                    if (count == 0)
                    {
                        this.IsExpanded = true;
                    }
                    foreach (SPClientField field in fields)
                    {
                        var viewModel = new SPFieldViewModel(field, this);
                        viewModel.IsExpanded = true;
                        this.Children.Add(viewModel);
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
}