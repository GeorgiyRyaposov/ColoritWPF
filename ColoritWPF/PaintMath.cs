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

        private decimal _amount =0;
        private decimal _polishSum =0;
        private decimal _prepay =0;
        private decimal _goodsSum=0;
        private decimal _total=0;
        private PaintName _paintName;
        private Client _selectedClient;

        #endregion

        #region Properties
       
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
                CalcTotal();
            }
        }

        public decimal GoodsSum
        {
            get { return _goodsSum; }
            set 
            { 
                _goodsSum = value;
                OnPropertyChanged("GoodsSum");
                CalcTotal();
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

        public decimal Total
        {
            get { return _total; }
            set
            {
                _total = value;
                OnPropertyChanged("Total");
            }
        }

        public Client SelectedClient
        {
            get { return _selectedClient; }
            set { _selectedClient = value;
            OnPropertyChanged("SelectedClient");
            }
        }

        #endregion


        public PaintMath()
        {
            _paintName = new PaintName();
            _selectedClient = new Client();
        }

        //Вычисляет сумму за товар
        public void CountGoodsSum()
        {
            decimal census = 0;
            decimal work = 0;
            decimal container = 0;
            decimal discount = 1;
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

            if (SelectedClient.Discount != null)
                discount = (Decimal) SelectedClient.Discount;
                GoodsSum = ((cost * (Amount + census) + PolishSum) * discount) + work + container;

        }

        public void CalcTotal()
        {
            Total = GoodsSum + Prepay + SelectedClient.Balance;
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
