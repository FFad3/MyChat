using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using MyChatApp.MVVM.Core;
using MyChatApp.MVVM.Models;
using MyChatApp.Net;
using MyChatApp.Stores;

namespace MyChatApp.MVVM.ViewModels
{
    internal class ChatViewModel : ViewModelBase
    {
        private readonly CurrentUserStore _userStore;
        private readonly Server _server;

        public string Username
        {
            get { return _userStore.Username; }
            set
            {
                _userStore.Username = value;
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

        public ObservableCollection<User> Users { get; } = new();

        #region commands

        public RelayCommand SubmitUsernameChange { get; }

        public RelayCommand CancellUsernameChange { get; }

        public AsyncRelayCommand ConnectToServer { get; }

        #endregion commands

        public ChatViewModel(NavigationStore navigationStore, CurrentUserStore userStore)
            : base(navigationStore)
        {
            this._server = new Server();
            this._server.ConnectedEvent += OnUserConnected;
            this._server.DisconectedEvent += OnUserDisconected;
            this._messages = new ObservableCollection<Message>()
            {
                new(new(Colors.Yellow),"User1","random message 1 from user1",DateTime.UtcNow),
                new(new(Colors.Red),"User2","random message 2 from user2",DateTime.UtcNow),
            };
            this._userStore = userStore;
            this._usernameInput = userStore.Username;
            this.SubmitUsernameChange = new RelayCommand(ChangeUsername, IsValidUserName);
            this.CancellUsernameChange = new RelayCommand(_ => this.UsernameInput = this.Username);
            this.ConnectToServer = new AsyncRelayCommand(async (token) => await this._server.ConnectToServer(Username, token), IsValidUserName);
        }

        private void OnUserConnected(IEnumerable<User> users)
        {
            foreach (var user in users)
            {
                if (!this.Users.Contains(user))
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        this.Users.Add(user);
                    });
                }
            }
        }

        private void OnUserDisconected(Guid userid)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var user = this.Users.FirstOrDefault(x => x.Id == userid);
                if (user is not null)
                {
                    this.Users.Remove(user);
                }
            });
        }

        #region UserName

        private bool IsValidUserName()
        {
            return !string.IsNullOrEmpty(UsernameInput) && UsernameInput.Length is not < 4 and not > 10;
        }

        private void ChangeUsername(object obj)
        {
            Username = UsernameInput;
        }

        #endregion UserName
    }
}