using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace SP2013Access.Commands
{
    public interface IRelayCommand : ICommand
    {
        bool IsAutomaticRequeryDisabled { get; set; }

        void RaiseCanExecuteChanged();
    }

    public class RelayCommand<T> : IRelayCommand
    {
        #region Fields

        private readonly Predicate<T> _canExecute;
        private readonly Action<T> _execute;
        private List<WeakReference> _canExecuteChangedHandlers;
        private bool _isAutomaticRequeryDisabled;

        #endregion Fields

        #region Constructors

        public RelayCommand(Action<T> execute)
            : this(execute, null, false) { }

        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
            : this(execute, canExecute, false) { }

        public RelayCommand(Action<T> execute, Predicate<T> canExecute, bool isAutomaticRequeryDisabled)
        {
            _execute = execute;
            _canExecute = canExecute;
            _isAutomaticRequeryDisabled = isAutomaticRequeryDisabled;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        ///     Property to enable or disable CommandManager's automatic requery on this command
        /// </summary>
        public bool IsAutomaticRequeryDisabled
        {
            get
            {
                return _isAutomaticRequeryDisabled;
            }
            set
            {
                if (_isAutomaticRequeryDisabled != value)
                {
                    if (value)
                    {
                        WeakRefEventManager.RemoveHandlersFromRequerySuggested(_canExecuteChangedHandlers);
                    }
                    else
                    {
                        WeakRefEventManager.AddHandlersToRequerySuggested(_canExecuteChangedHandlers);
                    }
                    _isAutomaticRequeryDisabled = value;
                }
            }
        }

        #endregion Properties

        #region ICommand implementation

        public virtual bool CanExecute(object parameter)
        {
            if (_canExecute == null)
            {
                return true;
            }
            return _canExecute((T)parameter);
        }

        public virtual void Execute(object parameter)
        {
            if (_execute != null && CanExecute(parameter))
            {
                _execute((T)parameter);
            }
        }

        /// <summary>
        ///     ICommand.CanExecuteChanged implementation
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (!_isAutomaticRequeryDisabled)
                {
                    CommandManager.RequerySuggested += value;
                }
                WeakRefEventManager.AddWeakReferenceHandler(ref _canExecuteChangedHandlers, value, 2);
            }
            remove
            {
                if (!_isAutomaticRequeryDisabled)
                {
                    CommandManager.RequerySuggested -= value;
                }
                WeakRefEventManager.RemoveWeakReferenceHandler(_canExecuteChangedHandlers, value);
            }
        }

        #endregion ICommand implementation

        #region Methods

        public virtual void RaiseCanExecuteChanged()
        {
            WeakRefEventManager.CallWeakReferenceHandlers(_canExecuteChangedHandlers);
        }

        #endregion Methods
    }
}