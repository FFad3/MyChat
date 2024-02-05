using MyChatApp.Stores;

namespace MyChatApp.MVVM.ViewModels
{
    internal class MainViewModel : ViewModelBase
    {
        private readonly NavigationStore _navigationStore;
        private readonly CurrentUserStore _userStore;
        public ViewModelBase CurrentViewModel => _navigationStore.CurrentViewModel;

        public MainViewModel()
        {
            _userStore = new CurrentUserStore();
            _navigationStore = new NavigationStore(new ChatViewModel(_userStore));
        }
    }
}