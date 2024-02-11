using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
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

        private string _message;

        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
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

        private string _userColor;

        public string UserColor
        {
            get { return _userColor; }
            set
            {
                _userColor = value;
                OnPropertyChanged();
            }
        }

        private UserContext _UserContext => new(Username, _userColor);
        public ObservableCollection<Message> Messages { get; } = new();
        public ObservableCollection<User> Users { get; } = new();

        #region commands

        public RelayCommand SubmitUsernameChange { get; }

        public RelayCommand CancellUsernameChange { get; }

        public AsyncRelayCommand ConnectToServer { get; }
        public AsyncRelayCommand SendMessage { get; }

        #endregion commands

        public ChatViewModel(NavigationStore navigationStore, CurrentUserStore userStore)
            : base(navigationStore)
        {
            this._server = new Server();
            this._server.OnMessageRecived += OnMessageRecived;
            this._message = string.Empty;
            this._userStore = userStore;
            this._usernameInput = userStore.Username;
            this._userColor = GenerateRandomColorAsString();
            this.SubmitUsernameChange = new RelayCommand(ChangeUsername, IsValidUserName);
            this.CancellUsernameChange = new RelayCommand(_ => this.UsernameInput = this.Username);
            this.ConnectToServer = new AsyncRelayCommand(async (token) => await this._server.ConnectToServerAsync(_UserContext, token), IsValidUserName);
            this.SendMessage = new AsyncRelayCommand(async (token) =>
            await this._server.SendMessageAsync(new(Username, Message, UserColor, DateTime.UtcNow), token));
        }
        
        private void OnMessageRecived(int opCode, string? content)
        {
            switch (opCode)
            {
                case 1:
                    if (string.IsNullOrEmpty(content)) return;
                    var users = JsonSerializer.Deserialize<IEnumerable<User>>(content) ?? new List<User>();
                    UpdateUsers(users);
                    break;

                case 2:
                    if (Guid.TryParse(content, out Guid val))
                    {
                        RemoveUser(val);
                    }
                    break;

                case 3:
                    if (string.IsNullOrEmpty(content)) return;
                    var msg = JsonSerializer.Deserialize<Message>(content);
                    if (msg is not null)
                    {
                        AddNewMessage(msg);
                    }
                    break;

                default:
                    break;
            }
        }

        private void UpdateUsers(IEnumerable<User> users)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (var user in users)
                {
                    if (!this.Users.Contains(user))
                    {
                        this.Users.Add(user);
                    }
                }
            });
        }

        private void RemoveUser(Guid userid)
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

        private void AddNewMessage(Message message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                this.Messages.Add(message);
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

        private static readonly Random random = new();

        public static string GenerateRandomColorAsString()
        {
            byte[] rgb = new byte[3];
            random.NextBytes(rgb);
            return Color.FromArgb(255, rgb[0], rgb[1], rgb[2]).ToString();
        }
    }
}