using System;
using System.Windows.Media;

namespace MyChatApp.MVVM.Models
{
    public sealed record Message(
        SolidColorBrush Color,
        string Sender,
        string Content,
        DateTime Date);
}