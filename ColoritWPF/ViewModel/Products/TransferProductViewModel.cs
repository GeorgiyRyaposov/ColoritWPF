using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ColoritWPF.Models;
using ColoritWPF.Views.Products;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace ColoritWPF.ViewModel.Products
{
    public class TransferProductViewModel : ViewModelBase, IDataErrorInfo
    {
        private ColorITEntities colorItEntities;

        public TransferProductViewModel()
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
            }
        }

        private void AddToTransferDocuments(MoveProductDocument moveProductDocument)
        {
            TransferDocumentsList.Add(moveProductDocument);
            CurrentTransferDocument = moveProductDocument;
        }

        #region Properties
        
        public ObservableCollection<MoveProduct> ProductsList { get; set; }
        public ObservableCollection<StorageModel> StorageList { get; set; }
        public ObservableCollection<MoveProductDocument> TransferDocumentsList { get; set; }

        private MoveProductDocument _currentTransferDocument = new MoveProductDocument();
        public MoveProductDocument CurrentTransferDocument
        {
            get { return _currentTransferDocument; }
            set
            {
                _currentTransferDocument = value;

                ProductsList.Clear();
                ProductsList = new ObservableCollection<MoveProduct>(_currentTransferDocument.MoveProduct.ToList());

                base.RaisePropertyChanged("CurrentTransferDocument");
                base.RaisePropertyChanged("ProductsList");

                UpdateSenderReceiver();

                _confirmButtonContent = value.Confirmed ? "Разпровести" : "Провести";
                base.RaisePropertyChanged("ConfirmButtonContent");

                IsEnabled = !value.Confirmed;
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

        private StorageModel _sender;
        public StorageModel Sender
        {
            get { return _sender; }
            set
            {
                _sender = value;
                base.RaisePropertyChanged("Sender");
                CurrentTransferDocument.Sender = value.Name;
            }
        }

        private StorageModel _receiver;
        public StorageModel Receiver
        {
            get { return _receiver; }
            set
            {
                _receiver = value;
                base.RaisePropertyChanged("Receiver");
                CurrentTransferDocument.Receiver = value.Name;
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

        private StorageModel _storage;
        private StorageModel _warehouse;
        private StorageModel _none;
        #endregion

        #region Methods

        private void GetData()
        {
            ProductsList = new ObservableCollection<MoveProduct>();
            TransferDocumentsList = new ObservableCollection<MoveProductDocument>(colorItEntities.MoveProductDocument.ToList());

            _storage = new StorageModel {Name = "Магазин", Value = "Storage"};
            _warehouse = new StorageModel {Name = "Склад", Value = "Warehouse"};
            _none = new StorageModel { Name = "Не выбран", Value = "None" };
            StorageList = new ObservableCollection<StorageModel>
                {
                    _storage,
                    _warehouse,
                    _none
                };
        }
        
        private void UpdateSenderReceiver()
        {
            if (_currentTransferDocument.ToStorage)
            {
                Sender = _warehouse;
                Receiver = _storage;
            }
            else if(_currentTransferDocument.ToWarehouse)
            {
                Sender = _storage;
                Receiver = _warehouse;
            }
            else if (_currentTransferDocument.ToWarehouse == _currentTransferDocument.ToStorage)
            {
                Sender = _none;
                Receiver = _none;
            }
        }

        /// <summary>
        /// Проверяет наличие каждого товара из списка
        /// </summary>
        /// <returns>Возвращает false если товара в магазине\на складе не достаточно</returns>
        private bool IsAllProductInStock()
        {
            ObservableCollection<Product> notEnough = new ObservableCollection<Product>();

            bool toStorage = false; 
            bool toWarehouse = false;
            if (Receiver.Value == "Storage")
                toStorage = true;
            else
                toWarehouse = true;

            if (toStorage)
            {
                foreach (MoveProduct product in ProductsList)
                {
                    if (product.Product.Warehouse < product.Amount)
                        notEnough.Add(product.Product);
                }
            }
            if (toWarehouse)
            {

                foreach (MoveProduct product in ProductsList)
                {
                    if (product.Product.Storage < product.Amount)
                        notEnough.Add(product.Product);
                }
            }

            if (notEnough.Count > 0)
            {
                string msgText = String.Format("Недостаточно товара в "+ Sender.Value.ToLower() +"е:\n");
                msgText = notEnough.Aggregate(msgText, (current, product) => current + product.Name + "\n");

                MessageBox.Show(msgText, "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        #endregion

        #region Commands

        public RelayCommand TransferSelectedProductCommand
        {
            get;
            private set;
        }
        
        public RelayCommand OpenSelectionCommand
        {
            get;
            private set;
        }

        public RelayCommand SaveCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Command executed to print an visual component. The component is passed in as a parameter.
        /// </summary>
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

                    TextBlock myBlock = new TextBlock
                        {
                            Text = "Перемещение № " + CurrentTransferDocument.DisplayDocumentNumber +
                                   " от " + CurrentTransferDocument.Date,
                            TextAlignment = TextAlignment.Left,
                            Margin = new Thickness(5, 5, 5, 5)
                        };
                    myPanel.Children.Add(myBlock);

                    
                    TextBlock senderTxtBx = new TextBlock
                        {
                            Text = "Склад отправитель: " + CurrentTransferDocument.Sender,
                            TextAlignment = TextAlignment.Left,
                            Margin = new Thickness(5, 5, 5, 5)
                        };
                    myPanel.Children.Add(senderTxtBx);

                    TextBlock receiverTxtBx = new TextBlock
                    {
                        Text = "Склад получатель: " + CurrentTransferDocument.Receiver,
                        TextAlignment = TextAlignment.Left,
                        Margin = new Thickness(5, 5, 5, 5)
                    };
                    myPanel.Children.Add(receiverTxtBx);

                    TextBlock commentaryTxtBx = new TextBlock
                    {
                        Text = "Комментарии: " + CurrentTransferDocument.Comment,
                        TextAlignment = TextAlignment.Left,
                        Margin = new Thickness(5, 5, 5, 5)
                    };
                    myPanel.Children.Add(commentaryTxtBx);

                    DataGrid dg = v as DataGrid;
                    DataGrid printDataGrid = new DataGrid {Margin = new Thickness(5, 5, 5, 5)};
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

                    foreach (MoveProduct item in dg.Items)
                    {
                        printDataGrid.Items.Add(new MoveProduct
                        {
                            ID = item.ID,
                            ProductID = item.ProductID,
                            Amount = item.Amount,
                            DocNumber = item.DocNumber,
                            Product = item.Product
                        });
                    }

                    myPanel.Children.Add(printDataGrid);
                    
                    Grid grid = new Grid();
                    ColumnDefinition columnDefinition = new ColumnDefinition();
                    columnDefinition.Width = new GridLength(1.0, GridUnitType.Auto);
                    grid.Children.Add(myPanel);

                    grid.ColumnDefinitions.Add(columnDefinition);
                    printDlg.PrintVisual(grid, "Перемещение");
                });
            }
        }

        private void LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
        
        private void InitializeCommands()
        {
            TransferSelectedProductCommand = new RelayCommand(ApplyDocument, TransferSelectedCanExecute);
            OpenSelectionCommand = new RelayCommand(OpenSelection);
            SaveCommand = new RelayCommand(SaveCmd);
        }

        private void SaveCmd()
        {
            Save(false); // Сохранить не проведенным
        }

        private void ApplyDocument()
        {
            if (Receiver.Value == Sender.Value)
            {
                MessageBox.Show("Склад получатель и склад отправитель должны быть разными", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                if (MessageBox.Show("Вы уверены что хотите " + ConfirmButtonContent.ToLower() + " документ?",
                                    "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    if (IsAllProductInStock())
                    {
                        if(!CurrentTransferDocument.Confirmed)
                            ConfirmDocument();
                        else
                            UnConfirmDocument();
                    }
                }
            }
        }

        private void Save(bool isConfirmed)
        {
            var productDocToUpdate = colorItEntities.MoveProductDocument.First(item => item.Id == CurrentTransferDocument.Id);
            productDocToUpdate.Comment = CurrentTransferDocument.Comment;
            productDocToUpdate.Confirmed = isConfirmed;
            CurrentTransferDocument.Confirmed = isConfirmed;

            if (Sender.Value == "Warehouse")
            {
                productDocToUpdate.ToStorage = true;
                productDocToUpdate.ToWarehouse = false;
                CurrentTransferDocument.ToStorage = true;
                CurrentTransferDocument.ToWarehouse = false;
            }

            if (Sender.Value == "Storage")
            {
                productDocToUpdate.ToStorage = false;
                productDocToUpdate.ToWarehouse = true;
                CurrentTransferDocument.ToStorage = false;
                CurrentTransferDocument.ToWarehouse = true;
            }
            
            try
            {
                colorItEntities.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось сохранить изменения\n" + ex.Message + "\n" + ex.InnerException);
            }
        }

        private void ConfirmDocument()
        {
            Save(true);

            foreach (MoveProduct moveProduct in ProductsList)
            {
                var productToUpdate = colorItEntities.Product.First(pr => pr.ID == moveProduct.Product.ID);
                if (Sender.Value == "Warehouse")
                {
                    productToUpdate.Storage = productToUpdate.Storage + moveProduct.Amount;
                    productToUpdate.Warehouse = productToUpdate.Warehouse - moveProduct.Amount;
                }

                if (Sender.Value == "Storage")
                {
                    productToUpdate.Warehouse = productToUpdate.Warehouse + moveProduct.Amount;
                    productToUpdate.Storage = productToUpdate.Storage - moveProduct.Amount;
                }
            }
            try
            {
                colorItEntities.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось совершить перемещение\n"+ex.Message+"\n" + ex.InnerException);
            }

            IsEnabled = false;
        }

        private void UnConfirmDocument()
        {
            Save(false);

            foreach (MoveProduct product in ProductsList)
            {
                var productToUpdate = colorItEntities.Product.First(pr => pr.ID == product.ID);
                if (Sender.Value == "Warehouse")
                {
                    productToUpdate.Storage = productToUpdate.Storage - product.Amount;
                    productToUpdate.Warehouse = productToUpdate.Warehouse + product.Amount;
                }

                if (Sender.Value == "Storage")
                {
                    productToUpdate.Warehouse = productToUpdate.Warehouse - product.Amount;
                    productToUpdate.Storage = productToUpdate.Storage + product.Amount;
                }
            }
            try
            {
                colorItEntities.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось совершить перемещение\n" + ex.Message + "\n" + ex.InnerException);
            }

            IsEnabled = true;
        }

        private void OpenSelection()
        {
//            TransferProductsSelector productsSelector = new TransferProductsSelector();
//            productsSelector.ShowDialog();
            var dlg = new UniProductSelectorView();
            dlg.Show();
            dlg.Closed += OnProductSelectorClosed;
        }

        private void OnProductSelectorClosed(object sender, EventArgs e)
        {
            var dlg = sender as UniProductSelectorView;
            var vm = dlg.DataContext as UniProductSelectorViewModel;
            if (vm == null || vm.SelectedProducts.Count == 0)
                return;

            MoveProductDocument moveProductDocument = new MoveProductDocument();
            moveProductDocument.GenerateDocNumber();
            moveProductDocument.Date = DateTime.Now;
            moveProductDocument.Confirmed = false;

            colorItEntities.MoveProductDocument.AddObject(moveProductDocument);

            try
            {
                colorItEntities.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось сохранить документ в базу\n" + ex.Message + "\n" + ex.InnerException);
            }

            foreach (Product selectedProduct in vm.SelectedProducts)
            {
                MoveProduct moveProduct = new MoveProduct
                {
                    ProductID = selectedProduct.ID,
                    Amount = selectedProduct.Amount,
                    DocNumber = moveProductDocument.Id
                };

                colorItEntities.MoveProduct.AddObject(moveProduct);
            }

            try
            {
                colorItEntities.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось сохранить список перемещаемых продуктов в базу\n" + ex.Message + "\n" + ex.InnerException);
            }
        }

        private bool TransferSelectedCanExecute()
        {
            if (ProductsList.Count == 0)
                return false;

            return ProductsList != null;
        }
        
        #endregion

        #region IDataErrorInfo Members

        public string Error
        {
            get { throw new NotImplementedException(); }
        }

        public string this[string columnName]
        {
            get
            {
                string result = null;
                switch (columnName)
                {
                    case "Sender":
                        if (Sender == null || Sender == _none)
                        {
                            result = "Выберите склад отправитель";
                        }
                        break;
                    case "Receiver":
                        if (Receiver == null || Receiver == _none)
                        {
                            result = "Выберите склад получатель";
                        }
                        break;
                        
                }
                return result;
            }
        }

        #endregion
    }
}
