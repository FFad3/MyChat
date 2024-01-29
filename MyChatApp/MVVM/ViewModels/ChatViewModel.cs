using System.Windows.Input;
using MyChatApp.MVVM.Commands;
using MyChatApp.MVVM.Models;

namespace MyChatApp.MVVM.ViewModels
{
    internal class ChatViewModel : ViewModelBase
    {
        private readonly User _user;

        private string _username;

        public string Username
        {
            get { return _username; }
            set
            {
                _user.Username = value;
                _username = value;
                OnPropertyChanged();
            }
        }

        private string _usernameInput;

        public string UsernameInput
        {
            get { return _usernameInput; }
            set
            {
                _usernameInput = value;
                OnPropertyChanged();
            }
        }

        public ICommand SubmitUsernameChange { get; }
        public ICommand CancellUsernameChange { get; }

        public ChatViewModel(User user)
        {
            this._user = user;
            this._username = user.Username;
            this._usernameInput = user.Username;
            this.SubmitUsernameChange = new ChangeUsernameCommand(this);
            this.CancellUsernameChange = new CancellUsernameChangeCommand(this);
        }
    }
}