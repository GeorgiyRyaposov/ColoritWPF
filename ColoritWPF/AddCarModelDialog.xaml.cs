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
    /// Interaction logic for AddCarModelDialog.xaml
    /// </summary>
    public partial class AddCarModelDialog : Window
    {
        public AddCarModelDialog()
        {
            InitializeComponent();
        }

        private void btn_Add_Click(object sender, RoutedEventArgs e)
        {
            string modelName;
            if (!String.IsNullOrEmpty(txtbx_carModel.Text))
            {
                modelName = txtbx_carModel.Text.Trim();
                using (ColorITEntities colorItEntities = new ColorITEntities())
                {
                    CarModels carModel = new CarModels();
                    carModel.ModelName = modelName;
                    try
                    {
                        colorItEntities.AddToCarModels(carModel);
                        colorItEntities.SaveChanges();
                        txtbx_carModel.Text = String.Empty;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
            }
            else
            {
                MessageBox.Show("Заполните поле наименования автомобиля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
