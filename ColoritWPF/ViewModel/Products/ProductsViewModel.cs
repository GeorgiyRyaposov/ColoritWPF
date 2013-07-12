using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
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
                Messenger.Default.Register<Product>(this, selectedItem => Products.Add(selectedItem));

                Products.CollectionChanged += CollectionChanged;
            }
        }
        
        #region Properties

        public ObservableCollection<Client> Clients { get; set; }
        public ObservableCollection<Product> Products { get; set; }

        private Client _currentClient;
        public Client CurrentClient
        {
            get { return _currentClient; }
            set { _currentClient = value;
                base.RaisePropertyChanged("CurrentClient");
                UpdateDiscountValue();
            }
        }

        private decimal _cleanSum;
        public decimal CleanSum
        {
            get
            {
                return _cleanSum;
            }
            set { _cleanSum = value;
            base.RaisePropertyChanged("CleanSum");
            }
        }

        private decimal _sum;
        public decimal Sum
        {
            get { return _sum; }
            set { _sum = value;
            base.RaisePropertyChanged("Sum");
            }
        }

        #endregion

        #region Methods

        private void GetData()
        {
            Clients = new ObservableCollection<Client>(colorItEntities.Client.ToList());
            Products = new ObservableCollection<Product>();
            CurrentClient = Clients.First(client => client.PrivatePerson);
        }

        //Этот метод позволяет узнать когда изменился элемент коллекции
        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (Product item in e.OldItems)
                {
                    //Removed items
                    item.PropertyChanged -= Recalc;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (Product item in e.NewItems)
                {
                    //Added items
                    item.PropertyChanged += Recalc;
                }
            }

            UpdateDiscountValue();
        }
        //А этот метод уже делает что надо когда узнает что элемент был изменен
        public void Recalc(object sender, PropertyChangedEventArgs e)
        {
            CleanSum = 0;
            Sum = 0;
            foreach (Product product in Products)
            {
                CleanSum += product.CleanTotal;
                Sum += product.Total;
            }
        }

        /// <summary>
        /// Обновляет значение скидки для каждого продукта
        /// </summary>
        private void UpdateDiscountValue()
        {
            if (CurrentClient == null)
                return;

            foreach (Product product in Products)
            {
                product.CurrentDiscount = CurrentClient.Discount;
            }
        }

        #endregion

        #region Commands

        public RelayCommand OpenSelectionCommand
        {
            get;
            private set;
        }

        private void InitializeCommands()
        {
            OpenSelectionCommand = new RelayCommand(OpenSelection);
        }

        private void OpenSelection()
        {
            ProductsSelectorView productsSelector = new ProductsSelectorView();
            productsSelector.ShowDialog();
        }
        #endregion
    }
}
