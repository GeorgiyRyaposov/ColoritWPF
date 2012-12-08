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
    /// Interaction logic for ClientList.xaml
    /// </summary>
    public partial class ClientList : Window
    {
        public ClientList()
        {
            InitializeComponent();
        }

        private System.Data.Objects.ObjectQuery<Client> GetClientQuery(ColorITEntities colorITEntities)
        {
            // Auto generated code

            System.Data.Objects.ObjectQuery<ColoritWPF.Client> clientQuery = colorITEntities.Client;
            // Returns an ObjectQuery.
            return clientQuery;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            ColoritWPF.ColorITEntities colorITEntities = new ColoritWPF.ColorITEntities();
            // Load data into Client. You can modify this code as needed.
            System.Windows.Data.CollectionViewSource clientViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("clientViewSource")));
            System.Data.Objects.ObjectQuery<ColoritWPF.Client> clientQuery = this.GetClientQuery(colorITEntities);
            clientViewSource.Source = clientQuery.Execute(System.Data.Objects.MergeOption.AppendOnly);

            // Load data into Product. You can modify this code as needed.
            System.Windows.Data.CollectionViewSource productViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("productViewSource")));
            System.Data.Objects.ObjectQuery<ColoritWPF.Product> productQuery = this.GetProductQuery(colorITEntities);
            productViewSource.Source = productQuery.Execute(System.Data.Objects.MergeOption.AppendOnly);
            
            // Load data into ClientGroups. You can modify this code as needed.
            System.Windows.Data.CollectionViewSource clientGroupsViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("clientGroupsViewSource")));
            System.Data.Objects.ObjectQuery<ColoritWPF.ClientGroups> clientGroupsQuery = this.GetClientGroupsQuery(colorITEntities);
            clientGroupsViewSource.Source = clientGroupsQuery.Execute(System.Data.Objects.MergeOption.AppendOnly);

            ListCollectionView view = CollectionViewSource.GetDefaultView(dgv_Clients.ItemsSource) as ListCollectionView;
            if ((view != null) && !String.IsNullOrEmpty(txtbx_FIO.Text))
            {
                ClientByNameFilter filter = new ClientByNameFilter(txtbx_FIO.Text);
                view.Filter = new Predicate<object>(filter.FilterItem);
            }
            /*
            //Фильтрация для поиска по наименованию товара
            ListCollectionView view = CollectionViewSource.GetDefaultView(dgv_product.ItemsSource) as ListCollectionView;
            if((view != null))// && !String.IsNullOrEmpty(txtbx_Search.Text))
            {
                filter = new ProductByNameFilter(txtbx_Search.Text);
                view.Filter = new Predicate<object>(filter.FilterItem);
            }
             */
            // Load data into Group. You can modify this code as needed.
            System.Windows.Data.CollectionViewSource groupViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("groupViewSource")));
            System.Data.Objects.ObjectQuery<ColoritWPF.Group> groupQuery = this.GetGroupQuery(colorITEntities);
            groupViewSource.Source = groupQuery.Execute(System.Data.Objects.MergeOption.AppendOnly);
        }

        private void GetClients()
        {
            using (ColorITEntities CIentity = new ColorITEntities())
            {
                var cl = from clients in CIentity.Client select clients;
                dgv_Clients.DataContext = cl;
            }
        }

        private void btn_Save_Click(object sender, RoutedEventArgs e)
        {
            Client selectedClient = (Client)dgv_Clients.SelectedItem;

            int id = selectedClient.ID ;
            decimal balance = decimal.Parse(txtbx_Balance.Text.ToString(), NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign);
            double discount = double.Parse(txtbx_Discount.Text.ToString(), NumberStyles.AllowDecimalPoint);

            using (ColorITEntities CIentity = new ColorITEntities())
            {
                Client cl = CIentity.Client.First(i => i.ID == id);
                cl.Name = txtbx_FIO.Text.ToString();
                cl.Balance = balance;
                cl.Discount = discount;
                cl.Info = txtbx_Info.Text.ToString();
                cl.PhoneNumber = txtbx_phoneNumber.Text.ToString();

                CIentity.SaveChanges();
            }

            if (txtbx_Loan.Text.ToString() != String.Empty)
            {
                decimal loan = decimal.Parse(txtbx_Loan.Text.ToString(), NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign);

                using (ColorITEntities CIentity = new ColorITEntities())
                {
                    Client cl = CIentity.Client.First(i => i.ID == id);

                    Loan ln = new Loan();
                    ln.ClientID = id;
                    ln.Amount = loan;
                    ln.Date = DateTime.Today;

                    CIentity.AddToLoan(ln);

                    cl.Balance = cl.Balance - loan;
                    CIentity.SaveChanges();
                }
            }

            if (txtbx_Deposit.Text.ToString() != String.Empty)
            {
                decimal deposit = decimal.Parse(txtbx_Deposit.Text.ToString(), NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign);

                using (ColorITEntities CIentity = new ColorITEntities())
                {
                    Client cl = CIentity.Client.First(i => i.ID == id);

                    Deposit dp = new Deposit();
                    dp.ClientID = id;
                    dp.Amount = deposit;
                    dp.Date = DateTime.Today;

                    CIentity.AddToDeposit(dp);

                    cl.Balance = cl.Balance + deposit;
                    CIentity.SaveChanges();
                }
            }
            txtbx_Loan.Clear();
            txtbx_Deposit.Clear();
            //GetClients();
            dgv_Clients.SelectedItem = 0;
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void txtbx_Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            ListCollectionView view = CollectionViewSource.GetDefaultView(dgv_Clients.ItemsSource) as ListCollectionView;
            if ((view != null) && !String.IsNullOrEmpty(txtbx_FIO.Text))
            {
                ClientByNameFilter filter = new ClientByNameFilter(txtbx_FIO.Text);
                view.Filter = new Predicate<object>(filter.FilterItem);
            }
            /*
            using (ColorITEntities CIentity = new ColorITEntities())
            {
                var cl = from clients in CIentity.Client
                         where clients.Name.Contains(txtbx_Search.Text)
                         select clients;
                dgv_Clients.DataContext = cl;
            }
             * 
             */
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

        private System.Data.Objects.ObjectQuery<ClientGroups> GetClientGroupsQuery(ColorITEntities colorITEntities)
        {
            // Auto generated code

            System.Data.Objects.ObjectQuery<ColoritWPF.ClientGroups> clientGroupsQuery = colorITEntities.ClientGroups;
            // Update the query to include Client data in ClientGroups. You can modify this code as needed.
            clientGroupsQuery = clientGroupsQuery.Include("Client");
            // Returns an ObjectQuery.
            return clientGroupsQuery;
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
