using System;
using System.ComponentModel;
using System.Windows.Controls;

namespace ColoritWPF
{
    public partial class Product : IDataErrorInfo
    {
        #region Properties

        public string Article
        {
            get { return ID.ToString("00000"); }
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

        private int _amount;
        public int Amount
        {
            get { return _amount; }
            set
            {
                _amount = value;
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

        public string Groups
        {
            get { return Group1.Name; }
        }

        public string ProducerGr
        {
            get { return Producers.Name; }
        }

    #endregion

        #region Methods

        public void ReCalc()
        {
            Total = Amount * Cost - Amount * Cost* (decimal)CurrentDiscount;
            CleanTotal = Amount*Cost;
        }

        partial void OnCostChanged()
        {
            ReCalc();
        }

        #endregion

        #region IDataErrorInfo Members

        private int _errors = 0;
        public void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added)
                _errors++;
            else
                _errors--;
        }

        public string Error
        {
            get { throw new NotImplementedException(); }
        }

        public string this[string columnName]
        {
            get
            {
                string result = null;
                switch (columnName)
                {
                    case "Amount":
                        if (Amount > (Warehouse+Storage))
                            result = "Недостаточно товара";
                        break;
                    case "Name":
                        if (String.IsNullOrEmpty(Name))
                            result = "Введите наименование продукта";
                        break;
                    case "Cost":
                        if (Cost<=0)
                            result = "Цена должна быть больше нуля";
                        break;
                    case "SelfCost":
                        if (SelfCost<= 0)
                            result = "Себестоимость должна быть больше нуля";
                        break;
                    case "MaxDiscount":
                        if (MaxDiscount < 0)
                            result = "Скидка должна быть положительной или равно нулю";
                        break;
                }
                return result;
            }
        }

        #endregion
    }
}
