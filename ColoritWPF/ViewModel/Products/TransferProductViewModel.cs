using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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
                Messenger.Reset();
                Messenger.Default.Register<Product>(this, selectedItem => ProductsList.Add(selectedItem));
            }
        }
        
        #region Properties
        
        public ObservableCollection<Product> ProductsList { get; set; }
        public ICollectionView ProductsView { get; private set; }
        public ObservableCollection<StorageModel> StorageList { get; set; }
        

        private string _sender;
        public string Sender
        {
            get { return _sender; }
            set
            {
                _sender = value;
                base.RaisePropertyChanged("Sender");
            }
        }

        private string _receiver;
        public string Receiver
        {
            get { return _receiver; }
            set
            {
                _receiver = value;
                base.RaisePropertyChanged("Receiver");
            }
        }
        
        private string _comment;
        public string Comment
        {
            get { return _comment; }
            set
            {
                _comment = value;
                base.RaisePropertyChanged("Comment");
            }
        }

        #endregion

        #region Methods

        private void GetData()
        {
            ProductsList = new ObservableCollection<Product>();
            ProductsView = CollectionViewSource.GetDefaultView(ProductsList);

            StorageList = new ObservableCollection<StorageModel>
                {
                    new StorageModel {Name = "Магазин", Value = "Storage"},
                    new StorageModel {Name = "Склад", Value = "Warehouse"}
                };
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
        
        private void InitializeCommands()
        {
            TransferSelectedProductCommand = new RelayCommand(TransferSelectedProduct, TransferSelectedCanExecute);
            SendProductsListCommand = new RelayCommand(SendProductsList);
            OpenSelectionCommand = new RelayCommand(OpenSelection);
        }

        private void TransferSelectedProduct()
        {
            try
            {
                foreach (Product product in ProductsList)
                {
                    bool toStorage = false;
                    bool toWarehouse = false;
                    var productToUpdate = colorItEntities.Product.First(pr => pr.ID == product.ID);
                    if (Sender == "Warehouse")
                    {
                        productToUpdate.Storage = productToUpdate.Storage + product.Amount;
                        productToUpdate.Warehouse = productToUpdate.Warehouse - product.Amount;
                        toStorage = true;
                    }

                    if (Sender == "Storage")
                    {
                        productToUpdate.Warehouse = productToUpdate.Warehouse + product.Amount;
                        productToUpdate.Storage = productToUpdate.Storage - product.Amount;
                        toWarehouse = true;
                    }

                    MoveProduct moveProduct = new MoveProduct
                    {
                        ProductID = product.ID,
                        ToStorage = toStorage,
                        ToWarehouse = toWarehouse,
                        Date = DateTime.Now,
                        Amount = product.Amount,
                        Comment = Comment
                    };

                    colorItEntities.MoveProduct.AddObject(moveProduct);

                    colorItEntities.SaveChanges();
                }
                ProductsList.Clear();
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось совершить перемещение\n"+ex.Message+"\n" + ex.InnerException);
            }
        }

        private void OpenSelection()
        {
            ProductsSelectorView productsSelector = new ProductsSelectorView();
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
                        if (Sender == Receiver)
                        {
                            result = "Склад отправитель должен отличаться от получателя";
                        }
                        if(Sender == null)
                            result = "Выберите склад отправитель";
                        break;
                    case "Receiver":
                        if (Receiver == Sender)
                        {
                            result = "Склад получатель должен отличаться от отправителя";
                        }
                        if (Receiver == null)
                            result = "Выберите склад получатель";
                        break;
                }
                return result;
            }
        }

        #endregion
    }
}
