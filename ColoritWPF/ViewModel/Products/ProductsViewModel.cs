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
using GalaSoft.MvvmLight.Messaging;

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
                Messenger.Default.Register<SaleDocument>(this, UpdateSelectedDocument);
            }
        }

        private void UpdateSelectedDocument(SaleDocument saleDocument)
        {
            SaleDocuments.Add(saleDocument);
            CurrentSaleDocument = saleDocument;
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
            set { 
                    _currentSaleDocument = value;
                    base.RaisePropertyChanged("CurrentSaleDocument");
                    
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
                    clientBalance.Text = "Клиент: \t" + CurrentSaleDocument.Client.Name +
                                                                 " Баланс клиента: " + CurrentSaleDocument.Client.Balance.ToString("c");
                    clientBalance.Margin = new Thickness(5, 5, 5, 5);
                    myPanel.Children.Add(clientBalance);

                    CheckBox checkBox = new CheckBox();
                    checkBox.Content = "Включить баланс в оплату";
                    checkBox.IsChecked = IncludeClientBalanceToTotal;
                    checkBox.Margin = new Thickness(5,5,5,5);
                    //string inclInTotalBalance = IncludeClientBalanceToTotal ? "Включить баланс в оплату: да" : "Включить баланс в оплату: нет";
                    myPanel.Children.Add(checkBox);

                    DataGrid dg = v as DataGrid;
                    DataGrid printDataGrid = new DataGrid();
                    printDataGrid.Margin = new Thickness(5,5,5,5);
                    printDataGrid.LoadingRow += LoadingRow;

                    //Style style = new Style{TargetType = typeof(DataGridColumnHeader)};
                    //style.Setters.Add(new Setter(FrameworkElement.WidthProperty, new DataGridLength(1.0, DataGridLengthUnitType.Auto)));
                    //style.Setters.Add(new Setter(DataGridColumnHeader.HorizontalAlignmentProperty, HorizontalAlignment.Center));
                    //printDataGrid.ColumnHeaderStyle = style;
                    printDataGrid.RowHeaderTemplate = dg.RowHeaderTemplate;
                    printDataGrid.RowHeaderWidth = dg.RowHeaderWidth;
                     
                    printDataGrid.Columns.Add(new DataGridTextColumn
                                                  {
                                                      Width = new DataGridLength(0.0, DataGridLengthUnitType.SizeToCells), 
                                                      Header = "#"
                                                  });
                    foreach (DataGridTextColumn item in dg.Columns)
                    {
                        printDataGrid.Columns.Add(new DataGridTextColumn
                                                      {
                                                          Width = new DataGridLength(1.0, DataGridLengthUnitType.Auto),
                                                          Header = item.Header,
                                                          Binding = item.Binding
                                                      });
                    }
                    
                    foreach (Product item in dg.Items)
                    {
                        printDataGrid.Items.Add(new Product
                                                    {
                                                        ID = item.ID,
                                                        Name = item.Name,
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
                });
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
                }
                else
                {
                    Prepay = true;
                    Confirmed = false;
                    UnConfirmDocument();
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

            CurrentSaleDocument.ClientId = CurrentClient.ID;
            CurrentSaleDocument.Prepay = Prepay;
            CurrentSaleDocument.Confirmed = Confirmed;
            
            if (IncludeClientBalanceToTotal && Confirmed)
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
            ProductsSelectorView productsSelector = new ProductsSelectorView();
            productsSelector.ShowDialog();
        }

        #endregion
    }
}
