using System;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace ColoritWPF.ViewModel.Products
{
    public class AddNewProductViewModel : ViewModelBase
    {
        private ColorITEntities colorItEntities;

        public AddNewProductViewModel()
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

        public ObservableCollection<Producers> ProducersCollection { get; set; }
        public ObservableCollection<Group> GroupCollection { get; set; }

        private Product _newProduct;
        public Product NewProduct
        {
            get { return _newProduct; }
            set
            {
                _newProduct = value;
                base.RaisePropertyChanged("NewProduct");
            }
        }

        //private Group _selectedGroup;
        //public Group SelectedGroup
        //{
        //    get { return _selectedGroup; }
        //    set
        //    {
        //        _selectedGroup = value;
        //        base.RaisePropertyChanged("SelectedGroup");
        //    }
        //}

        //private Producers _selectedProducer;
        //public Producers SelectedProducer
        //{
        //    get { return _selectedProducer; }
        //    set
        //    {
        //        _selectedProducer = value;
        //        base.RaisePropertyChanged("SelectedProducer");
        //    }
        //}
        #endregion

        #region Methods

        private void GetData()
        {
            ProducersCollection = new ObservableCollection<Producers>(colorItEntities.Producers.ToList());
            GroupCollection = new ObservableCollection<Group>(colorItEntities.Group.ToList());
            NewProduct = new Product
                {
                    Amount = 0,
                    Warehouse = 0,
                    Storage = 0
                };
        }

        #endregion

        #region Commands

        public RelayCommand AddProductCommand
        {
            get;
            private set;
        }

        private void InitializeCommands()
        {
            AddProductCommand = new RelayCommand(AddProductCmd, AddProductCommandCanExecute);
        }

        private bool AddProductCommandCanExecute()
        {
            if (String.IsNullOrEmpty(NewProduct.Name))
                return false;
            return true;
        }

        private void AddProductCmd()
        {
            if (NewProduct == null)
                return;

            //NewProduct.Group = SelectedGroup.ID;
            //NewProduct.ProducerId = SelectedProducer.Id;

            try
            {
                colorItEntities.Product.AddObject(NewProduct);
                colorItEntities.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось добавить продукт:\n"+ex.Message+"\n"+ex.InnerException);
            }
        }

        #endregion
    }
}
