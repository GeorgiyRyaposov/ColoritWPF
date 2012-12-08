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
    /// Interaction logic for AddProduct.xaml
    /// </summary>
    public partial class AddProduct : Window
    {
        public AddProduct()
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
            // Load data into Group. You can modify this code as needed.
            System.Windows.Data.CollectionViewSource groupViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("groupViewSource")));
            System.Data.Objects.ObjectQuery<ColoritWPF.Group> groupQuery = this.GetGroupQuery(colorITEntities);
            groupViewSource.Source = groupQuery.Execute(System.Data.Objects.MergeOption.AppendOnly);
        }

        private void btn_Add_Click(object sender, RoutedEventArgs e)
        {
            decimal selfCost = decimal.Parse(txtbx_SelfCost.Text.ToString(), NumberStyles.AllowDecimalPoint);
            decimal cost = decimal.Parse(txtbx_Cost.Text.ToString(), NumberStyles.AllowDecimalPoint);
            double toWarehouse = double.Parse(txtbx_Warehouse.Text.ToString(), NumberStyles.AllowDecimalPoint);
            double toStorage = double.Parse(txtbx_Storage.Text.ToString(), NumberStyles.AllowDecimalPoint);
            double maxDiscount = double.Parse(txtbx_MaxDiscount.Text.ToString(), NumberStyles.AllowDecimalPoint);

            using (ColorITEntities ColorItEnt = new ColorITEntities())
            {
                Product pr = new Product();
                pr.Name = txtbx_Name.Text.ToString();
                pr.SelfCost = selfCost;
                pr.Cost = cost;
                pr.Warehouse = toWarehouse;
                pr.Storage = toStorage;
                pr.MaxDiscount = maxDiscount;
                pr.Bottled = chbx_Bottled.IsChecked.Value;
                pr.Group = ((Group)cmbx_Group.SelectedItem).ID;

                try
                {
                    ColorItEnt.AddToProduct(pr);
                    ColorItEnt.SaveChanges();

                    MessageBox.Show(String.Format("Товар:\n{0}\nуспешно добавлен", pr.Name), "Сохранено", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtbx_Cost.Text = String.Empty;
                    txtbx_MaxDiscount.Text = String.Empty;
                    txtbx_Name.Text = String.Empty;
                    txtbx_SelfCost.Text = String.Empty; ;
                    txtbx_Storage.Text = String.Empty;
                    txtbx_Warehouse.Text = String.Empty;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private System.Data.Objects.ObjectQuery<Group> GetGroupQuery(ColorITEntities colorITEntities)
        {
            // Auto generated code

            System.Data.Objects.ObjectQuery<ColoritWPF.Group> groupQuery = colorITEntities.Group;
            // Returns an ObjectQuery.
            return groupQuery;
        }
    }
}
