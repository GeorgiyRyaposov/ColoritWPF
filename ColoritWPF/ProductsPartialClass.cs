namespace ColoritWPF
{
    public partial class Product
    {
        #region Properties
        
        private decimal _total;
        public decimal Total
        {
            get { return _total; }
            set { _total = value;
            OnPropertyChanged("Total");
            }
        }

        private decimal _cleanTotal;
        public decimal CleanTotal
        {
            get { return _cleanTotal; }
            set
            {
                _cleanTotal = value;
                OnPropertyChanged("CleanTotal");
            }
        }

        private int _amount;
        public int Amount
        {
            get { return _amount; }
            set { _amount = value;
                OnPropertyChanged("Amount");
                ReCalc();
            }
        }

        private double _currentDiscount;
        public double CurrentDiscount
        {
            get { return _currentDiscount; }
            set 
            {
                _currentDiscount = value > MaxDiscount ? MaxDiscount : value;
                
                OnPropertyChanged("CurrentDiscount");
                ReCalc();
            }
        }

        #endregion

        #region Methods

        public void ReCalc()
        {
            Total = Amount * Cost - Amount * Cost* (decimal)CurrentDiscount;
            CleanTotal = Amount*Cost;
        }

        #endregion
    }
}
