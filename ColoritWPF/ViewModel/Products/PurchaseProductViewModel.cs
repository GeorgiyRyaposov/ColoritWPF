using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace ColoritWPF.ViewModel.Products
{
    public class PurchaseProductViewModel : ViewModelBase
    {
        public PurchaseProductViewModel()
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


                PurchaseDocuments.CollectionChanged += CollectionChanged;
                Messenger.Default.Register<PurchaseDocument>(this, UpdateSelectedDocument);
            }
        }

        #region Properties
        private ColorITEntities colorItEntities;
        private bool _prepay, _confirm;


        public ObservableCollection<Client> Clients { get; set; }
        public ObservableCollection<Purchase> PurchaseProductsList { get; set; }
        public ObservableCollection<PurchaseDocument> PurchaseDocuments { get; set; }

        private PurchaseDocument _currentPurchaseDocument;
        public PurchaseDocument CurrentPurchaseDocument
        {
            get { return _currentPurchaseDocument; }
            set
            {
                _currentPurchaseDocument = value;
                base.RaisePropertyChanged("CurrentPurchaseDocument");

                CurrentClient = _currentPurchaseDocument.Client;

                _confirmButtonContent = value.Confirmed ? "Разпровести" : "Провести";
                base.RaisePropertyChanged("ConfirmButtonContent");

                //При смене документа отчищаем список продуктов и перезаполняем.. да, ужас.. но я пока хз как лучше сделать
                //а все потому что на изменения SaleProductsList подписка слушателя, которая сбрасывается
                //если оставить как было: SaleProductsList = new ObservableCollection<Sale>(_currentSaleDocument.Sale.ToList());
                PurchaseProductsList.Clear();
                var tempCollection = new ObservableCollection<Purchase>(_currentPurchaseDocument.Purchase.ToList());
                foreach (Purchase purchase in tempCollection)
                {
                    PurchaseProductsList.Add(purchase);
                }

                base.RaisePropertyChanged("PurchaseProductsList");

                IsEnabled = !_currentPurchaseDocument.Confirmed;
            }
        }

        private string _confirmButtonContent = "Провести";
        public string ConfirmButtonContent
        {
            get
            {
                return _confirmButtonContent;
            }
            set
            {
                _confirmButtonContent = value;
                base.RaisePropertyChanged("ConfirmButtonContent");
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

        private Client _currentClient;
        public Client CurrentClient
        {
            get
            {
                if (CurrentPurchaseDocument == null)
                    return Clients.First(client => client.PrivatePerson);
                return _currentClient;
            }
            set
            {
                CurrentPurchaseDocument.Client = value;
                _currentClient = value;
            }
        }

        #endregion //Properties

        #region Methods

        private void UpdateSelectedDocument(PurchaseDocument purchaseDocument)
        {
            PurchaseDocuments.Add(purchaseDocument);
            CurrentPurchaseDocument = purchaseDocument;
        }

        private void GetData()
        {
            Clients = new ObservableCollection<Client>(colorItEntities.Client.ToList());
            PurchaseProductsList = new ObservableCollection<Purchase>();
            PurchaseDocuments = new ObservableCollection<PurchaseDocument>(colorItEntities.PurchaseDocument.ToList());
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
        }

        //А этот метод уже делает что надо когда узнает что элемент был изменен
        public void Recalc(object sender, PropertyChangedEventArgs e)
        {
            CurrentPurchaseDocument.CleanTotal = 0;
            CurrentPurchaseDocument.Total = 0;
            foreach (Purchase product in PurchaseProductsList)
            {
                CurrentPurchaseDocument.CleanTotal += product.CleanTotal;
                CurrentPurchaseDocument.Total += product.Total;
            }
        }
        public void Recalc()
        {
            if (CurrentPurchaseDocument != null)
            {
                CurrentPurchaseDocument.CleanTotal = 0;
                CurrentPurchaseDocument.Total = 0;
                foreach (Purchase purchaseProduct in PurchaseProductsList)
                {
                    CurrentPurchaseDocument.CleanTotal += purchaseProduct.CleanTotal;
                    CurrentPurchaseDocument.Total += purchaseProduct.Total;
                }
            }
        }

        #endregion //Methods

        #region Commands

        public RelayCommand OpenSelectionCommand
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

        public RelayCommand<Visual> PrintCommand
        {
            get
            {
                return new RelayCommand<Visual>(v =>
                {
                    PrintDialog printDlg = new PrintDialog();

                    if (printDlg.ShowDialog() != true)
                    { return; }

                    StackPanel myPanel = new StackPanel();
                    myPanel.Margin = new Thickness(5, 5, 5, 5);

                    TextBlock myBlock = new TextBlock();
                    myBlock.Text = "Приходная накладная №" + CurrentPurchaseDocument.DocumentNumber +
                                                                 " от " + CurrentPurchaseDocument.Date;
                    myBlock.TextAlignment = TextAlignment.Left;
                    myBlock.Margin = new Thickness(5, 5, 5, 5);
                    myPanel.Children.Add(myBlock);

                    TextBlock clientBalance = new TextBlock();
                    clientBalance.Text = "Клиент: \t" + CurrentPurchaseDocument.Client.Name +
                                                                 " Баланс клиента: " + CurrentPurchaseDocument.Client.Balance.ToString("c");
                    clientBalance.Margin = new Thickness(5, 5, 5, 5);
                    myPanel.Children.Add(clientBalance);

                    DataGrid dg = v as DataGrid;
                    DataGrid printDataGrid = new DataGrid();
                    printDataGrid.Margin = new Thickness(5, 5, 5, 5);
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
                    totalValue.Text = "Итого: " + CurrentPurchaseDocument.Total.ToString("c");
                    totalValue.Margin = new Thickness(5, 5, 5, 5);
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
            PrepayDocumentCommand = new RelayCommand(PrepayDocument);
        }

        private void OpenSelection()
        {
            throw new NotImplementedException();
        }

        private bool ConfirmDocumentCanExecute()
        {
            return CurrentPurchaseDocument != null;
        }

        private void PrepayDocument()
        {
            Save(true, false, false);
        }

        private void ConfirmDocument()
        {
            if (MessageBox.Show("Вы уверены что хотите " + ConfirmButtonContent.ToLower() + " документ?",
                                "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (!CurrentPurchaseDocument.Confirmed)
                {
                    Save(false,true, false);
                    IsEnabled = false;
                }
                else
                {
                    Save(false,false,true);
                    IsEnabled = true;
                }
            }
        }

        /// <summary>
        /// Используется при проведении и сохранении документа, в зависимости от значений Confirmed и Prepay
        /// </summary>
        private void Save(bool prepay, bool confirm, bool unConfirm)
        {
            PurchaseDocument purchaseDocumentToUpd =
                colorItEntities.PurchaseDocument.First(doc => doc.Id == CurrentPurchaseDocument.Id);
            purchaseDocumentToUpd.ClientId = CurrentClient.ID;
            purchaseDocumentToUpd.Total = CurrentPurchaseDocument.Total;
            purchaseDocumentToUpd.CleanTotal = CurrentPurchaseDocument.CleanTotal;
            purchaseDocumentToUpd.Contractor = CurrentClient.Name;
            purchaseDocumentToUpd.Confirmed = confirm;
            purchaseDocumentToUpd.Prepay = prepay;
            purchaseDocumentToUpd.Comment = CurrentPurchaseDocument.Comment;

            CurrentPurchaseDocument.ClientId = CurrentClient.ID;
            CurrentPurchaseDocument.Prepay = prepay;
            CurrentPurchaseDocument.Confirmed = confirm;

            if (confirm && !CurrentClient.PrivatePerson)
            {
                Client client = colorItEntities.Client.First(cl => cl.ID == CurrentClient.ID);
                client.Balance += CurrentPurchaseDocument.CleanTotal;
            }

            try
            {
                colorItEntities.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось сохранить документ покупок\n" + ex.Message + "\n" + ex.InnerException);
            }

            if (prepay)
            {
                foreach (Purchase purchaseProduct in PurchaseProductsList)
                {
                    Purchase purchaseToUpd = colorItEntities.Purchase.First(item => item.ID == purchaseProduct.ID);
                    purchaseToUpd.Amount = purchaseProduct.Amount;
                    purchaseToUpd.Cost = purchaseProduct.Cost;
                }
            }

            if (confirm)
            {
                foreach (Purchase purchaseProduct in PurchaseProductsList)
                {
                    Purchase purchaseToUpd = colorItEntities.Purchase.First(item => item.ID == purchaseProduct.ID);
                    purchaseToUpd.Amount = purchaseProduct.Amount;
                    purchaseToUpd.Cost = purchaseProduct.Cost;

                    var productStorage = colorItEntities.Product.First(pr => pr.ID == purchaseProduct.Product.ID);
                    productStorage.Warehouse += purchaseProduct.Amount;
                }
            }

            if(unConfirm)
            {
                foreach (Purchase purchaseProduct in PurchaseProductsList)
                {
                    Purchase purchaseToUpd = colorItEntities.Purchase.First(item => item.ID == purchaseProduct.ID);
                    purchaseToUpd.Amount = purchaseProduct.Amount;
                    purchaseToUpd.Cost = purchaseProduct.Cost;

                    var productStorage = colorItEntities.Product.First(pr => pr.ID == purchaseProduct.Product.ID);
                    productStorage.Warehouse -= purchaseProduct.Amount;
                }
            }

            try
            {
                colorItEntities.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось сохранить список покупок\n" + ex.Message + "\n" + ex.InnerException);
            }
        }

        #endregion //Commands

    }
}
