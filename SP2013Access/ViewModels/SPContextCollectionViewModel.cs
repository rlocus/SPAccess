﻿using SharePoint.Remote.Access.Helpers;
using SP2013Access.Commands;
using System;
using System.Collections.Generic;

namespace SP2013Access.ViewModels
{
    public class SPContextCollectionViewModel : TreeViewItemViewModel
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

        public override void LoadChildren()
        {
            foreach (SPClientContext clientContext in _clientContexts)
            {
                var viewModel = new SPSiteViewModel(clientContext.ClientSite);
                this.Children.Add(viewModel);
                viewModel.LoadChildren();
            } 
            base.LoadChildren();
        }

        public void Add(SPClientContext clientContext)
        {
            _clientContexts.Add(clientContext);
            var viewModel = new SPSiteViewModel(clientContext.ClientSite);
            this.Children.Add(viewModel);
            viewModel.LoadChildren();
            viewModel.IsExpanded = true;
            viewModel.Commands.Add(new CommandEntity()
            {
                Name = "Close",
                Command = new DelegateCommand<object>(arg =>
                {
                    clientContext.Dispose();
                    this.Children.Remove(viewModel);
                }, null)
            });
        }

        public override void Refresh()
        {
            base.Refresh();
            base.IsExpanded = true;
        }
    }
}