using System;
using System.Windows.Input;

namespace MyChatApp.MVVM.Core
{
    internal class RelayCommand : ICommand
    {
        //TODO: Read about RelayCommand
        //public event EventHandler? CanExecuteChanged;
        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        private readonly Action<object> _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action<object> execute, Func<bool> canExecute = null!)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            return _canExecute == null || this._canExecute();
        }

        public void Execute(object? parameter)
        {
            this._execute(parameter!);
        }
    }
}