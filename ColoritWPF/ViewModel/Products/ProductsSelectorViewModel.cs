using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using ColoritWPF.Common;
using ColoritWPF.Models;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace ColoritWPF.ViewModel.Products
{
    public class ProductsSelectorViewModel : BaseVmWithBlls
    {
        public ProductsSelectorViewModel()
        {
            GetData();
            InitializeCommands();
        }

        #region Properties

        public ObservableCollection<Product> Products { get; set; }
        public ObservableCollection<Product> SelectedProducts { get; set; }
        public ObservableCollection<GroupByItem> GroupingList { get; set; }
        public ICollectionView ProductsView { get; private set; }

        private string _searchCriteria;
        public string SearchCriteria
        {
            get { return _searchCriteria; }
            set { _searchCriteria = value;
                OnPropertyChanged("SearchCriteria");
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
                OnPropertyChanged("RemoveSelectedProduct");
            }
        }

        private Product _selectedProduct;
        public Product SelectedProduct
        {
            get { return _selectedProduct; }
            set
            {
                _selectedProduct = value;
                OnPropertyChanged("SelectedProduct");
            }
        }

        private string _selectedGroup;
        public string SelectedGroup
        {
            get { return _selectedGroup; }
            set 
            { 
                _selectedGroup = value;
                OnPropertyChanged("SelectedGroup");
                ProductsView.GroupDescriptions.Clear();
                ProductsView.GroupDescriptions.Add(new PropertyGroupDescription(value));
            }
        }

        private bool _inStock;
        public bool InStock
        {
            get { return _inStock; }
            set
            {
                _inStock = value;
                OnPropertyChanged("InStock");
                ProductsView.Refresh();
            }
        }

        #endregion

        #region Methods

        private void GetData()
        {
            Products = new ObservableCollection<Product>(ProductsBll.GetProducts());
            SelectedProducts = new ObservableCollection<Product>();
            
            GroupingList = new ObservableCollection<GroupByItem>
            {
                new GroupByItem {Name = "Типу", Value = "Groups"},
                new GroupByItem {Name = "Производителю", Value = "ProducerGr"}
            };

            ProductsView = CollectionViewSource.GetDefaultView(Products);
            ProductsView.Filter = ProductsFilter;
            ProductsView.GroupDescriptions.Add(new PropertyGroupDescription("Groups"));
            SelectedGroup = "Groups";
        }

        private bool ProductsFilter(object item)
        {
            var product = item as Product;

            if (String.IsNullOrEmpty(SearchCriteria))
                return IsProductInStock(product);

            return product != null && 
                product.Name.ToLower().Contains(SearchCriteria.ToLower()) &&
                IsProductInStock(product);
        }

        private bool IsProductInStock(Product product)
        {
            if (InStock)
            {
                if (product.Warehouse + product.Storage > 0)
                    return true;
                return false;
            }
            return true;
        }

        public void NotifyWindowToClose()
        {
            Messenger.Default.Send(
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
            SendProductsListCommand = new RelayCommand(SendProductsList, SendProductsListCanExecute);
        }

        private bool SendProductsListCanExecute()
        {
            if (SelectedProducts.Count == 0)
                return false;
            return true;
        }

        private void AddProductToList()
        {
            if (SelectedProduct == null)
                return;

            if (SelectedProducts.Contains(SelectedProduct))
            {
                Product prod = SelectedProducts.First(product => product.ID == SelectedProduct.ID);
                if (prod.Amount < (SelectedProduct.Warehouse + SelectedProduct.Storage))
                    prod.Amount++;
            }
            else
            {
                if (SelectedProduct.Warehouse != 0 || SelectedProduct.Storage != 0)
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
            Messenger.Default.Send(SelectedProduct);                
        }

        #endregion
    }
}
