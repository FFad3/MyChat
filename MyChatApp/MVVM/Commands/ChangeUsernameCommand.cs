using System.ComponentModel;
using MyChatApp.MVVM.ViewModels;

namespace MyChatApp.MVVM.Commands
{
    internal sealed class ChangeUsernameCommand : CommandBase
    {
        private readonly ChatViewModel _viewModel;

        public ChangeUsernameCommand(ChatViewModel viewModel)
        {
            this._viewModel = viewModel;
            this._viewModel.PropertyChanged += OnUserNameChange;
        }

        private void OnUserNameChange(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_viewModel.UsernameInput))
            {
                OnCanExecuteChanged();
            }
        }

        public override bool CanExecute(object? parameter)
        {
            if (string.IsNullOrEmpty(_viewModel.UsernameInput) || _viewModel.UsernameInput.Length is < 4 or >10) return false;

            return base.CanExecute(parameter);
        }

        public override void Execute(object? parameter)
        {
            _viewModel.Username = _viewModel.UsernameInput;
        }
    }
}