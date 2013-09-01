using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using ColoritWPF.Models;
using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace ColoritWPF.ViewModel.Products
{
    public class PurchaseProductsSelectorViewModel : ViewModelBase
    {
        public PurchaseProductsSelectorViewModel()
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
        public ObservableCollection<GroupByItem> GroupingList { get; set; }
        public ICollectionView ProductsView { get; private set; }

        private ColorITEntities colorItEntities;

        private string _searchCriteria;
        public string SearchCriteria
        {
            get { return _searchCriteria; }
            set
            {
                _searchCriteria = value;
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

        private string _selectedGroup;
        public string SelectedGroup
        {
            get { return _selectedGroup; }
            set
            {
                _selectedGroup = value;
                base.RaisePropertyChanged("SelectedGroup");
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
                base.RaisePropertyChanged("InStock");
                ProductsView.Refresh();
            }
        }

        #endregion

        #region Methods

        private void GetData()
        {
            Products = new ObservableCollection<Product>(colorItEntities.Product.ToList());
            SelectedProducts = new ObservableCollection<Product>();

            GroupingList = new ObservableCollection<GroupByItem>();
            GroupingList.Add(new GroupByItem { Name = "Типу", Value = "Groups" });
            GroupingList.Add(new GroupByItem { Name = "Производителю", Value = "ProducerGr" });

            ProductsView = CollectionViewSource.GetDefaultView(Products);
            ProductsView.Filter = ProductsFilter;
            ProductsView.GroupDescriptions.Add(new PropertyGroupDescription("Groups"));
            SelectedGroup = "Groups";
        }

        private bool ProductsFilter(object item)
        {
            Product product = item as Product;

            if (String.IsNullOrEmpty(SearchCriteria))
                return isProductInStock(product);

            return product != null &&
                product.Name.ToLower().Contains(SearchCriteria.ToLower()) &&
                isProductInStock(product);
        }

        private bool isProductInStock(Product product)
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
            Client defaultClient = colorItEntities.Client.First(client => client.PrivatePerson);

            PurchaseDocument purchaseDocument = new PurchaseDocument
            {
                Date = DateTime.Now,
                Confirmed = false,
                Prepay = false,
                Client = defaultClient,
                ClientId = defaultClient.ID
            };
            purchaseDocument.GenerateDocNumber();
            colorItEntities.PurchaseDocument.AddObject(purchaseDocument);

            try
            {
                colorItEntities.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось сохранить документ в базу\n" + ex.Message + "\n" + ex.InnerException);
            }

            foreach (Product product in SelectedProducts)
            {
                Purchase purchaseProduct = new Purchase
                {
                    ProductID = product.ID,
                    ListNumber = purchaseDocument.Id,
                    Amount = product.Amount,
                    Cost = product.Cost,
                    SelfCost = product.SelfCost
                };
                colorItEntities.Purchase.AddObject(purchaseProduct);
            }

            try
            {
                colorItEntities.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось сохранить список продаваемых продуктов в базу\n" + ex.Message + "\n" + ex.InnerException);
            }

            Messenger.Default.Send<PurchaseDocument>(purchaseDocument);

            NotifyWindowToClose();
        }

        #endregion
    }
}
