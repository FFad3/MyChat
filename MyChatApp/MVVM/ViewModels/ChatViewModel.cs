using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using MyChatApp.MVVM.Core;
using MyChatApp.MVVM.Models;
using MyChatApp.Net;
using MyChatApp.Stores;

namespace MyChatApp.MVVM.ViewModels
{
    internal class ChatViewModel : ViewModelBase
    {
        private readonly CurrentUserStore _user;
        private readonly Server _server;

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
                OnPropertyChanged(nameof(Messages));
            }
        }

        private ObservableCollection<Message> _messages { get; }
        public IEnumerable<Message> Messages => _messages.ToList();

        #region commands

        public RelayCommand SubmitUsernameChange { get; }

        public RelayCommand CancellUsernameChange { get; }

        //public RelayCommand ConnectToServer { get; }

        #endregion commands

        public ChatViewModel(CurrentUserStore user)
        {
            this._server = new Server();
            this._messages = new ObservableCollection<Message>()
            {
                new("User1","random message 1 from user1",DateTime.UtcNow),
                new("User2","random message 2 from user2",DateTime.UtcNow),
                new("User3","random message 3 from user3",DateTime.UtcNow),
                new("User4","random message 4 from user4",DateTime.UtcNow),
                new("User5","random message 5 from user5",DateTime.UtcNow),
            };
            this._user = user;
            this._username = user.Username;
            this._usernameInput = user.Username;
            this.SubmitUsernameChange = new RelayCommand(ChangeUsername, CanChangeUsername);
            this.CancellUsernameChange = new RelayCommand(_ => this.UsernameInput = this.Username);
            //this.ConnectToServer = new AsyncRelayCommand();
        }

        #region UserName

        private bool CanChangeUsername()
        {
            return !string.IsNullOrEmpty(UsernameInput) && UsernameInput.Length is not < 4 and not > 10;
        }

        private void ChangeUsername(object obj)
        {
            Username = UsernameInput;
        }

        #endregion UserName
    }

    public class MultivalueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Count() >= 2)
            {
                if (string.Equals(values[0], values[1]))
                    return false;
                else return true;
            }
            else
                return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}