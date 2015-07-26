using SharePoint.Remote.Access.Helpers;
using SP2013Access.Commands;
using System;
using System.Collections.Generic;

namespace SP2013Access.ViewModels
{
    public class SPContextCollectionViewModel : TreeViewItemViewModel, IDisposable
    {
        private readonly List<SPClientContext> _clientContexts;

        public override string ID
        {
            get { return "Sites"; }
        }

        public override string Name
        {
            get { return "Sites"; }
        }

        public SPContextCollectionViewModel()
            : this(null, false)
        {
            _clientContexts = new List<SPClientContext>();
        }

        public SPContextCollectionViewModel(IEnumerable<SPClientContext> clientContexts)
            : this()
        {
            if (clientContexts == null) throw new ArgumentNullException("clientContexts");
            _clientContexts.AddRange(clientContexts);
        }

        /// <summary>
        /// Initializes a new instance of the SiteItemViewModel class.
        /// </summary>
        protected SPContextCollectionViewModel(TreeViewItemViewModel parent, bool lazyLoadChildren)
            : base(parent, lazyLoadChildren)
        {
        }

        protected override void LoadChildren()
        {
            if (IsLoaded) return;
            foreach (SPClientContext clientContext in _clientContexts)
            {
                var viewModel = new SPSiteViewModel(clientContext.ClientSite, this);
                this.Children.Add(viewModel);
            } 
            base.LoadChildren();
        }

        public void Add(SPClientContext clientContext)
        {
            var viewModel = new SPSiteViewModel(clientContext.ClientSite, this);
            this.Children.Add(viewModel);
            viewModel.IsExpanded = true;
            viewModel.Commands.Add(new CommandEntity()
            {
                Name = "Close",
                Command = new RelayCommand<object>(arg =>
                {
                    clientContext.Dispose();
                    _clientContexts.Remove(clientContext);
                    this.Children.Remove(viewModel);
                }, null)
            });
        }

        public void Dispose()
        {
            foreach (SPClientContext clientContext in _clientContexts)
            {
                if (clientContext != null)
                {
                    clientContext.Dispose();
                    _clientContexts.Remove(clientContext);
                }
            }
        }
    }
}