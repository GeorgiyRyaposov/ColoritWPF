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
using System.Collections.ObjectModel;


namespace ColoritWPF
{
    /// <summary>
    /// Interaction logic for ProductSelection.xaml
    /// </summary>
    public partial class ProductSelection : Window
    {
        ListOfProductsForSale listOfPrs;

        public ListOfProductsForSale ListOfPrs
        {
            get { return listOfPrs; }
        }

        ProductsForSale prForSale;

        public ProductSelection()
        {
            InitializeComponent();
            listOfPrs = new ListOfProductsForSale();
        }

        private System.Data.Objects.ObjectQuery<Product> GetProductQuery(ColorITEntities colorITEntities)
        {
            // Auto generated code

            System.Data.Objects.ObjectQuery<ColoritWPF.Product> productQuery = colorITEntities.Product;
            // To explicitly load data, you may need to add Include methods like below:
            // productQuery = productQuery.Include("Product.Group1").
            // For more information, please see http://go.microsoft.com/fwlink/?LinkId=157380
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

            //Группировка
            ProductGroupConverter prGrConv = new ProductGroupConverter();
            productViewSource.GroupDescriptions.Add(new PropertyGroupDescription("Group", prGrConv));
        }

        private bool CheckRepeatableProducts(ProductsForSale prForSale)
        {
            foreach (ProductsForSale pr in listOfPrs)
            {
                if (pr.Id == prForSale.Id)
                {
                    pr.Amount = pr.Amount + prForSale.Amount;
                    return true;
                }
            }
            return false;
        }

        private void AddNewPrForSale()
        {
            
            prForSale = new ProductsForSale();
            prForSale.Id = ((Product)dg_Products.SelectedItem).ID;
            prForSale.Name = ((Product)dg_Products.SelectedItem).Name;
            prForSale.SelfCost = ((Product)dg_Products.SelectedItem).SelfCost;
            prForSale.Cost = ((Product)dg_Products.SelectedItem).Cost;
            prForSale.Warehouse = ((Product)dg_Products.SelectedItem).Warehouse;
            prForSale.Storage = ((Product)dg_Products.SelectedItem).Storage;
            prForSale.Bottled = ((Product)dg_Products.SelectedItem).Bottled;
            prForSale.MaxDiscount = ((Product)dg_Products.SelectedItem).MaxDiscount;
            prForSale.ClientDiscount = 0;

            if(!CheckRepeatableProducts(prForSale))
                listOfPrs.Add(prForSale);
        }

        private void productDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            AddNewPrForSale();
            dg_SelectedProducts.ItemsSource = listOfPrs;
        }

        private void btn_Finish_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
