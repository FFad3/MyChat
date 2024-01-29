using System;

namespace MyChatApp.MVVM.Models
{
    public class User
    {
        public string Username { get; set; }

        public User()
        {
            Username = Guid.NewGuid().ToString()[..8];
        }
    }
}