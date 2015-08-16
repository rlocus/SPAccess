﻿using System;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using SharePoint.Remote.Access.Helpers;

namespace SP2013Access.ViewModels
{
    public class SPContentTypeFieldCollectionViewModel : TreeViewItemViewModel
    {
        private readonly SPClientContentType _contentType;

        public SPContentTypeFieldCollectionViewModel(SPClientContentType contentType, SPContentTypeViewModel parent)
            : this(parent, true)
        {
            if (contentType == null) throw new ArgumentNullException("contentType");
            _contentType = contentType;
        }

        /// <summary>
        ///     Initializes a new instance of the SiteItemViewModel class.
        /// </summary>
        protected SPContentTypeFieldCollectionViewModel(SPContentTypeViewModel parent, bool lazyLoadChildren)
            : base(parent, lazyLoadChildren)
        {
        }

        public override string ID
        {
            get { return string.Format("ContentTypeFieldCollection_{0}", _contentType.ContentType.Id); }
        }

        public override ImageSource ImageSource
        {
            get { return new BitmapImage(new Uri("pack://application:,,,/images/SiteColumn.png")); }
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

        protected override IPromise<object, Exception> LoadChildrenAsync()
        {
            var promise = Utility.ExecuteAsync(_contentType.IncludeFields().LoadAsync());
            promise.Done(() =>
            {
                var fields = _contentType.GetFields();
                Name = string.Format("Fields ({0})", fields.Length);
                foreach (var field in fields.OrderBy(f => f.Field.Title))
                {
                    var f = field;
                    Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                    {
                        var viewModel = new SPFieldViewModel(f, this);
                        Children.Add(viewModel);
                    }));
                }
            });
            return promise;
        }

        public override void Refresh()
        {
            base.Refresh();
            IsExpanded = true;
        }
    }
}