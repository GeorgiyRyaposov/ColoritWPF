using System;
using System.Windows;
using System.Windows.Controls;

namespace ColoritWPF.Views.Products
{
    /// <summary>
    /// Interaction logic for PurchaseProductView.xaml
    /// </summary>
    public partial class PurchaseProductView : Window
    {
        public PurchaseProductView()
        {
            InitializeComponent();
        }

        private void dgPurchaseList_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
        
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
