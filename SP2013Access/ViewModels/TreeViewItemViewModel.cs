using SP2013Access.Commands;
using System.Collections.ObjectModel;
using System.ComponentModel;

//using GalaSoft.MvvmLight;
using System.Windows.Media;

namespace SP2013Access.ViewModels
{
    /// <summary>
    /// Base class for all ViewModel classes displayed by TreeViewItems.
    /// This acts as an adapter between a raw data object and a TreeViewItem.
    /// </summary>
    public class TreeViewItemViewModel : /*ViewModelBase,*/ ITreeViewItemViewModel
    {
        #region Fields

        private static readonly TreeViewItemViewModel DummyChild = new TreeViewItemViewModel();
        private readonly TreeViewItemViewModel _parent;

        private bool _isExpanded;
        private bool _isSelected;
        private bool _isBusy;
        private bool _isLoaded;
        private bool _isdirty;

        #endregion Fields

        #region Constructors

        // This is used to create the DummyChild instance.
        private TreeViewItemViewModel()
        {
            Children = new AsyncObservableCollection<TreeViewItemViewModel>();
            Commands = new AsyncObservableCollection<CommandEntity>();
        }

        protected TreeViewItemViewModel(TreeViewItemViewModel parent, bool lazyLoadChildren)
            : this()
        {
            _parent = parent;

            if (lazyLoadChildren)
                Children.Add(DummyChild);
        }

        #endregion Constructors

        #region Properties

        public TreeViewItemViewModel Parent
        {
            get { return _parent; }
        }

        public virtual string Name { get; set; }

        public virtual string ID { get; set; }

        public bool IsBusy
        {
            get { return _isBusy; }
            protected set
            {
                if (value != _isBusy)
                {
                    _isBusy = value;
                    this.OnPropertyChanged("IsBusy");
                    //this.RaisePropertyChanged("IsBusy");
                }
            }
        }

        public bool IsLoaded
        {
            get { return _isLoaded; }
            protected set
            {
                if (value != _isLoaded)
                {
                    _isLoaded = value;
                    this.OnPropertyChanged("IsLoaded");
                    //this.RaisePropertyChanged("IsLoaded");
                }
            }
        }

        /// <summary>
        /// Returns the logical child items of this object.
        /// </summary>
        public ObservableCollection<TreeViewItemViewModel> Children { get; private set; }

        /// <summary>
        /// Returns true if this object's Children have not yet been populated.
        /// </summary>
        public bool HasDummyChild
        {
            get { return this.Children.Count == 1 && this.Children[0] == DummyChild; }
        }

        public bool HasChildren
        {
            get { return !HasDummyChild && this.Children.Count > 0; }
        }

        /// <summary>
        /// Gets/sets whether the TreeViewItem
        /// associated with this object is expanded.
        /// </summary>
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (value != _isExpanded)
                {
                    _isExpanded = value;
                    this.OnPropertyChanged("IsExpanded");
                    //this.RaisePropertyChanged("IsExpanded");
                }

                // Expand all the way up to the root.
                if (_isExpanded && _parent != null)
                    _parent.IsExpanded = true;

                // Lazy load the child items, if necessary.
                if (this.HasDummyChild)
                {
                    this.Children.Remove(DummyChild);
                    this.LoadChildren();
                }

                if (this.IsDirty)
                {
                    this.Children.Clear();
                    this.LoadChildren();
                }
            }
        }

        public bool IsDirty
        {
            get { return _isdirty; }
            set
            {
                if (value != _isdirty)
                {
                    _isdirty = value;
                    if (this._isdirty)
                    {
                        this.Children.Clear();
                        this.Children.Add(DummyChild);
                    }
                    this.OnPropertyChanged("IsDirty");
                    //this.RaisePropertyChanged("IsDirty");
                }
            }
        }

        /// <summary>
        /// Gets/sets whether the TreeViewItem
        /// associated with this object is selected.
        /// </summary>
        public virtual bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                    this.OnPropertyChanged("IsSelected");
                    //this.RaisePropertyChanged("IsSelected");
                }
            }
        }

        private ImageSource _imageSource;

        public virtual ImageSource ImageSource
        {
            get
            {
                return _imageSource;
            }
            protected set
            {
                _imageSource = value;
                this.OnPropertyChanged("ImageSource");
            }
        }

        public ObservableCollection<CommandEntity> Commands { get; private set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Invoked when the child items need to be loaded on demand.
        /// Subclasses can override this to populate the Children collection.
        /// </summary>
        public virtual void LoadChildren()
        {
            this.IsBusy = true;
            this.IsLoaded = false;

            //if (!this.HasDummyChild)
            //{
            //    if (this.Children.Count > 0)
            //    {
            //        this.Children.Clear();
            //    }
            //}
        }

        public virtual void Refresh()
        {
            this.IsBusy = true;
            this.IsLoaded = false;

            if (!this.HasDummyChild)
            {
                if (this.Children.Count > 0)
                {
                    this.Children.Clear();
                }
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged Members

        public override string ToString()
        {
            return string.Format("ID: {0}, Name: {1}", this.ID, this.Name);
        }

        #endregion Methods
    }
}