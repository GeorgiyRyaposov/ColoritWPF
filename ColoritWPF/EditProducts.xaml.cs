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
using System.ComponentModel;

namespace ColoritWPF
{
    /// <summary>
    /// Interaction logic for EditProducts.xaml
    /// </summary>
    public partial class EditProducts : Window
    {
        ICollectionView view;
        public BindingListCollectionView View1 { get; set; }

        public EditProducts()
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
            /*
            System.Windows.Data.CollectionViewSource productViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("productViewSource")));
            BindingListCollectionView bcview = (BindingListCollectionView)CollectionViewSource.GetDefaultView(productViewSource);
            if(bcview != null)
                bcview.CustomFilter = txtbx_Search.Text;
             * */
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
