using MyChatApp.MVVM.ViewModels;

namespace MyChatApp.Stores
{
    internal sealed class NavigationStore
    {
        public ViewModelBase? CurrentViewModel { get; set; }
    }
}
