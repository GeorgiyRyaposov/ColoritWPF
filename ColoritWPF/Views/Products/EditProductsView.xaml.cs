using System.Windows;
using System.Windows.Controls;

namespace ColoritWPF.Views.Products
{
    /// <summary>
    /// Interaction logic for EditProductsView.xaml
    /// </summary>
    public partial class EditProductsView : Window
    {
        public EditProductsView()
        {
            InitializeComponent();
        }

        private void dg_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
