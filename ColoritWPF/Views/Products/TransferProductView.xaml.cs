using System.Windows;
using System.Windows.Controls;

namespace ColoritWPF.Views.Products
{
    /// <summary>
    /// Interaction logic for TransferProductView.xaml
    /// </summary>
    public partial class TransferProductView : Window
    {
        public TransferProductView()
        {
            InitializeComponent();
        }

        private void dg_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
