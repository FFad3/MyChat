using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyChatApp.MVVM.ViewModels;

namespace MyChatApp.MVVM.Commands
{
    internal sealed class CancellUsernameChangeCommand : CommandBase
    {
        private readonly ChatViewModel _viewModel;

        public CancellUsernameChangeCommand(ChatViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public override void Execute(object? parameter)
        {
            _viewModel.UsernameInput = _viewModel.Username;
        }
    }
}
