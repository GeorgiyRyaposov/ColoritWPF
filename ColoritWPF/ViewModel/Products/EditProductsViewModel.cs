using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Data;
using ColoritWPF.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace ColoritWPF.ViewModel.Products
{
    public class EditProductsViewModel : ViewModelBase
    {
        private ColorITEntities colorItEntities;

        public EditProductsViewModel()
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

        public ObservableCollection<Product> ProductsCollection { get; set; }
        public ObservableCollection<GroupByItem> GroupingList { get; set; }
        public ObservableCollection<Group> GroupCollection { get; set; }
        public ObservableCollection<Producers> ProducersCollection { get; set; }
        public ICollectionView ProductsView { get; private set; }

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

        private Product _selectedProduct;
        public Product SelectedProduct
        {
            get { return _selectedProduct; }
            set
            {
                _selectedProduct = value;
                GroupForSelectedProduct = value.Group1;
                SelectedProducer = value.Producers;
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

        private Group _groupForSelectedProduct;
        public Group GroupForSelectedProduct
        {
            get { return _groupForSelectedProduct; }
            set
            {
                _groupForSelectedProduct = value;
                base.RaisePropertyChanged("GroupForSelectedProduct");
            }
        }

        private Producers _selectedProducer;
        public Producers SelectedProducer
        {
            get { return _selectedProducer; }
            set
            {
                _selectedProducer = value;
                base.RaisePropertyChanged("SelectedProducer");
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
            ProductsCollection = new ObservableCollection<Product>(colorItEntities.Product.ToList());
            GroupCollection = new ObservableCollection<Group>(colorItEntities.Group.ToList());
            ProducersCollection = new ObservableCollection<Producers>(colorItEntities.Producers.ToList());
            SelectedProduct = new Product();

            GroupingList = new ObservableCollection<GroupByItem>
                               {
                                   new GroupByItem {Name = "Типу", Value = "Groups"},
                                   new GroupByItem
                                       {
                                           Name = "Производителю",
                                           Value = "ProducerGr"
                                       }
                               };

            ProductsView = CollectionViewSource.GetDefaultView(ProductsCollection);
            ProductsView.Filter = ProductsFilter;
            ProductsView.GroupDescriptions.Add(new PropertyGroupDescription("Groups"));
            SelectedGroup = "Groups";
        }

        private bool ProductsFilter(object item)
        {
            Product product = item as Product;

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

        #endregion

        #region Commands

        public RelayCommand SaveChangesCommand
        {
            get;
            private set;
        }

        private void InitializeCommands()
        {
            SaveChangesCommand = new RelayCommand(SaveChangesCmd, SaveChangesCommandCanExecute);
        }

        private bool SaveChangesCommandCanExecute()
        {
            return SelectedProduct != null;
        }

        private void SaveChangesCmd()
        {
            try
            {
                SelectedProduct.Group = GroupForSelectedProduct.ID;
                SelectedProduct.Group1 = GroupForSelectedProduct;
                SelectedProduct.ProducerId = SelectedProducer.Id;

                colorItEntities.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось сохранить изменения\n"+ex.Message + "\n"+ex.InnerException);
            }
            ProductsView.Refresh();
        }

        #endregion
    }
}
