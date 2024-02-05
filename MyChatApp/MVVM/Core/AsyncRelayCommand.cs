using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MyChatApp.MVVM.Core
{
    internal class AsyncRelayCommand : IAsycRelayCommand
    {
        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        private readonly Func<CancellationToken, Task> _execute;
        private readonly Func<bool> _canExecute;
        private CancellationTokenSource? _cts;

        public AsyncRelayCommand(Func<CancellationToken, Task> execute, Func<bool> canExecute = null!)
        {
            _isExecuting = false;
            _execute = execute;
            _canExecute = canExecute;
        }

        private bool _isExecuting;

        public bool IsExecuting
        {
            get { return _isExecuting; }
            private set
            {
                if (_isExecuting != value)
                {
                    _isExecuting = value;
                    CommandManager.InvalidateRequerySuggested(); // Notify UI that CanExecute might have changed
                }
            }
        }

        public void Cancel()
        {
            _cts?.Cancel();
        }

        public bool CanExecute(object? parameter)
        {
            return !_isExecuting && (_canExecute == null || this._canExecute());
        }

        public async void Execute(object? parameter)
        {
            if (!CanExecute(parameter)) return;

            try
            {
                IsExecuting = true;
                _cts = new CancellationTokenSource();
                await ExecuteAsync(_cts.Token);
            }
            finally
            {
                IsExecuting = false;
                CommandManager.InvalidateRequerySuggested(); // Notify UI that CanExecute might have changed
            }
        }

        public async Task ExecuteAsync(CancellationToken token)
        {
            try
            {
                await _execute(token);
            }
            catch (OperationCanceledException ex)
            {
                Debug.WriteLine(ex);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}