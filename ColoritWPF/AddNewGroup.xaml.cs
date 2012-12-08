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
    /// Interaction logic for AddNewGroup.xaml
    /// </summary>
    public partial class AddNewGroup : Window
    {
        public AddNewGroup()
        {
            InitializeComponent();
        }

        private System.Data.Objects.ObjectQuery<Group> GetGroupQuery(ColorITEntities colorITEntities)
        {
            // Auto generated code

            System.Data.Objects.ObjectQuery<ColoritWPF.Group> groupQuery = colorITEntities.Group;
            // Returns an ObjectQuery.
            return groupQuery;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            ColoritWPF.ColorITEntities colorITEntities = new ColoritWPF.ColorITEntities();
            // Load data into Group. You can modify this code as needed.
            System.Windows.Data.CollectionViewSource groupViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("groupViewSource")));
            System.Data.Objects.ObjectQuery<ColoritWPF.Group> groupQuery = this.GetGroupQuery(colorITEntities);
            groupViewSource.Source = groupQuery.Execute(System.Data.Objects.MergeOption.AppendOnly);
        }

        private void btn_Add_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(txtbx_NewGroupName.Text))
            {
                using (ColorITEntities ColorEnt = new ColorITEntities())
                {
                    Group gr = new Group();
                    gr.Name = txtbx_NewGroupName.Text;

                    try
                    {
                        ColorEnt.AddToGroup(gr);
                        ColorEnt.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                    MessageBox.Show("Новая группа добавлена", "Успех!", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtbx_NewGroupName.Text = String.Empty;
                }
            }
        }
    }
}
