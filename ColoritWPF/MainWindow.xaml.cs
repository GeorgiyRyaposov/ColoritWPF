using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;

namespace ColoritWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ProductByNameFilter filter;
        ListCollectionView view;
        private PaintMath _paintMath;

        private ColoritWPF.ColorITEntities colorITEntities;
        public MainWindow()
        {
            InitializeComponent();
            grid_Total.DataContext = new PaintMath();
            _paintMath = grid_Total.DataContext as PaintMath;
        }
        
        private System.Data.Objects.ObjectQuery<Product> GetProductQuery(ColorITEntities colorITEntities)
        {
            // Auto generated code
            System.Data.Objects.ObjectQuery<ColoritWPF.Product> productQuery = colorITEntities.Product;
            // Returns an ObjectQuery.
            return productQuery;
        }

        void recalculateSum(object sender, EventArgs e)
        {
            //txtbx_PaintCostSum.Text = CalculateTotalSumForPaints().ToString();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            colorITEntities = new ColoritWPF.ColorITEntities();
            // Load data into Product. You can modify this code as needed.
            System.Windows.Data.CollectionViewSource productViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("productViewSource")));
            System.Data.Objects.ObjectQuery<ColoritWPF.Product> productQuery = this.GetProductQuery(colorITEntities);
            productViewSource.Source = productQuery.Execute(System.Data.Objects.MergeOption.AppendOnly);

            this.DataContext = productViewSource;
            view = (ListCollectionView)CollectionViewSource.GetDefaultView(this.DataContext);


            //wtf??
            //var collectionViewSource = this.FindResource("Paint") as CollectionViewSource;
            //collectionViewSource.Source = _paint;

            //view = (ListCollectionView)CollectionViewSource.GetDefaultView(dgv_product.DataContext);
            //Группировка
            ProductGroupConverter prGrConv = new ProductGroupConverter();
            productViewSource.GroupDescriptions.Add(new PropertyGroupDescription("Group", prGrConv));

            //
            /*
            //Фильтрация для поиска по наименованию товара
            ListCollectionView view = CollectionViewSource.GetDefaultView(dgv_product.ItemsSource) as ListCollectionView;
            if((view != null))// && !String.IsNullOrEmpty(txtbx_Search.Text))
            {
                filter = new ProductByNameFilter(txtbx_Search.Text);
                view.Filter = new Predicate<object>(filter.FilterItem);
            }
             */
            // Load data into Paints. You can modify this code as needed.
            System.Windows.Data.CollectionViewSource paintsViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("paintsViewSource")));
            System.Data.Objects.ObjectQuery<ColoritWPF.Paints> paintsQuery = this.GetPaintsQuery(colorITEntities);
            paintsViewSource.Source = paintsQuery.Execute(System.Data.Objects.MergeOption.AppendOnly);
            // Load data into CarModels. You can modify this code as needed.
            System.Windows.Data.CollectionViewSource carModelsViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("carModelsViewSource")));
            System.Data.Objects.ObjectQuery<ColoritWPF.CarModels> carModelsQuery = this.GetCarModelsQuery(colorITEntities);
            carModelsViewSource.Source = carModelsQuery.Execute(System.Data.Objects.MergeOption.AppendOnly);
            // Load data into PaintName. You can modify this code as needed.
            System.Windows.Data.CollectionViewSource paintNameViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("paintNameViewSource")));
            System.Data.Objects.ObjectQuery<ColoritWPF.PaintName> paintNameQuery = this.GetPaintNameQuery(colorITEntities);
            paintNameViewSource.Source = paintNameQuery.Execute(System.Data.Objects.MergeOption.AppendOnly);
            // Load data into Client. You can modify this code as needed.
            System.Windows.Data.CollectionViewSource clientViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("clientViewSource")));
            System.Data.Objects.ObjectQuery<ColoritWPF.Client> clientQuery = this.GetClientQuery(colorITEntities);
            clientViewSource.Source = clientQuery.Execute(System.Data.Objects.MergeOption.AppendOnly);
        }

        private void txtbx_Search_TextChanged(object sender, TextChangedEventArgs e)
        {

            //view = CollectionViewSource.GetDefaultView(dgv_product.DataContext) as ListCollectionView;
            if (view != null)
            {
                filter = new ProductByNameFilter(txtbx_Search.Text);
                //filter.SearchText = txtbx_Search.Text;
                if ((filter != null) && !String.IsNullOrEmpty(txtbx_Search.Text))
                {
                    view.Filter = new Predicate<object>(filter.FilterItem);
                    view.Refresh();
                }
                else
                    view.Filter = null;
            }
            /*
            using (ColorITEntities ColorItEnt = new ColorITEntities())
            {
                var pr = from products in ColorItEnt.Product
                         where products.Name.Contains(txtbx_Search.Text)
                         select products;
                dgv_product.DataContext = pr;
            }
             */
        }



        #region Menu Items
        private void MenuItemAddProduct_Click(object sender, RoutedEventArgs e)
        {
            AddProduct addPr = new AddProduct();
            addPr.ShowDialog();
        }

        private void MenuItemPurchaseProduct_Click(object sender, RoutedEventArgs e)
        {
            BuyProduct buyPr = new BuyProduct();
            buyPr.ShowDialog();
        }

        private void MenuItemSaleProduct_Click(object sender, RoutedEventArgs e)
        {
            SaleProduct salePr = new SaleProduct();
            salePr.ShowDialog();
        }

        private void MenuItemClientAdd_Click(object sender, RoutedEventArgs e)
        {
            ClientAdd clientAdd = new ClientAdd();
            clientAdd.ShowDialog();
        }

        private void MenuItemClientList_Click(object sender, RoutedEventArgs e)
        {
            ClientList clientList = new ClientList();
            clientList.ShowDialog();
        }

        private void MenuItemSalesStats_Click(object sender, RoutedEventArgs e)
        {
            SalesStats slStats = new SalesStats();
            slStats.ShowDialog();
        }

        private void MenuItemGetFromExcel_Click(object sender, RoutedEventArgs e)
        {
            ImportFromExcel ife = new ImportFromExcel();
            ife.ShowDialog();
        }

        private void MenuItemEditProduct_Click(object sender, RoutedEventArgs e)
        {
            EditProducts edPr = new EditProducts();
            edPr.ShowDialog();
        }

        private void MenuItemAddGroup_Click(object sender, RoutedEventArgs e)
        {
            AddNewGroup addNewGr = new AddNewGroup();
            addNewGr.ShowDialog();
        }

        private void MenuItemClientGroupAdd_Click(object sender, RoutedEventArgs e)
        {
            AddClientGroup addClGr = new AddClientGroup();
            addClGr.ShowDialog();
        }
        #endregion

        private System.Data.Objects.ObjectQuery<Paints> GetPaintsQuery(ColorITEntities colorITEntities)
        {
            // Auto generated code

            System.Data.Objects.ObjectQuery<ColoritWPF.Paints> paintsQuery = colorITEntities.Paints;
            // To explicitly load data, you may need to add Include methods like below:
            // paintsQuery = paintsQuery.Include("Paints.CarModels").
            // For more information, please see http://go.microsoft.com/fwlink/?LinkId=157380
            // Returns an ObjectQuery.
            return paintsQuery;
        }

        private System.Data.Objects.ObjectQuery<CarModels> GetCarModelsQuery(ColorITEntities colorITEntities)
        {
            // Auto generated code

            System.Data.Objects.ObjectQuery<ColoritWPF.CarModels> carModelsQuery = colorITEntities.CarModels;
            // Returns an ObjectQuery.
            return carModelsQuery;
        }

        private void btnAddCarModel_Click(object sender, RoutedEventArgs e)
        {
            AddCarModelDialog carModelDialog = new AddCarModelDialog();
            carModelDialog.Show();
        }

        private void btnRemoveCarModel_Click(object sender, RoutedEventArgs e)
        {
            if ((MessageBox.Show("Вы уверены?", "Подтвердите", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes))
            {
                using (colorITEntities)
                {
                    CarModels carModel = (CarModels) (cmbxCarModel.SelectedItem);
                    try
                    {
                        colorITEntities.DeleteObject(carModel);
                        colorITEntities.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
            }
        }

        private int GetSalary()
        {
            int sum = 0;
            if (rbCode.IsChecked != null)
            {
                if ((bool)rbCode.IsChecked)
                {
                    sum = 50;
                }
            }
            if (rbSelection.IsChecked != null)
            {
                if ((bool)rbSelection.IsChecked)
                {
                    sum = 350;
                }
            }

            return sum;
        }

        private void rbLSB_Checked(object sender, RoutedEventArgs e)
        {
            if (lblPaint != null)
                lblPaint.Content = "LSB";
            if (cbPackage != null)
                cbPackage.IsChecked = false;
        }
    

        private void rbL2K_Checked(object sender, RoutedEventArgs e)
        {
            lblPaint.Content = "L2K";
            if (lblPolish != null)
            {
                lblPolish.Visibility = Visibility.Hidden;
                txtbxPolishAmount.Visibility = Visibility.Hidden;
            }
            if (grid_Colors != null)
                grid_Colors.Visibility = Visibility.Visible;
            if (cbThreeLayers != null)
            {
                cbThreeLayers.IsChecked = false;
                cbThreeLayers.IsEnabled = false;
            }
        }

        private void rbL2K_Unchecked(object sender, RoutedEventArgs e)
        {
            if (lblPolish != null)
            {
                lblPolish.Visibility = Visibility.Visible;
                txtbxPolishAmount.Visibility = Visibility.Visible;
            }
            if (grid_Colors != null)
                grid_Colors.Visibility = Visibility.Hidden;
            if (cbThreeLayers != null)
                cbThreeLayers.IsEnabled = true;
        }

        private void rbABP_Checked(object sender, RoutedEventArgs e)
        {
            lblPaint.Content = "ABP";
        }

        private void rbPolish_Checked(object sender, RoutedEventArgs e)
        {
            txtbxPaintAmount.IsEnabled = false;
            cbPackage.IsChecked = true;
        }

        private System.Data.Objects.ObjectQuery<PaintName> GetPaintNameQuery(ColorITEntities colorITEntities)
        {
            // Auto generated code

            System.Data.Objects.ObjectQuery<ColoritWPF.PaintName> paintNameQuery = colorITEntities.PaintName;
            // Returns an ObjectQuery.
            return paintNameQuery;
        }

        private void rbPolish_Unchecked(object sender, RoutedEventArgs e)
        {
            txtbxPaintAmount.IsEnabled = true;
        }

        private void cbPackage_Unchecked(object sender, RoutedEventArgs e)
        {
            rbL2K.IsEnabled = true;
            if ((bool)rbPolish.IsChecked == false)
                txtbxPolishAmount.IsEnabled = false;
            
        }

        private void rbOther_Checked(object sender, RoutedEventArgs e)
        {
            cmbxPaintName.IsEnabled = true;
            txtbxPolishAmount.IsEnabled = false;
            lblPaint.Content = "Кол-во:";
        }

        private void rbOther_Uncheked(object sender, RoutedEventArgs e)
        {
            cmbxPaintName.IsEnabled = false;
            txtbxPolishAmount.IsEnabled = true;
        }

        //Нажать по кнопке "Продать"
        private void btn_Add_Click_1(object sender, RoutedEventArgs e)
        {
            if(!String.IsNullOrEmpty(txtbxPaintAmount.Text))
                AddPaint();
            BindingExpression binding = paintsDataGrid.GetBindingExpression(DataGrid.DataContextProperty);
            if (binding != null) binding.UpdateSource();
        }

        private void rbSelection_Checked(object sender, RoutedEventArgs e)
        {
            cbColorist.IsChecked = true;
            
        }

        private void rbCode_Checked(object sender, RoutedEventArgs e)
        {
            if (cbColorist!=null)
                cbColorist.IsChecked = false;
            
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

        private void cbClient_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbClient.SelectedItem != null)
            {
                
                if (lblDebt != null)
                    lblDebt.Content = ((Client) cbClient.SelectedItem).Balance.ToString();
                if ((int)cbClient.SelectedValue == 7)
                {
                    txtbx_PhonNum.Text = "";
                    //txtbx_PhonNum.IsEnabled = true;
                }
                else
                {
                    txtbx_PhonNum.Text = ((Client)cbClient.SelectedItem).PhoneNumber;
                    //txtbx_PhonNum.IsEnabled = false;
                }
            }
            _paintMath.SelectedClient = (Client) cbClient.SelectedItem;
        }

        private void paintsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var currentRow = ((Paints) paintsDataGrid.SelectedItem);
            _paintMath.SelectedPaint = GetSelectedPaintName();
            //_paint.Amount = (decimal) ((Paints) paintsDataGrid.SelectedItem).Amount;
            cmbxCarModel.SelectedValue = currentRow.CarModels.ID;
            //_paint.Client = currentRow.Client;
            //_paint.ColorCode = currentRow.PaintCode;
            //_paint.PhoneNumber = currentRow.PhoneNumber;
            
            cbClient.SelectedValue = currentRow.Client.ID;
            
            txtbxPaintAmount.Text = currentRow.Amount.ToString();
            switch (currentRow.ServiceTypes.ID)
            {
                case 1:
                    {
                        //_paint.IsRed = true;
                        rb_Red.IsChecked = true;
                        break;
                    }
                case 2:
                    {
                        //_paint.IsSelection = true;
                        rbSelection.IsChecked = true;
                        break;
                    }
            }
            if (currentRow.Salary > 0)
            {
                //_paint.IsColorist = true;
                cbColorist.IsChecked = true;
            }
            else
            {
                //_paint.IsColorist = false;
                cbColorist.IsChecked = false;
            }

            //switch (currentRow.PaintName.ID)
            //{
            //    case 1:
            //        {
            //            //_paint.IsL2K = true;
            //            rbL2K.IsChecked = true;
            //            //_paint.IsWhite = true;
            //            rb_White.IsChecked = true;
            //            //_paint.Amount = Convert.ToDecimal(currentRow.Amount.ToString());
            //            //_paint.PolishAmount = 0;
            //            break;
            //        }
            //    case 2:
            //        {
            //            //_paint.IsL2K = true;
            //            rbL2K.IsChecked = true;
            //            //_paint.IsRed = true;
            //            rb_Red.IsChecked = true;
            //            //_paint.Amount = Convert.ToDecimal(currentRow.Amount.ToString());
            //            //_paint.PolishAmount = 0;
            //            break;
            //        }
            //    case 3:
            //        {
            //            //_paint.IsL2K = true;
            //            rbL2K.IsChecked = true;
            //            //_paint.IsRed = true;
            //            rb_Red.IsChecked = true;
            //            //_paint.Amount = Convert.ToDecimal(currentRow.Amount.ToString());
            //            //_paint.PolishAmount = 0;
            //            break;
            //        }
            //    case 4:
            //        {
            //            rbLSB.IsChecked = true;
            //            //_paint.IsLsb = true;
            //            //cbThreeLayers.IsChecked = false;
            //            txtbxPolishAmount.Text = currentRow.Amount.ToString();
            //            break;
            //        }
            //    case 6:
            //        {
            //            //_paint.IsLsb = true;
            //            rbLSB.IsChecked = true;
            //            //cbThreeLayers.IsChecked = true;
            //            //_paint.PolishAmount = Convert.ToDecimal(currentRow.Amount.ToString());
            //            break;
            //        }
            //    default:
            //        {
            //            //_paint.IsOther = true;
            //            rbOther.IsChecked = true;
            //            //_paint.Other = currentRow.PaintName;
            //            break;
            //        }
            //}
            //_paint.SumOfGoods = Convert.ToDecimal(currentRow.Sum.ToString());
        }

        private void txtbxPaintAmount_LostFocus(object sender, RoutedEventArgs e)
        {
            _paintMath.Amount = decimal.Parse(txtbxPaintAmount.Text);
            /*
            TextBox myTextBox = (TextBox)sender;
            string value = myTextBox.Text;
            var culture = CultureInfo.CreateSpecificCulture("fr-FR");
            double dblTemp;

            if(value.Contains("."))
            {value = value.Replace(".", ",");}
            
            Double.TryParse(value, NumberStyles.AllowDecimalPoint, culture, out dblTemp);
            myTextBox.Text = dblTemp.ToString("#0.00", culture);
            */
        }

        private void txtbxPolishAmount_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox myTextBox = (TextBox)sender;
            string value = myTextBox.Text;
            var culture = CultureInfo.CreateSpecificCulture("fr-FR");
            double dblTemp;

            if (value.Contains("."))
            { value = value.Replace(".", ","); }

            Double.TryParse(value, NumberStyles.AllowDecimalPoint, culture, out dblTemp);
            myTextBox.Text = dblTemp.ToString("#0.00", culture);
            
        }

        //Сохранение изменения значения ячейки в колонке (таблица редактирования красок в подборе)
        private void paintNameDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            using (ColorITEntities colorItEntities = new ColorITEntities())
            {
                int id = ((PaintName) DataGrid_EditPaintName.CurrentItem).ID;
                var paintNameToUpdate = (from paint in colorItEntities.PaintName
                                 where paint.ID == id
                                 select paint).First();
                paintNameToUpdate.Name = ((PaintName) DataGrid_EditPaintName.CurrentItem).Name;
                decimal cost;
                decimal work = 0;
                decimal container;

                //try
                //{
                    
                //}
                //catch (Exception)
                //{
                //    MessageBox.Show("Введите корректное значение цены за краску");
                //}

                //try
                //{
                    
                //}
                //catch (Exception)
                //{
                //    MessageBox.Show("Введите корректное значение цены за работу");
                //}
                //try
                //{
                    
                //}
                //catch (Exception)
                //{
                //    MessageBox.Show("Введите корректное значение цены за тару");
                //}
                cost = decimal.Parse(((PaintName)DataGrid_EditPaintName.CurrentItem).Cost.ToString(CultureInfo.InvariantCulture));
                
                if (!String.IsNullOrEmpty(((PaintName)DataGrid_EditPaintName.CurrentItem).Work.ToString()))
                    work = decimal.Parse(((PaintName)DataGrid_EditPaintName.CurrentItem).Work.ToString());

                container = decimal.Parse(((PaintName)DataGrid_EditPaintName.CurrentItem).Container.ToString());
                paintNameToUpdate.Cost = cost;
                paintNameToUpdate.Work = work;
                paintNameToUpdate.Container = container;

                colorItEntities.SaveChanges();
            }

        }

        //Нажатие по кнопке "сохранить"
        private void btn_Save_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void cbPackage_Checked(object sender, RoutedEventArgs e)
        {
            txtbxPolishAmount.IsEnabled = true;
        }

        //Новая запись в красках
        private void btn_New_Click(object sender, RoutedEventArgs e)
        {
            //_paint = new Paint();
            txtbxPaintCode.Text = String.Empty;
            cbClient.SelectedValue = 7; //Частный клиент
            cmbxCarModel.SelectedValue = 3; //Выберите авто
            txtbxPaintAmount.Text = String.Empty;
            txtbx_PaintCostSum.Text = String.Empty;
            txtbx_Prepayment.Text = String.Empty;
            rbLSB.IsChecked = true;
            cbThreeLayers.IsChecked = false;
            cbPackage.IsChecked = false;
            rbCode.IsChecked = true;
            cbColorist.IsChecked = false;
            txtbxPolishAmount.Text = String.Empty;
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void cmbxPaintName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void cbThreeLayers_Checked(object sender, RoutedEventArgs e)
        {
            
        }

        private void rbCode_Checked_1(object sender, RoutedEventArgs e)
        {
            
        }

        private void rbSelection_Checked_1(object sender, RoutedEventArgs e)
        {
            
        }

        private void cbColorist_Checked(object sender, RoutedEventArgs e)
        {
            
        }

        private void txtbx_Prepayment_LostFocus(object sender, RoutedEventArgs e)
        {
            
        }

    }
}
