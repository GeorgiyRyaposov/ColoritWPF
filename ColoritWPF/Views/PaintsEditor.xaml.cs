using System.Windows;

namespace ColoritWPF.Views
{
    /// <summary>
    /// Interaction logic for PaintsEditor.xaml
    /// </summary>
    public partial class PaintsEditor : Window
    {
        public PaintsEditor()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
