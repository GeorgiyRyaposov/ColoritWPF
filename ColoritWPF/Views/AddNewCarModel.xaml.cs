using System.Windows;


namespace ColoritWPF.Views
{
    /// <summary>
    /// Interaction logic for AddNewCarModel.xaml
    /// </summary>
    public partial class AddNewCarModel : Window
    {
        public AddNewCarModel()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
