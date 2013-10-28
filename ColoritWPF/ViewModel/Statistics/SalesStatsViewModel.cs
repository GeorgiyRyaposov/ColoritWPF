using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Objects;
using System.Linq;
using System.Text;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace ColoritWPF.ViewModel.Statistics
{
    public class SalesStatsViewModel : ViewModelBase
    {
        public SalesStatsViewModel()
        {
            if (IsInDesignMode)
            {
                
            }
            else
            {
                _colorItEntities = new ColorITEntities();

                StartDate = DateTime.Today;
                EndDate = DateTime.Today;

                ListOfSoldProducts = new ObservableCollection<Sale>();

//                var listOfSaleDocs =
//                    _colorItEntities.SaleDocument.Where(saleDoc => saleDoc.DateCreated.Date == DateTime.Today.Date).ToList();
//
//                ListOfSoldProducts = new ObservableCollection<Sale>();
//                foreach (SaleDocument saleDoc in listOfSaleDocs)
//                {
//                    var soldProduct =
//                        _colorItEntities.Sale.FirstOrDefault(sale => sale.SaleListNumber == saleDoc.SaleListNumber);
//                    if (soldProduct != null)
//                    {
//                        if (!ListOfSoldProducts.Contains(soldProduct))
//                        {ListOfSoldProducts.Add(soldProduct);}
//                        else
//                        {
//                            var addAmountToSoldProduct = ListOfSoldProducts.FirstOrDefault(sp => sp.ID == soldProduct.ID);
//                            if (addAmountToSoldProduct != null) addAmountToSoldProduct.Amount += soldProduct.Amount;
//                            else MessageBox.Show("Что-то пошло не так, не нашел продукт");
//                        }
//                    }
//                }

                GetProducts();

                InitCommands();
            }
        }

        #region Fields
        
        private ColorITEntities _colorItEntities;

        #endregion //Fields

        #region Properties

        private DateTime _startDate;
        public DateTime StartDate
        {
            get { return _startDate; }
            set
            {
                _startDate = value;
                RaisePropertyChanged(() => StartDate);
            }
        }

        private DateTime _endDate;
        public DateTime EndDate
        {
            get { return _endDate; }
            set
            {
                _endDate = value;
                RaisePropertyChanged(() => EndDate);
            }
        }

        private ObservableCollection<Sale> _listOfSoldProducts;
        public ObservableCollection<Sale> ListOfSoldProducts
        {
            get { return _listOfSoldProducts; }
            set
            {
                _listOfSoldProducts = value;
                RaisePropertyChanged(() => ListOfSoldProducts);
            }
        }

        #endregion //Properties

        #region Methods

        private void GetProducts()
        {
            var listOfSaleDocs = _colorItEntities.SaleDocument.Where
            (saleDoc => EntityFunctions.TruncateTime(saleDoc.DateCreated) >= StartDate 
                && EntityFunctions.TruncateTime(saleDoc.DateCreated) <= EndDate).ToList();

            ListOfSoldProducts.Clear();
            foreach (SaleDocument saleDoc in listOfSaleDocs)
            {
                var soldProduct =
                    _colorItEntities.Sale.FirstOrDefault(sale => sale.SaleListNumber == saleDoc.SaleListNumber);
                if (soldProduct != null)
                {
                    if (!ListOfSoldProducts.Contains(soldProduct))
                    { ListOfSoldProducts.Add(soldProduct); }
                    else
                    {
                        var addAmountToSoldProduct = ListOfSoldProducts.FirstOrDefault(sp => sp.ID == soldProduct.ID);
                        if (addAmountToSoldProduct != null) addAmountToSoldProduct.Amount += soldProduct.Amount;
                        else MessageBox.Show("Что-то пошло не так, не нашел продукт");
                    }
                }
            }
        }

        #endregion //Methods

        #region Commands

        public RelayCommand RefreshGrid { get; private set; }

        private void InitCommands()
        {
            RefreshGrid = new RelayCommand(GetProducts);
        }

        #endregion //Commands
    }
}
