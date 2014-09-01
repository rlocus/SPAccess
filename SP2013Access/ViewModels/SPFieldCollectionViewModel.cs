using SharePoint.Remote.Access.Extensions;
using SharePoint.Remote.Access.Helpers;
using System;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SP2013Access.ViewModels
{
    public class SPFieldCollectionViewModel : TreeViewItemViewModel
    {
        private readonly SPClientList _list;

        public override string ID
        {
            get { return string.Format("FieldCollection_{0}_{1}", _list.ClientWeb.Id, _list.Id); }
        }

        public override ImageSource ImageSource
        {
            get
            {
                return new BitmapImage(new Uri("pack://application:,,,/images/SiteColumn.png"));
            }
        }

        public SPFieldCollectionViewModel(SPClientList list, SPListViewModel parent)
            : this(parent, true)
        {
            if (list == null) throw new ArgumentNullException("list");
            _list = list;
        }

        /// <summary>
        /// Initializes a new instance of the SiteItemViewModel class.
        /// </summary>
        protected SPFieldCollectionViewModel(SPListViewModel parent, bool lazyLoadChildren)
            : base(parent, lazyLoadChildren)
        {
        }

        public override void LoadChildren()
        {
            base.LoadChildren();

            var promise = Utility.ExecuteAsync(_list.LoadFieldsAsync());

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

        public override void Refresh()
        {
            base.Refresh();

            //if (this.HasDummyChild)
            //{
            //    this.IsBusy = false;
            //    this.IsLoaded = true;
            //    return;
            //}

            if (this.HasDummyChild)
            {
                if (Parent != null)
                {
                    var fields = (Parent as SPListViewModel).Fields;

                    if (fields != null)
                    {
                        fields.RefreshLoad();

                        var promise = Utility.ExecuteAsync(_list.Context.ExecuteQueryAsync());

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

                var promise = Utility.ExecuteAsync(_list.LoadFieldsAsync());

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
        }
    }
}