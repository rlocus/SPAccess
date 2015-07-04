﻿using SharePoint.Remote.Access.Helpers;
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SP2013Access.ViewModels
{
    public class SPListViewModel : TreeViewItemViewModel
    {
        private readonly SPClientList _list;

        public override string ID
        {
            get { return string.Format("List_{0}_{1}", _list.ClientWeb.Web.Id, _list.List.Id); }
        }

        public override string Name
        {
            get
            {
                if (string.IsNullOrEmpty(base.Name))
                {
                    return string.Format("{0} ({1})", _list.List.Title, _list.List.ItemCount);
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

        public SPListViewModel(SPClientList list, SPListCollectionViewModel parent)
            : this(parent, false)
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
            var contentTypesViewModel = new SPListContentTypeCollectionViewModel(_list, this);
            this.Children.Add(contentTypesViewModel);
            var fieldsViewModel = new SPFieldCollectionViewModel(_list, this);
            this.Children.Add(fieldsViewModel);
            base.LoadChildren();
        }

        public override void Refresh()
        {
            base.Refresh();
            this.IsBusy = true;
            this.IsLoaded = false;
            _list.RefreshLoad();
            var promise = Utility.ExecuteAsync(_list.LoadAsync());
            promise.Done(() =>
            {
                IsExpanded = true;
                Name = string.Format("{0} ({1})", _list.List.Title, _list.List.ItemCount);
            });
            promise.Fail((ex) => { if (OnExceptionCommand != null) OnExceptionCommand.Execute(ex); });
            promise.Always(() =>
            {
                this.IsBusy = false;
                this.IsLoaded = true;
            });
        }
    }
}