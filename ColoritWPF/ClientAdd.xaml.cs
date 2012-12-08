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
    /// Interaction logic for ClientAdd.xaml
    /// </summary>
    public partial class ClientAdd : Window
    {
        public ClientAdd()
        {
            InitializeComponent();
        }

        private void btn_Add_Click(object sender, RoutedEventArgs e)
        {
            decimal balance = decimal.Parse(txtbx_Balance.Text.ToString(), NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign);
            double discount = double.Parse(txtbx_Discount.Text.ToString(), NumberStyles.AllowDecimalPoint);

            using (ColorITEntities CIentity = new ColorITEntities())
            {
                Client cl = new Client();
                cl.Name = txtbx_FIO.Text.ToString();
                cl.Balance = balance;
                cl.Discount = discount;
                cl.Info = txtbx_Info.Text.ToString();
                cl.PhoneNumber = txtbx_phoneNumber.Text.ToString();
                cl.GroupID = ((ClientGroups)cmbx_Group.SelectedItem).ID;

                try
                {
                    CIentity.AddToClient(cl);
                    CIentity.SaveChanges();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                txtbx_Balance.Clear();
                txtbx_Discount.Clear();
                txtbx_FIO.Clear();
                txtbx_Info.Clear();
                MessageBox.Show("Запись успешно добавлена.", "Спасибо");
            }
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private System.Data.Objects.ObjectQuery<Client> GetClientQuery(ColorITEntities colorITEntities)
        {
            // Auto generated code

            System.Data.Objects.ObjectQuery<ColoritWPF.Client> clientQuery = colorITEntities.Client;
            // To explicitly load data, you may need to add Include methods like below:
            // clientQuery = clientQuery.Include("Client.ClientGroups").
            // For more information, please see http://go.microsoft.com/fwlink/?LinkId=157380
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
            // Load data into ClientGroups. You can modify this code as needed.
            System.Windows.Data.CollectionViewSource clientGroupsViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("clientGroupsViewSource")));
            System.Data.Objects.ObjectQuery<ColoritWPF.ClientGroups> clientGroupsQuery = this.GetClientGroupsQuery(colorITEntities);
            clientGroupsViewSource.Source = clientGroupsQuery.Execute(System.Data.Objects.MergeOption.AppendOnly);
        }

        private System.Data.Objects.ObjectQuery<ClientGroups> GetClientGroupsQuery(ColorITEntities colorITEntities)
        {
            // Auto generated code

            System.Data.Objects.ObjectQuery<ColoritWPF.ClientGroups> clientGroupsQuery = colorITEntities.ClientGroups;
            // Returns an ObjectQuery.
            return clientGroupsQuery;
        }
    }
}
