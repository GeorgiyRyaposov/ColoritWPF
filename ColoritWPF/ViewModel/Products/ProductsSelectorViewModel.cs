using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Data;
using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace ColoritWPF.ViewModel.Products
{
    public class ProductsSelectorViewModel : ViewModelBase
    {
        private ColorITEntities colorItEntities;

        public ProductsSelectorViewModel()
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

        #region Properties

        public ObservableCollection<Product> Products { get; set; }
        public ObservableCollection<Product> SelectedProducts { get; set; }
        public ICollectionView ProductsView { get; private set; }

        private string _searchCriteria;
        public string SearchCriteria
        {
            get { return _searchCriteria; }
            set { _searchCriteria = value;
                base.RaisePropertyChanged("SearchCriteria");
                ProductsView.Refresh();
            }
        }

        private Product _removeSelecteProduct;
        public Product RemoveSelectedProduct
        {
            get { return _removeSelecteProduct; }
            set
            {
                _removeSelecteProduct = value;
                base.RaisePropertyChanged("RemoveSelectedProduct");
            }
        }

        private Product _selectedProduct;
        public Product SelectedProduct
        {
            get { return _selectedProduct; }
            set
            {
                _selectedProduct = value;
                base.RaisePropertyChanged("SelectedProduct");
            }
        }

        #endregion

        #region Methods

        private void GetData()
        {
            Products = new ObservableCollection<Product>(colorItEntities.Product.ToList());
            SelectedProducts = new ObservableCollection<Product>();

            ProductsView = CollectionViewSource.GetDefaultView(Products);
            ProductsView.Filter = ProductsFilter;
            ProductsView.GroupDescriptions.Add(new PropertyGroupDescription("Group"));
        }

        private bool ProductsFilter(object item)
        {
            if (String.IsNullOrEmpty(SearchCriteria))
                return true;
            Product product = item as Product;
            return product != null && product.Name.ToLower().Contains(SearchCriteria.ToLower());
        }

        public void NotifyWindowToClose()
        {
            Messenger.Default.Send<NotificationMessage>(
                new NotificationMessage(this, "CloseWindowsBoundToMe"));
        }

        #endregion

        #region Commands

        public RelayCommand AddProductToListCommand
        {
            get;
            private set;
        }

        public RelayCommand RemoveProductFromListCommand
        {
            get;
            private set;
        }

        public RelayCommand SendProductsListCommand
        {
            get;
            private set;
        }

        private void InitializeCommands()
        {
            AddProductToListCommand = new RelayCommand(AddProductToList);
            RemoveProductFromListCommand = new RelayCommand(RemoveProductFromList);
            SendProductsListCommand = new RelayCommand(SendProductsList);
        }

        private void AddProductToList()
        {
            if (SelectedProduct == null)
                return;

            if ((SelectedProduct.Warehouse + SelectedProduct.Storage) == 0)
                return;

            if (SelectedProducts.Contains(SelectedProduct))
            {
                Product prod = SelectedProducts.First(product => product.ID == SelectedProduct.ID);
                if (prod.Amount < (SelectedProduct.Warehouse + SelectedProduct.Storage))
                    prod.Amount++;
            }
            else
            {
                SelectedProduct.Amount++;
                SelectedProducts.Add(SelectedProduct);
            }
        }

        private void RemoveProductFromList()
        {
            SelectedProducts.Remove(RemoveSelectedProduct);
        }

        private void SendProductsList()
        {
            foreach (Product product in SelectedProducts)
            {
                Messenger.Default.Send<Product>(product);                
            }
            NotifyWindowToClose();
        }

        #endregion
    }
}
