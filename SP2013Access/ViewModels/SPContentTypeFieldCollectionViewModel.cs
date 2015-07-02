using SharePoint.Remote.Access.Helpers;
using System;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SP2013Access.ViewModels
{
    public class SPContentTypeFieldCollectionViewModel : TreeViewItemViewModel
    {
        private readonly SPClientContentType _contentType;

        public override string ID
        {
            get { return string.Format("ContentTypeFieldCollection_{0}", _contentType.ContentType.Id); }
        }

        public override ImageSource ImageSource
        {
            get
            {
                return new BitmapImage(new Uri("pack://application:,,,/images/SiteColumn.png"));
            }
        }

        public SPContentTypeFieldCollectionViewModel(SPClientContentType contentType, SPContentTypeViewModel parent)
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

            //if (Parent != null)
            //{
            //    var fields = (Parent as SPContentTypeViewModel).Fields;

            //    foreach (SPClientField field in fields)
            //    {
            //        this.Children.Add(new SPFieldViewModel(field, this));
            //    }
            //}

            var promise = Utility.ExecuteAsync(_contentType.IncludeFields().LoadAsync());

            promise.Done(() =>
            {
                var fields = _contentType.GetFields();
                Name = string.Format("Fields ({0})", fields.Length);

                foreach (SPClientField field in fields)
                {
                    var viewModel = new SPFieldViewModel(field, this);
                    viewModel.LoadChildren();
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

        //public override void Refresh()
        //{
        //    base.Refresh();
        //}
    }
}