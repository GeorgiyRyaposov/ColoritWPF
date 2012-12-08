using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ColoritWPF
{
    /// <summary>
    /// Interaction logic for SalesStats.xaml
    /// </summary>
    public partial class SalesStats : Window
    {
        public SalesStats()
        {
            InitializeComponent();
            dp_StartDate.Text = DateTime.Today.ToShortDateString();
            dp_EndDate.Text = DateTime.Today.ToShortDateString();
        }

        private System.Data.Objects.ObjectQuery<Sale> GetSaleQuery(ColorITEntities colorITEntities)
        {
            // Auto generated code

            System.Data.Objects.ObjectQuery<ColoritWPF.Sale> saleQuery = colorITEntities.Sale;
            // To explicitly load data, you may need to add Include methods like below:
            // saleQuery = saleQuery.Include("Sale.Client").
            // For more information, please see http://go.microsoft.com/fwlink/?LinkId=157380
            // Returns an ObjectQuery.
            return saleQuery;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            ColoritWPF.ColorITEntities colorITEntities = new ColoritWPF.ColorITEntities();
            // Load data into Sale. You can modify this code as needed.
            System.Windows.Data.CollectionViewSource saleViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("saleViewSource")));
            System.Data.Objects.ObjectQuery<ColoritWPF.Sale> saleQuery = this.GetSaleQuery(colorITEntities);
            saleViewSource.Source = saleQuery.Execute(System.Data.Objects.MergeOption.AppendOnly);
        }

        private void txtbx_Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            using (ColorITEntities CIentity = new ColorITEntities())
            {
                var pr = from products in CIentity.Product
                         where products.Name.Contains(txtbx_Search.Text)
                         select products;
                dgv_SalesStats.DataContext = pr;
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            using (ColorITEntities CIentity = new ColorITEntities())
            {
                var pr = from sold in CIentity.Sale
                         where ((sold.Date >= dp_StartDate.SelectedDate.Value) && (sold.Date <= dp_EndDate.SelectedDate.Value))
                         select sold;
                dgv_SalesStats.DataContext = pr;
            }
        }
    }
}
