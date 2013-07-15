using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
//using Excel = Microsoft.Office.Interop.Excel;

namespace ColoritWPF
{
    /// <summary>
    /// Interaction logic for SaleProduct.xaml
    /// </summary>
    public partial class SaleProduct : Window
    {
        ListOfProductsForSale forSale;

        double _clientDiscount;

        public double ClientDiscount
        {
            get { return (double)((Client)clientComboBox.SelectedItem).Discount; }
        }
        public SaleProduct()
        {
            InitializeComponent();
            forSale = new ListOfProductsForSale();
            dgv_ForSale.ItemsSource = forSale;
        }

        private System.Data.Objects.ObjectQuery<Product> GetProductQuery(ColorITEntities colorITEntities)
        {
            // Auto generated code

            System.Data.Objects.ObjectQuery<ColoritWPF.Product> productQuery = colorITEntities.Product;
            // Returns an ObjectQuery.
            return productQuery;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            ColoritWPF.ColorITEntities colorITEntities = new ColoritWPF.ColorITEntities();
            // Load data into Product. You can modify this code as needed.
            System.Windows.Data.CollectionViewSource productViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("productViewSource")));
            System.Data.Objects.ObjectQuery<ColoritWPF.Product> productQuery = this.GetProductQuery(colorITEntities);
            productViewSource.Source = productQuery.Execute(System.Data.Objects.MergeOption.AppendOnly);

            ProductGroupConverter prGrConv = new ProductGroupConverter();
            productViewSource.GroupDescriptions.Add(new PropertyGroupDescription("Group", prGrConv));

            // Load data into Client. You can modify this code as needed.
            System.Windows.Data.CollectionViewSource clientViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("clientViewSource")));
            System.Data.Objects.ObjectQuery<ColoritWPF.Client> clientQuery = this.GetClientQuery(colorITEntities);
            clientViewSource.Source = clientQuery.Execute(System.Data.Objects.MergeOption.AppendOnly);
        }

        private System.Data.Objects.ObjectQuery<Client> GetClientQuery(ColorITEntities colorITEntities)
        {
            // Auto generated code
            System.Data.Objects.ObjectQuery<ColoritWPF.Client> clientQuery = colorITEntities.Client;
            // Returns an ObjectQuery.
            return clientQuery;
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void dgv_ForSale_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SumCount();
        }

        private void SumCount()
        {
            decimal sum = 0;
            decimal sumWithDiscount = 0;
            foreach (ProductsForSale pr in forSale)
            {
                sum += pr.ProductSum;
                sumWithDiscount += pr.ProductSumWithDiscount;
            }
            txtbx_Sum.Text = sum.ToString();
            txtbx_WIthoutDiscount.Text = sumWithDiscount.ToString();
        }

        private void ClientReselected()
        {
            foreach (ProductsForSale prForS in forSale)
                prForS.ClientDiscount = (double)((Client)clientComboBox.SelectedItem).Discount;
        }

        private void clientComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ClientReselected();
            SumCount();
        }

        private void btn_Sale_Click(object sender, RoutedEventArgs e)
        {
            using (ColorITEntities CIentity = new ColorITEntities())
            {
                for (int i = 0; i < dgv_ForSale.Items.Count; i++)
                {
                    long id = ((ProductsForSale)dgv_ForSale.Items[i]).Id;
                    double currentAmount = ((ProductsForSale)dgv_ForSale.Items[i]).Amount;

                    Product pr = CIentity.Product.First(j => j.ID == id);
                    Sale sale = new Sale();
                    sale.ProductID = id;
                    sale.Amount = Convert.ToDecimal(currentAmount);
                    sale.Discount = ((ProductsForSale)dgv_ForSale.Items[i]).CurrentDiscount;
                    sale.Date = DateTime.Today;
                    sale.ClientID = ((Client)clientComboBox.SelectedItem).ID;

                    if (((ProductsForSale)dgv_ForSale.Items[i]).Storage < currentAmount)
                    {
                        currentAmount = currentAmount - ((ProductsForSale)dgv_ForSale.Items[i]).Storage;
                        sale.FromWareHouse = currentAmount;
                        sale.FromStorage = ((ProductsForSale)dgv_ForSale.Items[i]).Storage;
                        pr.Warehouse = pr.Warehouse - currentAmount;
                        pr.Storage = 0;
                    }
                    else
                    {
                        sale.FromStorage = currentAmount;
                        sale.FromWareHouse = 0;
                        pr.Storage = pr.Storage - currentAmount;
                    }


                    try
                    {
                        CIentity.AddToSale(sale);
                        CIentity.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                    Close();
                    MessageBox.Show("Товар успешно продан", "Успех!");
                }
            }
        }



        private void btn_ExportToExcel_Click(object sender, RoutedEventArgs e)
        {
           //ExportToExcel<ProductsForSale, ListOfProductsForSale> s = new ExportToExcel<ProductsForSale, ListOfProductsForSale>();
           //s.dataToPrint = (ListOfProductsForSale)dgv_ForSale.ItemsSource;
           //s.GenerateReport();
        }

        private bool CheckRepeatableProducts(ProductsForSale prForSale)
        {
            foreach(ProductsForSale pr in forSale)
            {
                if (pr.Id == prForSale.Id)
                {
                    pr.Amount = pr.Amount + prForSale.Amount;
                    return true;
                }
            }
            return false;
        }

        private void btn_SelectProduct_Click(object sender, RoutedEventArgs e)
        {
            ProductSelection prSel = new ProductSelection();
            if (prSel.ShowDialog() == true)
            {
                foreach (ProductsForSale prForSale in prSel.ListOfPrs)
                {
                    if (!CheckRepeatableProducts(prForSale))
                        forSale.Add(prForSale);
                }
                dgv_ForSale.ItemsSource = forSale;
                ClientReselected();
                SumCount();
            }
        }
    }
}
