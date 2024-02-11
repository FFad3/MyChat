using System;

namespace MyChatApp.MVVM.Models
{
    public sealed record Message(
        string Sender,
        string Content,
        string Color,
        DateTime Date);
}