using System.Windows;

namespace ColoritWPF.Views
{
    /// <summary>
    /// Interaction logic for ClientEditor.xaml
    /// </summary>
    public partial class ClientEditor : Window
    {
        public ClientEditor()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
