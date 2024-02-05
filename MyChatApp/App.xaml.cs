using System.Windows;
using MyChatApp.MVVM.ViewModels;

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
            //initialize main widow with new ViewModel
            MainWindow window = new()
            {
                DataContext = new MainViewModel()
            };

            window.Show();

            base.OnStartup(e);
        }
    }
}