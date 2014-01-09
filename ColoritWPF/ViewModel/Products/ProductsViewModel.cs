using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ColoritWPF.Views.Products;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace ColoritWPF.ViewModel.Products
{
    public class ProductsViewModel : ViewModelBase
    {
        private ColorITEntities colorItEntities;

        public ProductsViewModel()
        {
            if (IsInDesignMode)
            {
                //SetDefaultValues();
            }
            else
            {
                colorItEntities = new ColorITEntities();

                GetData();
                InitializeCommands();


                SaleProductsList.CollectionChanged += CollectionChanged;
            }
        }
        
        #region Properties

        public ObservableCollection<Client> Clients { get; set; }
        public ObservableCollection<Sale> SaleProductsList { get; set; }
        public ObservableCollection<SaleDocument> SaleDocuments { get; set; }

        private Client _currentClient;
        public Client CurrentClient
        {
            get
            {
                if (CurrentSaleDocument == null)
                    return Clients.First(client => client.PrivatePerson);
                return _currentClient;
            }
            set
            {
                CurrentSaleDocument.Client = value;
                _currentClient = value;
                CurrentDiscount = _currentClient.Discount;
                base.RaisePropertyChanged("CurrentClient");
                UpdateDiscountValue();
            }
        }

        private bool _includeClientBalanceToTotal;
        public bool IncludeClientBalanceToTotal
        {
            get { return _includeClientBalanceToTotal; }
            set
            {
                _includeClientBalanceToTotal = value;
                base.RaisePropertyChanged("IncludeClientBalanceToTotal");
                Recalc();
            }
        }

        public double CurrentDiscount
        {
            get
            {
                if (CurrentSaleDocument == null)
                    return 0;
                return CurrentSaleDocument.Discount;
            }
            set
            {
                CurrentSaleDocument.Discount = value;
                base.RaisePropertyChanged("CurrentDiscount");
                UpdateDiscountValue();
            }
        }

        private bool _prepay;
        public bool Prepay
        {
            get { return _prepay; }
            set
            {
                _prepay = value;
                base.RaisePropertyChanged("Prepay");
            }
        }

        private bool _isEnabled;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                base.RaisePropertyChanged("IsEnabled");
            }
        }

        private bool _confirmed;
        public bool Confirmed
        {
            get { return _confirmed; }
            set
            {
                _confirmed = value;
                base.RaisePropertyChanged("Confirmed");
            }
        }

        private SaleDocument _currentSaleDocument;
        public SaleDocument CurrentSaleDocument
        {
            get { return _currentSaleDocument; }
            set 
            { 
                _currentSaleDocument = value;
                base.RaisePropertyChanged("CurrentSaleDocument");
                if (value != null)
                {
                    CurrentClient = _currentSaleDocument.Client;

                    _confirmButtonContent = value.Confirmed ? "Разпровести" : "Провести";
                    base.RaisePropertyChanged("ConfirmButtonContent");

                    //При смене документа отчищаем список продуктов и перезаполняем.. да, ужас.. но я пока хз как лучше сделать
                    //а все потому что на изменения SaleProductsList подписка слушателя, которая сбрасывается
                    //если оставить как было: SaleProductsList = new ObservableCollection<Sale>(_currentSaleDocument.Sale.ToList());
                    SaleProductsList.Clear();
                    var tempCollection = new ObservableCollection<Sale>(_currentSaleDocument.Sale.ToList());
                    foreach (Sale sale in tempCollection)
                    {
                        SaleProductsList.Add(sale);
                    }

                    base.RaisePropertyChanged("SaleProductsList");

                    IsEnabled = !_currentSaleDocument.Confirmed;
                }
            }
        }

        private Sale _currentSale;
        public Sale CurrentSale
        {
            get { return _currentSale; }
            set
            {
                _currentSale = value;
                base.RaisePropertyChanged("CurrentSale");
            }
        }

        private string _confirmButtonContent = "Провести";
        public string ConfirmButtonContent
        {
            get
            {
                return _confirmButtonContent;
            }
            set { 
                    _confirmButtonContent = value;
                    base.RaisePropertyChanged("ConfirmButtonContent");
                }
        }

        #endregion

        #region Methods

        private void GetData()
        {
            Clients = new ObservableCollection<Client>(colorItEntities.Client.ToList());
            SaleDocuments = new ObservableCollection<SaleDocument>(colorItEntities.SaleDocument.ToList());
            SaleProductsList = new ObservableCollection<Sale>();
        }

        //Этот метод позволяет узнать когда изменился элемент коллекции
        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (Sale item in e.OldItems)
                {
                    //Removed items
                    item.PropertyChanged -= Recalc;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (Sale item in e.NewItems)
                {
                    //Added items
                    item.PropertyChanged += Recalc;
                }
            }
            Recalc();
            UpdateDiscountValue();
        }
        //А этот метод уже делает что надо когда узнает что элемент был изменен
        public void Recalc(object sender, PropertyChangedEventArgs e)
        {
            CurrentSaleDocument.CleanTotal = 0;
            CurrentSaleDocument.Total = 0;
            foreach (Sale product in SaleProductsList)
            {
                CurrentSaleDocument.CleanTotal += product.CleanTotal;
                CurrentSaleDocument.Total += product.Total;
            }
        }
        public void Recalc()
        {
            if (CurrentSaleDocument != null)
            {
                CurrentSaleDocument.CleanTotal = 0;
                CurrentSaleDocument.Total = 0;
                foreach (Sale saleProduct in SaleProductsList)
                {
                    CurrentSaleDocument.CleanTotal += saleProduct.CleanTotal;
                    CurrentSaleDocument.Total += saleProduct.Total;
                }
                if (IncludeClientBalanceToTotal)
                    CurrentSaleDocument.Total -= CurrentClient.Balance;
            }
        }

        /// <summary>
        /// Обновляет значение скидки для каждого продукта
        /// </summary>
        private void UpdateDiscountValue()
        {
            if (CurrentClient == null)
                return;

            foreach (Sale product in SaleProductsList)
            {
                product.CurrentDiscount = CurrentDiscount;
            }
        }

        //Удаляет из списка продуктов выбранный элемент
        private void RemoveSaleItemFromList(IEnumerable<Sale> selectedItemsToRemove)
        {
            var tempList = new List<Sale>(selectedItemsToRemove);
            if (selectedItemsToRemove != null)
            {
                try
                {

                    foreach (Sale sale in tempList)
                    {

                        var removeIt = colorItEntities.Sale.First(item => item.ID == sale.ID);
                        colorItEntities.Sale.DeleteObject(removeIt);
                        if (SaleProductsList.Contains(sale))
                            SaleProductsList.Remove(sale);
                    }

                    colorItEntities.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw new Exception("Не удалось удалить продукт " +
                                        "Ошибка: " + ex.Message + "\n" + ex.InnerException);
                }
            }
        }
        private void RemoveSaleItemFromList(Sale selectedItemToRemove)
        {
            if (selectedItemToRemove != null)
            {
                try
                {
                    var removeIt = colorItEntities.Sale.First(item => item.ID == selectedItemToRemove.ID);
                    colorItEntities.Sale.DeleteObject(removeIt);
                    SaleProductsList.Remove(selectedItemToRemove);

                    colorItEntities.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw new Exception("Не удалось удалить продукт '" + selectedItemToRemove.Name + "' " +
                                        "Ошибка: " + ex.Message + "\n" + ex.InnerException);
                }
            }
        }

        #endregion

        #region Commands

        public RelayCommand OpenSelectionCommand
        {
            get;
            private set;
        }

        public RelayCommand PrintDocumentCommand
        {
            get;
            private set;
        }


        public RelayCommand ConfirmDocumentCommand
        {
            get;
            private set;
        }

        public RelayCommand PrepayDocumentCommand
        {
            get;
            private set;
        }

        public RelayCommand MoveProductDlgCommand
        {
            get;
            private set;
        }

        public RelayCommand RemoveProductFromListCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    RemoveSaleItemFromList(CurrentSale);
                }, () => IsEnabled);
            }
        }


        public RelayCommand RemoveProductDocumentFromListCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (MessageBox.Show("Вы уверены что хотите удалить документ?",
                                        "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        RemoveSaleItemFromList(SaleProductsList);
                        var docToRemove = colorItEntities.SaleDocument.First(doc => doc.Id == CurrentSaleDocument.Id);

                        try
                        {
                            colorItEntities.SaleDocument.DeleteObject(docToRemove);
                            colorItEntities.SaveChanges();
                            SaleDocuments.Remove(CurrentSaleDocument);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Не удалось удалить документ '" + CurrentSaleDocument.DocumentNumber + "' " +
                                                "Ошибка: " + ex.Message + "\n" + ex.InnerException);
                        }

                    }
                }, () => IsEnabled);
            }
        }

        public RelayCommand<Visual> PrintCommand
        {
            get
            {
                return new RelayCommand<Visual>(v =>
                {
                    PrintDialog printDlg = new PrintDialog();

                    if (printDlg.ShowDialog() != true)
                    {return;}

                    StackPanel myPanel = new StackPanel();
                    myPanel.Margin = new Thickness(5,5,5,5);

                    TextBlock myBlock = new TextBlock();
                    myBlock.Text = "Расходная накладная №" + CurrentSaleDocument.DocumentNumber +
                                                                 " от " + CurrentSaleDocument.DateCreated;
                    myBlock.TextAlignment = TextAlignment.Left;
                    myBlock.Margin = new Thickness(5,5,5,5);
                    myPanel.Children.Add(myBlock);

                    TextBlock clientBalance = new TextBlock();
                    clientBalance.Text = "Клиент: \t" + CurrentSaleDocument.Client.Name;
                    clientBalance.Margin = new Thickness(5, 5, 5, 5);
                    myPanel.Children.Add(clientBalance);

                    DataGrid dg = v as DataGrid;
                    DataGrid printDataGrid = new DataGrid();
                    printDataGrid.Margin = new Thickness(5,5,5,5);
                    printDataGrid.LoadingRow += LoadingRow;

                    printDataGrid.RowHeaderTemplate = dg.RowHeaderTemplate;
                    printDataGrid.RowHeaderWidth = dg.RowHeaderWidth;
                     
                    foreach (DataGridTextColumn item in dg.Columns)
                    {
                        printDataGrid.Columns.Add(new DataGridTextColumn
                                                      {
                                                          Width = new DataGridLength(1.0, DataGridLengthUnitType.Auto),
                                                          Header = item.Header,
                                                          Binding = item.Binding
                                                      });
                    }
                    
                    foreach (Sale item in dg.Items)
                    {
                        printDataGrid.Items.Add(new Sale
                                                    {
                                                        ID = item.ID,
                                                        Product = item.Product,
                                                        ProductID = item.ProductID,
                                                        Amount = item.Amount,
                                                        Cost = item.Cost,
                                                        CurrentDiscount = item.CurrentDiscount,
                                                        Total = item.Total
                                                    });
                    }
                    
                    myPanel.Children.Add(printDataGrid);

                    TextBlock totalValue = new TextBlock();
                    totalValue.Text = "Итого: " + CurrentSaleDocument.Total.ToString("c");
                    totalValue.Margin = new Thickness(5,5,5,5);
                    myPanel.Children.Add(totalValue);
                    
                    Grid grid = new Grid();
                    ColumnDefinition columnDefinition = new ColumnDefinition();
                    columnDefinition.Width = new GridLength(1.0, GridUnitType.Auto);
                    grid.Children.Add(myPanel);

                    grid.ColumnDefinitions.Add(columnDefinition);
                    printDlg.PrintVisual(grid, "Grid Printing.");
                }, visual => CurrentSaleDocument != null);
            }
        }

        private void LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();   
        }

        private void InitializeCommands()
        {
            OpenSelectionCommand = new RelayCommand(OpenSelection);
            
            ConfirmDocumentCommand = new RelayCommand(ConfirmDocument, ConfirmDocumentCanExecute);
            MoveProductDlgCommand = new RelayCommand(MoveProductDlg);
            PrepayDocumentCommand = new RelayCommand(PrepayDocument);
        }

        private bool ConfirmDocumentCanExecute()
        {
            return CurrentSaleDocument != null;
        }

        private void MoveProductDlg()
        {
            TransferProductView transferProductView = new TransferProductView();
            transferProductView.ShowDialog();
        }

        private void PrepayDocument()
        {
            Prepay = true;
            Confirmed = false;
            SaveDocument();
        }

        private void ConfirmDocument()
        {
            if (MessageBox.Show("Вы уверены что хотите "+ ConfirmButtonContent.ToLower()+" документ?",
                                "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (!CurrentSaleDocument.Confirmed)
                {
                    Prepay = false;
                    Confirmed = true;

                    SaveDocument();
                    IsEnabled = false;
                }
                else
                {
                    Prepay = true;
                    Confirmed = false;
                    UnConfirmDocument();
                    IsEnabled = true;
                }
            }
        }

        /// <summary>
        /// Используется при проведении и сохранении документа, в зависимости от значений Confirmed и Prepay
        /// </summary>
        private void SaveDocument()
        {
            if (IsAllProductInStock() == false)
                return;

            if(CurrentSaleDocument == null)
                return;         

            CurrentSaleDocument.ClientId = CurrentClient.ID;
            CurrentSaleDocument.Prepay = Prepay;
            CurrentSaleDocument.Confirmed = Confirmed;
            
            if (IncludeClientBalanceToTotal && Confirmed && !CurrentClient.PrivatePerson)
            {
                Client client = colorItEntities.Client.First(cl => cl.ID == CurrentClient.ID);
                CurrentSaleDocument.ClientBalancePartInTotal = client.Balance;
                client.Balance = 0;
            }

            try
            {
                colorItEntities.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось сохранить список продаж\n" + ex.Message + "\n" + ex.InnerException);
            }
            
            foreach (Sale saleProduct in SaleProductsList)
            {
                Sale saleToUpd = colorItEntities.Sale.First(item => item.ID == saleProduct.ID);
                saleToUpd.Amount = saleProduct.Amount;
                saleToUpd.Discount = saleProduct.CurrentDiscount;
                saleToUpd.Cost = saleProduct.Cost;

                if (Confirmed)
                {
                    var productStorage = colorItEntities.Product.First(pr => pr.ID == saleProduct.Product.ID);
                    productStorage.Storage = productStorage.Storage - saleProduct.Amount;
                }
            }

            if (Confirmed)
            {
                var settings = colorItEntities.Settings.FirstOrDefault();
                if (settings != null) settings.Cash = settings.Cash + CurrentSaleDocument.Total;
            }

            try
            {
                colorItEntities.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось сохранить список продаж\n"+ex.Message+"\n"+ex.InnerException);
            }
        }

        /// <summary>
        /// Используется при раcпроведении документа
        /// </summary>
        private void UnConfirmDocument()
        {
            CurrentSaleDocument.Prepay = Prepay;
            CurrentSaleDocument.Confirmed = Confirmed;

            try
            {
                colorItEntities.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось сохранить список продаж\n" + ex.Message + "\n" + ex.InnerException);
            }

            foreach (Sale saleProduct in SaleProductsList)
            {
                var productStorage = colorItEntities.Product.First(pr => pr.ID == saleProduct.Product.ID);
                productStorage.Storage = productStorage.Storage + saleProduct.Amount;
            }

            if (CurrentSaleDocument.ClientBalancePartInTotal != 0)
            {
                Client client = colorItEntities.Client.First(cl => cl.ID == CurrentClient.ID);
                client.Balance += CurrentSaleDocument.ClientBalancePartInTotal;
            }

            var settings = colorItEntities.Settings.FirstOrDefault();
            if (settings != null) settings.Cash = settings.Cash - CurrentSaleDocument.Total;

            try
            {
                colorItEntities.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось сохранить список продаж\n" + ex.Message + "\n" + ex.InnerException);
            }
        }

        /// <summary>
        /// Проверяет наличие каждого товара из списка
        /// </summary>
        /// <returns>Возвращает false если товара в магазине не достаточно</returns>
        private bool IsAllProductInStock()
        {
            List<string> notEnough = new List<string>();
            foreach (Sale saleProduct in SaleProductsList)
            {
                if (saleProduct.Product.Storage < saleProduct.Amount)
                    notEnough.Add(saleProduct.Name);
            }

            if (notEnough.Count > 0)
            {
                string msgText = String.Format("Недостаточно товара в магазине:\n");
                msgText = notEnough.Aggregate(msgText, (current, name) => current + name + "\n");

                MessageBox.Show(msgText, "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        private void OpenSelection()
        {
            var dlg = new UniProductSelectorView();
            dlg.Show();
            dlg.Closed += DlgOnClosed;
//            ProductsSelectorView productsSelector = new ProductsSelectorView();
//            productsSelector.ShowDialog();
        }

        private void DlgOnClosed(object sender, EventArgs eventArgs)
        {
            var dlg = sender as UniProductSelectorView;
            var vm = dlg.DataContext as UniProductSelectorViewModel;
            if (vm == null || vm.SelectedProducts.Count == 0)
                return;
            Client defaultClient = colorItEntities.Client.First(client => client.PrivatePerson);

            SaleDocument saleDocument = new SaleDocument
            {
                DateCreated = DateTime.Now,
                Confirmed = false,
                Prepay = false,
                Client = defaultClient,
                ClientId = defaultClient.ID
            };
            saleDocument.GenerateDocNumber();
            colorItEntities.SaleDocument.AddObject(saleDocument);

            try
            {
                colorItEntities.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось сохранить документ в базу\n" + ex.Message + "\n" + ex.InnerException);
            }

            foreach (Product product in vm.SelectedProducts)
            {
                Sale saleProduct = new Sale
                {
                    ProductID = product.ID,
                    SaleListNumber = saleDocument.Id,
                    Amount = product.Amount,
                    Cost = product.Cost
                };
                colorItEntities.Sale.AddObject(saleProduct);
            }

            try
            {
                colorItEntities.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось сохранить список продаваемых продуктов в базу\n" + ex.Message + "\n" + ex.InnerException);
            }

            SaleDocuments.Add(saleDocument);
            CurrentSaleDocument = saleDocument;
        }

        #endregion
    }
}
