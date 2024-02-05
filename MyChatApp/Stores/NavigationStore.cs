using MyChatApp.MVVM.ViewModels;

namespace MyChatApp.Stores
{
    internal sealed class NavigationStore
    {
        public ViewModelBase CurrentViewModel { get; private set; }

        public NavigationStore(ViewModelBase initVM)
        {
            CurrentViewModel = initVM;
        }

        public void ChangeView(ViewModelBase VM)
        {
            CurrentViewModel = VM;
        }
    }
}
