using SharePoint.Remote.Access.Extensions;
using SharePoint.Remote.Access.Helpers;
using System;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SP2013Access.ViewModels
{
    public class SPWebCollectionViewModel : TreeViewItemViewModel
    {
        private readonly SPClientWeb _web;

        public override string ID
        {
            get { return string.Format("WebCollection_{0}", _web.Id); }
        }

        public override ImageSource ImageSource
        {
            get
            {
                return new BitmapImage(new Uri("pack://application:,,,/images/SubSite.png"));
            }
        }

        public SPWebCollectionViewModel(SPClientWeb web, SPWebViewModel parent)
            : this(parent, true)
        {
            if (web == null) throw new ArgumentNullException("web");
            _web = web;
        }

        /// <summary>
        /// Initializes a new instance of the SiteItemViewModel class.
        /// </summary>
        protected SPWebCollectionViewModel(SPWebViewModel parent, bool lazyLoadChildren)
            : base(parent, lazyLoadChildren)
        {
        }

        public override void LoadChildren()
        {
            base.LoadChildren();

            var promise = Utility.ExecuteAsync(_web.LoadWebsAsync());

            promise.Done((subWebs) =>
            {
                int count = subWebs.Count();
                this.Name = string.Format("Webs ({0})", count);
                if (count == 0)
                {
                    this.IsExpanded = true;
                }
                foreach (SPClientWeb web in subWebs)
                {
                    this.Children.Add(new SPWebViewModel(web, this));
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
                    var subWebs = (Parent as SPWebViewModel).SubWebs;

                    if (subWebs != null)
                    {
                        subWebs.RefreshLoad();

                        var promise = Utility.ExecuteAsync(_web.Context.ExecuteQueryAsync());

                        promise.Done(() =>
                        {
                            int count = subWebs.Count();
                            this.Name = string.Format("Webs ({0})", count);
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

                var promise = Utility.ExecuteAsync(_web.LoadWebsAsync());

                promise.Done((subWebs) =>
                {
                    int count = subWebs.Count();
                    this.Name = string.Format("Webs ({0})", count);
                    if (count == 0)
                    {
                        this.IsExpanded = true;
                    }
                    foreach (SPClientWeb web in subWebs)
                    {
                        this.Children.Add(new SPWebViewModel(web, this));
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