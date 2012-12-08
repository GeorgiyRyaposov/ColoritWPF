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
using System.Globalization;

namespace ColoritWPF
{
    /// <summary>
    /// Interaction logic for BuyProduct.xaml
    /// </summary>
    public partial class BuyProduct : Window
    {
        public BuyProduct()
        {
            InitializeComponent();
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
        }

        private void txtbx_Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            using (ColorITEntities ColorItEnt = new ColorITEntities())
            {
                var pr = from products in ColorItEnt.Product
                         where products.Name.Contains(txtbx_Search.Text)
                         select products;
                dgv_Products.DataContext = pr;
            }
        }

        private void btn_Buy_Click(object sender, RoutedEventArgs e)
        {
            bool flag = true;
            if (txtbx_SelfCost.Text.ToString() == String.Empty)
            {
                
                txtbx_SelfCost.Focus();
                flag = false;
            }

            if (txtbx_Storage.Text.ToString() == String.Empty)
            {
                //txtbx_Storage.BackColor = Color.Red;
                txtbx_Storage.Focus();
                flag = false;
            }

            if (txtbx_Warehouse.Text.ToString() == String.Empty)
            {
                //txtbx_Warehouse.BackColor = Color.Red;
                txtbx_Warehouse.Focus();
                flag = false;
            }

            if (flag)
            {
                decimal selfCost = decimal.Parse(txtbx_SelfCost.Text.ToString(), NumberStyles.AllowDecimalPoint);
                double toWarehouse = double.Parse(txtbx_Warehouse.Text.ToString(), NumberStyles.AllowDecimalPoint);
                double toStorage = double.Parse(txtbx_Storage.Text.ToString(), NumberStyles.AllowDecimalPoint);
                Product selectedPr = (Product)dgv_Products.SelectedItem;

                using (ColorITEntities CIentity = new ColorITEntities())
                {
                    Product pr = CIentity.Product.First(i => i.ID == selectedPr.ID);

                    pr.Warehouse = pr.Warehouse + toWarehouse;
                    pr.Storage = pr.Storage + toStorage;

                    try
                    {
                        CIentity.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }

                using (ColorITEntities CIentity = new ColorITEntities())
                {
                    Purchase purch = new Purchase();
                    purch.ProductID = selectedPr.ID;
                    purch.SelfCost = selfCost;
                    purch.ToWarehouse = toWarehouse;
                    purch.ToStorage = toStorage;
                    purch.Date = DateTime.Today;

                    try
                    {
                        CIentity.AddToPurchase(purch);
                        CIentity.SaveChanges();

                        MessageBox.Show(String.Format("Товар:\n{0}\nуспешно добавлен", selectedPr.Name), "Добавлено", MessageBoxButton.OK, MessageBoxImage.Information);
                        txtbx_SelfCost.Text = String.Empty;
                        txtbx_Storage.Text = String.Empty;
                        txtbx_Warehouse.Text = String.Empty;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
            }
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
