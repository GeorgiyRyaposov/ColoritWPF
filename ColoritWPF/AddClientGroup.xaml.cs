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
    /// Interaction logic for AddClientGroup.xaml
    /// </summary>
    public partial class AddClientGroup : Window
    {
        public AddClientGroup()
        {
            InitializeComponent();
        }

        private System.Data.Objects.ObjectQuery<ClientGroups> GetClientGroupsQuery(ColorITEntities colorITEntities)
        {
            // Auto generated code

            System.Data.Objects.ObjectQuery<ColoritWPF.ClientGroups> clientGroupsQuery = colorITEntities.ClientGroups;
            // Returns an ObjectQuery.
            return clientGroupsQuery;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            ColoritWPF.ColorITEntities colorITEntities = new ColoritWPF.ColorITEntities();
            // Load data into ClientGroups. You can modify this code as needed.
            System.Windows.Data.CollectionViewSource clientGroupsViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("clientGroupsViewSource")));
            System.Data.Objects.ObjectQuery<ColoritWPF.ClientGroups> clientGroupsQuery = this.GetClientGroupsQuery(colorITEntities);
            clientGroupsViewSource.Source = clientGroupsQuery.Execute(System.Data.Objects.MergeOption.AppendOnly);
        }

        private void btn_Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btn_Add_Click(object sender, RoutedEventArgs e)
        {
            using (ColorITEntities colorEnt = new ColorITEntities())
            {
                ClientGroups clGr = new ClientGroups();
                clGr.Name = txtbx_AddGroup.Text.ToString();

                colorEnt.AddToClientGroups(clGr);
                try
                {
                    colorEnt.SaveChanges();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }
    }
}
