using System;
using MyChatApp.Stores;

namespace MyChatApp.MVVM.ViewModels
{
    internal class MainViewModel : ViewModelBase
    {
        public ViewModelBase CurrentViewModel => _navigationStore.CurrentViewModel
            ?? throw new ArgumentNullException("unspecified view");

        public MainViewModel(NavigationStore navigationStore) : base(navigationStore)
        {
        }
    }
}