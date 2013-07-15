using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
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

        private string _comments;
        public string Comments
        {
            get { return _comments; }
            set
            {
                _comments = value;
                base.RaisePropertyChanged("Comments");
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

        private void InitializeCommands()
        {
            OpenSelectionCommand = new RelayCommand(OpenSelection);
            PrintDocumentCommand = new RelayCommand(PrintDocument);
            ConfirmDocumentCommand = new RelayCommand(ConfirmDocument);
        }

        private void ConfirmDocument()
        {
            Prepay = false;
            Confirmed = true;
            SaveDocument();
        }

        private void SaveDocument()
        {
            if (IsAllProductInStock() == false)
                return;

            int num = 0;

            var previousSaleDoc = (from n in colorItEntities.SaleDocument
                                   orderby n.Id descending
                                   select n).FirstOrDefault();

            if (previousSaleDoc == null)
                previousSaleDoc = new SaleDocument{SaleListNumber = 0};

            num = previousSaleDoc.SaleListNumber;
            
            if (previousSaleDoc.DateCreated.Month != DateTime.Now.Month)
            {
                num = 0;
            }
            
            num++;

            SaleDocument saleDocument = new SaleDocument
            {
                ClientId = CurrentClient.ID,
                SaleListNumber = num,
                Total = Sum,
                CleanTotal = CleanSum,
                Comments = Comments,
                Prepay = Prepay,
                Confirmed = Confirmed,
                DateCreated = DateTime.Now
            };

            colorItEntities.SaleDocument.AddObject(saleDocument);

            try
            {
                colorItEntities.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось сохранить список продаж\n" + ex.Message + "\n" + ex.InnerException);
            }
            
            foreach (Product product in Products)
            {
                Sale sale = new Sale
                {
                    ProductID = product.ID,
                    Amount = product.Amount,
                    Discount = product.CurrentDiscount,
                    Date = DateTime.Now,
                    ClientID = CurrentClient.ID,
                    FromStorage = product.Storage,
                    FromWareHouse = product.Warehouse,
                    Cost = product.Cost,
                    SaleListNumber = saleDocument.Id,
                };


                product.Storage = product.Storage - product.Amount;
                
                colorItEntities.Sale.AddObject(sale);
            }

            try
            {
                colorItEntities.SaveChanges();
                Products.Clear();
                Comments = String.Empty;
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось сохранить список продаж\n"+ex.Message+"\n"+ex.InnerException);
            }
        }

        /// <summary>
        /// Проверяет наличие каждого товара из списка
        /// </summary>
        /// <returns>Возвращает false если товара в магазине не достаточно</returns>
        private bool IsAllProductInStock()
        {
            ObservableCollection<Product> notEnough = new ObservableCollection<Product>();
            foreach (Product product in Products)
            {
                if (product.Storage < product.Amount)
                    notEnough.Add(product);
            }

            if (notEnough.Count > 0)
            {
                string msgText = String.Format("Недостаточно товара в магазине:\n");
                msgText = notEnough.Aggregate(msgText, (current, product) => current + product.Name + "\n");

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

        private void PrintDocument()
        {

        }
        #endregion
    }

   
}
