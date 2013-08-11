using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using ColoritWPF.Models;
using ColoritWPF.Views.Products;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

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
                //Messenger.Reset();
                Messenger.Default.Register<MoveProductDocument>(this, moveProductDoc => TransferDocumentsList.Add(moveProductDoc));
            }
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

        #endregion

        #region Methods

        private void GetData()
        {
            ProductsList = new ObservableCollection<MoveProduct>();
            TransferDocumentsList = new ObservableCollection<MoveProductDocument>(colorItEntities.MoveProductDocument.ToList());

            _storage = new StorageModel {Name = "Магазин", Value = "Storage"};
            _warehouse = new StorageModel {Name = "Склад", Value = "Warehouse"};
            StorageList = new ObservableCollection<StorageModel>
                {
                    _storage,
                    _warehouse
                };
        }
        
        private void UpdateSenderReceiver()
        {
            if (_currentTransferDocument.ToStorage)
            {
                Sender = _warehouse;
                Receiver = _storage;
            }
            if(_currentTransferDocument.ToWarehouse)
            {
                Sender = _storage;
                Receiver = _warehouse;
            }
            base.RaisePropertyChanged("Sender");
            base.RaisePropertyChanged("Receiver");
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

        public RelayCommand SendProductsListCommand
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
        
        private void InitializeCommands()
        {
            TransferSelectedProductCommand = new RelayCommand(ApplyDocument, TransferSelectedCanExecute);
            SendProductsListCommand = new RelayCommand(SendProductsList);
            OpenSelectionCommand = new RelayCommand(OpenSelection);
            SaveCommand = new RelayCommand(SaveCmd);
        }

        private void ApplyDocument()
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

        private void SaveCmd()
        {
            if (Sender.Value == "Warehouse")
            {
                CurrentTransferDocument.ToStorage = true;
                CurrentTransferDocument.ToWarehouse = false;
            }

            if (Sender.Value == "Storage")
            {
                CurrentTransferDocument.ToStorage = false;
                CurrentTransferDocument.ToWarehouse = true;
            }

            foreach (MoveProduct product in ProductsList)
            {
                if (Sender.Value == "Warehouse")
                {
                    product.ToStorage = true;
                    product.ToWarehouse = false;
                }

                if (Sender.Value == "Storage")
                {
                    product.ToWarehouse = true;
                    product.ToStorage = false;
                }
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
            CurrentTransferDocument.Confirmed = true;

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
        }

        private void UnConfirmDocument()
        {
            CurrentTransferDocument.Confirmed = false;

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
        }

        private void OpenSelection()
        {
            TransferProductsSelector productsSelector = new TransferProductsSelector();
            productsSelector.ShowDialog();
        }

        private bool TransferSelectedCanExecute()
        {
            if (ProductsList.Count == 0)
                return false;

            return ProductsList != null;
        }
        
        private void SendProductsList()
        {
            //foreach (Product product in SelectedProducts)
            //{
            //    Messenger.Default.Send<Product>(product);
            //}
            //NotifyWindowToClose();
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
                        if (Sender == null)
                        {
                            result = "Выберите склад отправитель";
                            break;
                        }
                        if (Receiver != null)
                        {
                            if (Sender.Value == Receiver.Value)
                            {
                                result = "Склад отправитель должен отличаться от получателя";
                            }
                        }
                        break;
                    case "Receiver":
                        if (Receiver == null)
                        {
                            result = "Выберите склад получатель";
                            break;
                        }
                        if (Sender != null)
                        {
                            if (Receiver.Value == Sender.Value)
                            {
                                result = "Склад получатель должен отличаться от отправителя";
                            }
                        }
                        break;
                        
                }
                return result;
            }
        }

        #endregion
    }
}
