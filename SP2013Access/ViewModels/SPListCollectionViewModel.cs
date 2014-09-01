using SharePoint.Remote.Access.Extensions;
using SharePoint.Remote.Access.Helpers;
using System;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SP2013Access.ViewModels
{
    public class SPListCollectionViewModel : TreeViewItemViewModel
    {
        private readonly SPClientWeb _web;

        public override string ID
        {
            get { return string.Format("ListCollection_{0}", _web.Id); }
        }

        public override ImageSource ImageSource
        {
            get
            {
                return new BitmapImage(new Uri("pack://application:,,,/images/ITGEN.png"));
            }
        }

        public SPListCollectionViewModel(SPClientWeb web, SPWebViewModel parent)
            : this(parent, true)
        {
            if (web == null) throw new ArgumentNullException("web");
            _web = web;
        }

        /// <summary>
        /// Initializes a new instance of the SiteItemViewModel class.
        /// </summary>
        protected SPListCollectionViewModel(SPWebViewModel parent, bool lazyLoadChildren)
            : base(parent, lazyLoadChildren)
        {
        }

        public override void LoadChildren()
        {
            base.LoadChildren();

            var promise = Utility.ExecuteAsync(_web.LoadListsAsync());

            promise.Done((lists) =>
            {
                int count = lists.Count();
                this.Name = string.Format("Lists ({0})", count);
                if (count == 0)
                {
                    this.IsExpanded = true;
                }
                foreach (SPClientList list in lists)
                {
                    var viewModel = new SPListViewModel(list, this);
                    viewModel.Name = string.Format("{0} ({1})", list.Title, list.ItemCount);
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
                    var lists = (Parent as SPWebViewModel).Lists;

                    if (lists != null)
                    {
                        lists.RefreshLoad();

                        var promise = Utility.ExecuteAsync(_web.Context.ExecuteQueryAsync());

                        promise.Done(() =>
                        {
                            int count = lists.Count();
                            this.Name = string.Format("Lists ({0})", count);
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

                var promise = Utility.ExecuteAsync(_web.LoadListsAsync());

                promise.Done((lists) =>
                {
                    int count = lists.Count();
                    this.Name = string.Format("Lists ({0})", count);
                    if (count == 0)
                    {
                        this.IsExpanded = true;
                    }
                    foreach (SPClientList list in lists)
                    {
                        var viewModel = new SPListViewModel(list, this);
                        viewModel.Name = string.Format("{0} ({1})", list.Title, list.ItemCount);
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