using SharePoint.Remote.Access.Helpers;
using System;

namespace SP2013Access.ViewModels
{
    public class SPContextViewModel : TreeViewItemViewModel
    {
        private readonly SPClientContext _clientContext;

        public override string ID
        {
            get { return _clientContext.ClientTag; }
        }

        public override string Name
        {
            get { return _clientContext.Url; }
        }

        public SPContextViewModel(SPClientContext clientContext)
            : this(null, false)
        {
            if (clientContext == null) throw new ArgumentNullException("clientContext");
            _clientContext = clientContext;
        }

        /// <summary>
        /// Initializes a new instance of the SiteItemViewModel class.
        /// </summary>
        protected SPContextViewModel(TreeViewItemViewModel parent, bool lazyLoadChildren)
            : base(parent, lazyLoadChildren)
        {
        }

        public override void LoadChildren()
        {
            base.LoadChildren();

            var viewModel = new SPSiteViewModel(_clientContext.ClientSite, this);
            this.Children.Add(viewModel);
            viewModel.LoadChildren();
        }
    }
}