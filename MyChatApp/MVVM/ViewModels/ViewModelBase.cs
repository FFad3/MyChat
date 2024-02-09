using MyChatApp.MVVM.Core;
using MyChatApp.Stores;

namespace MyChatApp.MVVM.ViewModels
{
    internal abstract class ViewModelBase : ObservableObject
    {
        protected readonly NavigationStore _navigationStore;

        protected ViewModelBase(NavigationStore navigationStore)
        {
            _navigationStore = navigationStore;
        }
    }
}