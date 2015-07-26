using System;
using System.Linq;
using SharePoint.Remote.Access.Helpers;
using SP2013Access.Commands;
using System.Collections.ObjectModel;
using System.ComponentModel;

//using GalaSoft.MvvmLight;
using System.Windows.Media;
using SP2013Access.Extensions;

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
        private readonly bool _lazyLoadChildren;

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
            Children = new ObservableCollection<TreeViewItemViewModel>();
            Commands = new ObservableCollection<CommandEntity>();
        }

        protected TreeViewItemViewModel(TreeViewItemViewModel parent, bool lazyLoadChildren)
            : this()
        {
            _parent = parent;
            _lazyLoadChildren = lazyLoadChildren;

            if (_lazyLoadChildren)
            {
                Children.Add(DummyChild);
            }

            if (_parent != null)
            {
                this.FailEvent = _parent.FailEvent;
                this.SuccessEvent = _parent.SuccessEvent;
            }
        }

        #endregion Constructors

        #region Properties

        public TreeViewItemViewModel Parent
        {
            get { return _parent; }
        }

        public virtual string Name
        {
            get { return _name; }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    this.OnPropertyChanged("Name");
                    //this.RaisePropertyChanged("Name");
                }
            }
        }

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

                if (_isExpanded)
                {
                    if (HasDummyChild)
                    {
                        this.Children.Remove(DummyChild);
                    }
                    if (!IsLoaded && !IsBusy)
                    {
                        this.LoadChildren();
                    }
                }
            }
        }

        public bool IsDirty
        {
            get { return _isdirty; }
            protected set
            {
                if (value == _isdirty) return;
                _isdirty = value;
                this.OnPropertyChanged("IsDirty");
                //this.RaisePropertyChanged("IsDirty");
                if (!this._isdirty) return;
                if (this.HasDummyChild) return;
                if (this.Children.Count > 0)
                {
                    this.Children.Clear();
                }
                if (_lazyLoadChildren)
                {
                    Children.Add(DummyChild);
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
        private string _name;

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

        public RelayCommand<System.Exception> OnExceptionCommand { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Invoked when the child items need to be loaded on demand.
        /// Subclasses can override this to populate the Children collection.
        /// </summary>
        protected virtual void LoadChildren()
        {
            this.IsBusy = true;
            this.IsLoaded = false;

            if (_lazyLoadChildren)
            {
                var promise = this.LoadChildrenAsync();
                promise.Done(o =>
                {
                    foreach (TreeViewItemViewModel child in this.Children.OrderBy(c => c.Name))
                    {
                        if (!child._lazyLoadChildren && !child.IsLoaded)
                        {
                            child.LoadChildren();
                        }
                    }
                    OnSuccess(o);
                });
                promise.Fail(OnFail);
                //promise.Always(() =>
                //{
                //    this.IsBusy = false;
                //    this.IsLoaded = true;
                //    this.IsDirty = false;
                //});
            }
            else
            {
                foreach (TreeViewItemViewModel child in this.Children)
                {
                    if (!child._lazyLoadChildren && !child.IsLoaded)
                    {
                        child.LoadChildren();
                    }
                }
                this.IsBusy = false;
                this.IsLoaded = true;
                this.IsDirty = false;
            }
        }

        protected virtual IPromise<object, Exception> LoadChildrenAsync()
        {
            throw new NotImplementedException();
        }

        public virtual void Refresh()
        {
            this.IsDirty = true;
            this.IsExpanded = !_lazyLoadChildren;
            if (IsLoaded)
            {
                IsLoaded = false;
            }
        }

        public event EventHandler<ResultEventArgs> FailEvent;

        protected void OnFail(Exception ex)
        {
            this.IsBusy = false;
            EventHandler<ResultEventArgs> handler = FailEvent;
            if (handler != null) handler(this, new ResultEventArgs { Exception = ex });
        }

        public event EventHandler<ResultEventArgs> SuccessEvent;

        protected void OnSuccess(object obj)
        {
            this.IsBusy = false;
            this.IsLoaded = true;
            this.IsDirty = false;
            EventHandler<ResultEventArgs> handler = SuccessEvent;
            if (handler != null) handler(this, new ResultEventArgs { Source = obj });
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

    public class ResultEventArgs : EventArgs
    {
        public object Source;
        public Exception Exception;
    }
}