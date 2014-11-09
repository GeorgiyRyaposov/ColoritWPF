using System.Windows;
using ColoritWPF.ViewModel;

namespace ColoritWPF.Views
{
    /// <summary>
    /// Interaction logic for ProducerEditorWindow.xaml
    /// </summary>
    public partial class ProducerEditorWindow : Window
    {
        public ProducerEditorWindow()
        {
            InitializeComponent();
        }

        private void Close_Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
