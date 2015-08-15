using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media;
using SharePoint.Remote.Access.Helpers;
using SP2013Access.Commands;

//using GalaSoft.MvvmLight;

namespace SP2013Access.ViewModels
{
    /// <summary>
    ///     Base class for all ViewModel classes displayed by TreeViewItems.
    ///     This acts as an adapter between a raw data object and a TreeViewItem.
    /// </summary>
    public class TreeViewItemViewModel : /*ViewModelBase,*/ ITreeViewItemViewModel
    {
        #region Fields

        private static readonly TreeViewItemViewModel DummyChild = new TreeViewItemViewModel();
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
            Parent = parent;
            _lazyLoadChildren = lazyLoadChildren;

            if (_lazyLoadChildren)
            {
                Children.Add(DummyChild);
            }

            if (Parent != null)
            {
                FailEvent = Parent.FailEvent;
                SuccessEvent = Parent.SuccessEvent;
            }
        }

        #endregion Constructors

        #region Properties

        public TreeViewItemViewModel Parent { get; }

        public virtual string Name
        {
            get { return _name; }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    OnPropertyChanged("Name");
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
                    OnPropertyChanged("IsBusy");
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
                    OnPropertyChanged("IsLoaded");
                    //this.RaisePropertyChanged("IsLoaded");
                }
            }
        }

        /// <summary>
        ///     Returns the logical child items of this object.
        /// </summary>
        public ObservableCollection<TreeViewItemViewModel> Children { get; }

        /// <summary>
        ///     Returns true if this object's Children have not yet been populated.
        /// </summary>
        public bool HasDummyChild => Children.Count == 1 && Children[0] == DummyChild;

        public bool HasChildren => !HasDummyChild && Children.Count > 0;

        /// <summary>
        ///     Gets/sets whether the TreeViewItem
        ///     associated with this object is expanded.
        /// </summary>
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (value != _isExpanded)
                {
                    _isExpanded = value;
                    OnPropertyChanged("IsExpanded");
                    //this.RaisePropertyChanged("IsExpanded");
                }

                if (_isExpanded)
                {
                    if (HasDummyChild)
                    {
                        Children.Remove(DummyChild);
                    }
                    if (!IsLoaded && !IsBusy)
                    {
                        LoadChildren();
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
                OnPropertyChanged("IsDirty");
                //this.RaisePropertyChanged("IsDirty");
                if (!_isdirty) return;
                if (HasDummyChild) return;
                if (Children.Count > 0)
                {
                    Children.Clear();
                }
                if (_lazyLoadChildren)
                {
                    Children.Add(DummyChild);
                }
            }
        }

        /// <summary>
        ///     Gets/sets whether the TreeViewItem
        ///     associated with this object is selected.
        /// </summary>
        public virtual bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                    OnPropertyChanged("IsSelected");
                    //this.RaisePropertyChanged("IsSelected");
                }
            }
        }

        private ImageSource _imageSource;
        private string _name;

        public virtual ImageSource ImageSource
        {
            get { return _imageSource; }
            protected set
            {
                _imageSource = value;
                OnPropertyChanged("ImageSource");
            }
        }

        public ObservableCollection<CommandEntity> Commands { get; }

        public RelayCommand<Exception> OnExceptionCommand { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        ///     Invoked when the child items need to be loaded on demand.
        ///     Subclasses can override this to populate the Children collection.
        /// </summary>
        protected virtual void LoadChildren()
        {
            IsBusy = true;
            IsLoaded = false;

            if (_lazyLoadChildren)
            {
                var promise = LoadChildrenAsync();
                promise.Done(o =>
                {
                    foreach (var child in Children.OrderBy(c => c.Name))
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
                foreach (var child in Children)
                {
                    if (!child._lazyLoadChildren && !child.IsLoaded)
                    {
                        child.LoadChildren();
                    }
                }
                IsBusy = false;
                IsLoaded = true;
                IsDirty = false;
            }
        }

        protected virtual IPromise<object, Exception> LoadChildrenAsync()
        {
            throw new NotImplementedException();
        }

        public virtual void Refresh()
        {
            IsDirty = true;
            IsExpanded = !_lazyLoadChildren;
            if (IsLoaded)
            {
                IsLoaded = false;
            }
        }

        public event EventHandler<ResultEventArgs> FailEvent;

        protected void OnFail(Exception ex)
        {
            IsBusy = false;
            var handler = FailEvent;
            handler?.Invoke(this, new ResultEventArgs {Exception = ex});
        }

        public event EventHandler<ResultEventArgs> SuccessEvent;

        protected void OnSuccess(object obj)
        {
            IsBusy = false;
            IsLoaded = true;
            IsDirty = false;
            var handler = SuccessEvent;
            handler?.Invoke(this, new ResultEventArgs {Source = obj});
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged Members

        public override string ToString()
        {
            return $"ID: {ID}, Name: {Name}";
        }

        #endregion Methods
    }

    public class ResultEventArgs : EventArgs
    {
        public Exception Exception;
        public object Source;
    }
}