using System.Windows;

namespace ColoritWPF.Views.Products
{
    /// <summary>
    /// Interaction logic for AddNewProductView.xaml
    /// </summary>
    public partial class AddNewProductView : Window
    {
        public AddNewProductView()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
