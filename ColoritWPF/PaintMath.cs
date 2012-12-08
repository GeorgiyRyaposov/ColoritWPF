using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace ColoritWPF
{
    public class PaintMath
    {
        #region Fields

        private decimal _sum;
        private decimal _amount;
        private decimal _polishSum;
        private decimal _prepay;
        private decimal _goodsSum;
        private decimal _discount;
        private decimal _clientBalance;
        private PaintName _paintName;        

        private TextBox _goodsTotal;
        private TextBox _total;

        #endregion

        #region Properties
        
        public decimal Sum
        {
            get { return _sum; }
            set { _sum = value;
                ValueChangedEvent();
            }
        }

        public decimal Amount
        {
            get { return _amount; }
            set { _amount = value;
            ValueChangedEvent();
            }
        }

        public decimal PolishSum
        {
            get { return _polishSum; }
            set { _polishSum = value;
            ValueChangedEvent();
            }
        }

        public decimal Prepay
        {
            get { return _prepay; }
            set { _prepay = value;
            ValueChangedEvent();
            }
        }

        public decimal GoodsSum
        {
            get { return _goodsSum; }
            set { _goodsSum = value;
            ValueChangedEvent();
            }
        }

        public PaintName SelectedPaint
        {
            get { return _paintName; }
            set { _paintName = value;
                ValueChangedEvent();
            }
        }

        public decimal Discount
        {
            get { return _discount; }
            set { _discount = value;
            ValueChangedEvent();
            }
        }

        public decimal ClientBalance
        {
            get { return _clientBalance; }
            set { _clientBalance = value;
            ValueChangedEvent();
            }
        }

        #endregion

        public delegate void ValueChanged();
        public event ValueChanged ValueChangedEvent;

        public PaintMath(TextBox GoodsTotal, TextBox Total)
        {
            _goodsTotal = GoodsTotal;
            _total = Total;

            ValueChanged handler = ValueChangedHandlerMethod;
            ValueChangedEvent += handler;
        }

        private void ValueChangedHandlerMethod()
        {
            CountGoodsSum();
        }

        //Вычисляет сумму за товар
        private void CountGoodsSum()
        {
            decimal census = GetCensus();
            decimal work = 0;
            decimal container = 0;
            decimal cost = SelectedPaint.Cost;
            decimal _totalValue = 0;

            if (SelectedPaint.Work != null)
            {
                work = (decimal)SelectedPaint.Work;
            }
            if (SelectedPaint.Container != null)
            {
                container = (decimal)SelectedPaint.Container;
            }

            GoodsSum = ((cost * (Amount + census) + PolishSum) * Discount) + work + container;
            _goodsTotal.Text = GoodsSum.ToString(CultureInfo.InvariantCulture);
            
            _totalValue = GoodsSum + Prepay + ClientBalance;
            _total.Text = _totalValue.ToString(CultureInfo.InvariantCulture);
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
