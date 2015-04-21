using SharePoint.Remote.Access.Helpers;
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SP2013Access.ViewModels
{
    public class SPSiteFieldCollectionViewModel : TreeViewItemViewModel
    {
        private readonly SPClientWeb _web;

        public override string ID
        {
            get { return string.Format("SiteFieldCollection_{0}", _web.Web.Id); }
        }

        public override ImageSource ImageSource
        {
            get
            {
                return new BitmapImage(new Uri("pack://application:,,,/images/SiteColumn.png"));
            }
        }

        public override string Name
        {
            get
            {
                if (string.IsNullOrEmpty(base.Name))
                {
                    return "Fields";
                }
                return base.Name;
            }
        }

        public SPSiteFieldCollectionViewModel(SPClientWeb web, SPWebViewModel parent)
            : this(parent, true)
        {
            if (web == null) throw new ArgumentNullException("web");
            _web = web;
        }

        /// <summary>
        /// Initializes a new instance of the SiteItemViewModel class.
        /// </summary>
        protected SPSiteFieldCollectionViewModel(SPWebViewModel parent, bool lazyLoadChildren)
            : base(parent, lazyLoadChildren)
        {
        }

        public override void LoadChildren()
        {
            base.LoadChildren();

            var promise = Utility.ExecuteAsync(_web.IncludeFields().LoadAsync());

            promise.Done(() =>
            {
                var fields = _web.GetFields();
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

            //if (Parent != null)
            //{
            //    var fields = (Parent as SPWebViewModel).Fields;

            //    foreach (SPClientField field in fields)
            //    {
            //        this.Children.Add(new SPFieldViewModel(field, this));
            //    }
            //}
        }

        public override void Refresh()
        {
            base.Refresh();

            //if (this.HasDummyChild)
            //{
            //    this.IsBusy = false;
            //    this.IsLoaded = true;
            //    return;
            //}

            //if (this.HasDummyChild)
            //{
            //    if (Parent != null)
            //    {
            //        var fields = (Parent as SPWebViewModel).Fields;

            //        if (fields != null)
            //        {
            //            fields.RefreshLoad();

            //            var promise = Utility.ExecuteAsync(_web.Context.ExecuteQueryAsync());

            //            promise.Done(() =>
            //            {
            //                int count = fields.Count();
            //                this.Name = string.Format("Site Fields ({0})", count);
            //                if (count == 0)
            //                {
            //                    this.IsExpanded = true;
            //                }
            //            }).Always(() =>
            //            {
            //                this.IsBusy = false;
            //                this.IsLoaded = true;
            //            });
            //        }
            //        else
            //        {
            //            this.IsBusy = false;
            //            this.IsLoaded = true;
            //        }
            //    }
            //    else
            //    {
            //        this.IsBusy = false;
            //        this.IsLoaded = true;
            //    }
            //}
            //else
            //{
            //    this.Children.Clear();

            //    var promise = Utility.ExecuteAsync(_web.LoadFieldsAsync());

            //    promise.Done((fields) =>
            //    {
            //        int count = fields.Count();
            //        this.Name = string.Format("Site Fields ({0})", count);
            //        if (count == 0)
            //        {
            //            this.IsExpanded = true;
            //        }
            //        foreach (SPClientField field in fields)
            //        {
            //            var viewModel = new SPFieldViewModel(field, this);
            //            viewModel.LoadChildren();
            //            this.Children.Add(viewModel);
            //        }
            //    });

            //    promise.Fail((ex) =>
            //    {
            //    });

            //    promise.Always(() =>
            //    {
            //        this.IsBusy = false;
            //        this.IsLoaded = true;
            //    });
            //}
        }
    }
}