using Microsoft.SharePoint.Client;
using SharePoint.Remote.Access.Extensions;
using SharePoint.Remote.Access.Helpers;
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SP2013Access.ViewModels
{
    public class SPContentTypeViewModel : TreeViewItemViewModel
    {
        private readonly ContentType _contentType;
        private FieldCollection _fields;

        public override string ID
        {
            get { return string.Format("ContentType_{0}", _contentType.Id); }
        }

        public override string Name
        {
            get { return _contentType.Name; }
        }

        public FieldCollection Fields
        {
            get { return _fields; }
        }

        public override ImageSource ImageSource
        {
            get
            {
                return new BitmapImage(new Uri("pack://application:,,,/images/ContentType.png"));
            }
        }

        public SPContentTypeViewModel(ContentType contentType, TreeViewItemViewModel parent)
            : this(parent, true)
        {
            if (contentType == null) throw new ArgumentNullException("contentType");
            _contentType = contentType;
        }

        /// <summary>
        /// Initializes a new instance of the SiteItemViewModel class.
        /// </summary>
        protected SPContentTypeViewModel(TreeViewItemViewModel parent, bool lazyLoadChildren)
            : base(parent, lazyLoadChildren)
        {
        }

        public override void LoadChildren()
        {
            base.LoadChildren();

            _fields = _contentType.GetFieldCollection();

            var promise = Utility.ExecuteAsync(_contentType.Context.ExecuteQueryAsync());

            promise.Done(() =>
            {
                var fieldsViewModel = new SPContentTypeFieldCollectionViewModel(_contentType, this);
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

            _contentType.RefreshLoad();

            if (_fields != null)
            {
                _fields.RefreshLoad();
            }

            if (_fields != null)
            {
                _fields.RefreshLoad();
            }

            var promise = Utility.ExecuteAsync(_contentType.Context.ExecuteQueryAsync());

            promise.Done(() =>
            {
                if (!this.HasDummyChild)
                {
                    var fieldsViewModel = new SPContentTypeFieldCollectionViewModel(_contentType, this);
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