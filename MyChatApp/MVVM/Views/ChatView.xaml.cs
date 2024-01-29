using System.Windows;
using System.Windows.Controls;

namespace MyChatApp.MVVM.Views
{
    /// <summary>
    /// Interaction logic for ChatView.xaml
    /// </summary>
    public partial class ChatView : UserControl
    {
        public ChatView()
        {
            InitializeComponent();
        }

        private void BtnShowUsernameChangeForm_Click(object sender, RoutedEventArgs e)
        {
            if (this.UsernameChangeForm.Visibility == Visibility.Visible)
            {
                this.UsernameChangeForm.Visibility = Visibility.Hidden;
            }
            else
            {
                this.UsernameChangeForm.Visibility = Visibility.Visible;
            }
        }
    }
}