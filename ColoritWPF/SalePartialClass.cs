namespace ColoritWPF
{
    public partial class Sale
    {
        #region Properties

        public string Article
        {
            get { return ProductID.ToString("00000"); }
        }

        public string Name
        {
            get { return Product.Name; }
        }

        private decimal _total;
        public decimal Total
        {
            get { return _total; }
            set
            {
                _total = value;
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

        private double _currentDiscount;
        public double CurrentDiscount
        {
            get { return _currentDiscount; }
            set
            {
                _currentDiscount = value > Product.MaxDiscount ? Product.MaxDiscount : value;
                OnPropertyChanged("CurrentDiscount");
                ReCalc();
            }
        }

        #endregion //Properties

        #region Methods

        public void ReCalc()
        {
            Total = Amount * Cost - Amount * Cost * (decimal)CurrentDiscount;
            CleanTotal = Amount * Cost;
        }

        partial void OnCostChanged()
        {
            ReCalc();
        }

        partial void OnAmountChanged()
        {
            ReCalc();
        }

        #endregion //Methods
    }
}
