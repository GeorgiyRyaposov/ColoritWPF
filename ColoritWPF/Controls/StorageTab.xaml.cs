using System.Windows.Controls;
using ColoritWPF.ViewModel.Products;

namespace ColoritWPF.Controls
{
    /// <summary>
    /// Interaction logic for StorageTab.xaml
    /// </summary>
    public partial class StorageTab : UserControl
    {
        public StorageTab()
        {
            InitializeComponent();
            this.DataContext = new ProductsViewModel();
        }

        private void dgSaleList_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }
}
