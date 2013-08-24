namespace ColoritWPF
{
    partial class Purchase
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
        
        #endregion //Properties

        #region Methods

        public void ReCalc()
        {
            Total = Amount * Cost;
            CleanTotal = Amount * SelfCost;
        }

        partial void OnCostChanged()
        {
            ReCalc();
        }

        partial void OnSelfCostChanged()
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
