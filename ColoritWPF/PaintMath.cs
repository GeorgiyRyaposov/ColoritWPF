using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace ColoritWPF
{
    public class PaintMath : INotifyPropertyChanged
    {
        #region INotifiedProperty Block
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        #region Fields

        private decimal _sum;
        private decimal _amount;
        private decimal _polishSum;
        private decimal _prepay;
        private decimal _goodsSum;
        private decimal _discount;
        private decimal _total;
        private decimal _clientBalance;
        private PaintName _paintName;        

        #endregion

        #region Properties
        
        public decimal Sum
        {
            get { return _sum; }
            set 
            { 
                _sum = value;
                OnPropertyChanged("Sum");
            }
        }

        public decimal Amount
        {
            get
            {
                return _amount;
            }
            set 
            { 
                _amount = value;
                OnPropertyChanged("Amount");
                CountGoodsSum();
            }
        }

        public decimal PolishSum
        {
            get { return _polishSum; }
            set 
            { 
                _polishSum = value;
                OnPropertyChanged("PolishSum");
                CountGoodsSum();
            }
        }

        public decimal Prepay
        {
            get { return _prepay; }
            set 
            { 
                _prepay = value;
                OnPropertyChanged("Prepay");
                CountGoodsSum();
            }
        }

        public decimal GoodsSum
        {
            get { return _goodsSum; }
            set 
            { 
                _goodsSum = value;
                OnPropertyChanged("GoodsSum");
            }
        }

        public PaintName SelectedPaint
        {
            get { return _paintName; }
            set 
            { 
                _paintName = value;
                OnPropertyChanged("SelectedPaint");
                CountGoodsSum();
            }
        }

        public decimal Discount
        {
            get { return _discount; }
            set 
            { 
                _discount = value;
                OnPropertyChanged("Discount");
                CountGoodsSum();
            }
        }

        public decimal ClientBalance
        {
            get { return _clientBalance; }
            set 
            { 
                _clientBalance = value;
                OnPropertyChanged("ClientBalance");
                CountGoodsSum();
            }
        }

        public decimal Total
        {
            get { return _total; }
            set
            {
                _total = value;
                OnPropertyChanged("Total");
            }
        }

        #endregion


        public PaintMath()
        {
        }

        //Вычисляет сумму за товар
        public void CountGoodsSum()
        {
            decimal census = 0;
            decimal work = 0;
            decimal container = 0;
            decimal cost = SelectedPaint.Cost;
            if(SelectedPaint != null)
                census = GetCensus();
            if (SelectedPaint.Work != null)
            {
                work = (decimal)SelectedPaint.Work;
            }
            if (SelectedPaint.Container != null)
            {
                container = (decimal)SelectedPaint.Container;
            }

            GoodsSum = ((cost * (Amount + census) + PolishSum) * Discount) + work + container;
            
            Total = GoodsSum + Prepay + ClientBalance;
        }

        //Добыть перепыл
        private decimal GetCensus()
        {
            float f = 0.25f;
            if (Amount < (decimal)f)
            {
                if (SelectedPaint.Census1 != null)
                    return (decimal)SelectedPaint.Census1;
            }
            else
            {
                if (SelectedPaint.Census2 != null)
                    return (decimal)SelectedPaint.Census2;
            }
            return 0;
        }
    }
}
