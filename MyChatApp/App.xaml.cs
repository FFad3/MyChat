using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MyChatApp.MVVM.Models;
using MyChatApp.MVVM.ViewModels;

namespace MyChatApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly User _user;
        public App()
        {
            _user = new User();
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
                DataContext = new MainViewModel(_user)
            };

            window.Show();

            base.OnStartup(e);
        }
    }
}
