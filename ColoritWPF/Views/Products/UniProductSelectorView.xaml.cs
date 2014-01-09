using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ColoritWPF.ViewModel.Products;
namespace ColoritWPF.Views.Products
{
    /// <summary>
    /// Interaction logic for ProductsSelectorView.xaml
    /// </summary>
    public partial class UniProductSelectorView : Window
    {
        public UniProductSelectorView()
        {
            InitializeComponent();
        }

        private void ProductsGrid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var viewModel = this.DataContext as UniProductSelectorViewModel;
            viewModel.AddProductToListCommand.Execute(null);
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = this.DataContext as UniProductSelectorViewModel;
            viewModel.SelectedProducts.Clear();
            Close();
        }

        private void AcceptBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void dg_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }
}
