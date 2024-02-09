using System.Windows;
using MyChatApp.MVVM.ViewModels;
using MyChatApp.Stores;

namespace MyChatApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            FrameworkElement.StyleProperty.OverrideMetadata(typeof(Window), new FrameworkPropertyMetadata
            {
                DefaultValue = FindResource(typeof(Window))
            });
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            //init View
            NavigationStore NavigationStore = new();
            CurrentUserStore currentUserStore = new();
            NavigationStore.CurrentViewModel = new ChatViewModel(NavigationStore, currentUserStore);

            //initialize main widow with new ViewModel
            MainWindow window = new()
            {
                DataContext = new MainViewModel(NavigationStore)
            };

            window.Show();

            base.OnStartup(e);
        }
    }
}