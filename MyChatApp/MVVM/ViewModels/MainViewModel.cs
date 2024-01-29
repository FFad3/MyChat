using MyChatApp.MVVM.Models;

namespace MyChatApp.MVVM.ViewModels
{
    internal class MainViewModel : ViewModelBase
    {
        public ViewModelBase CurrentViewModel { get;}

        public MainViewModel(User user)
        {
            CurrentViewModel = new ChatViewModel(user);
        }
    }
}