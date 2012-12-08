using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ColoritWPF
{
    class PaintMath
    {
        #region Fields

        private decimal _sum;
        private decimal _amount;
        private decimal _cost;
        private decimal _polishAmount;
        private decimal _prepay;
        private decimal _goodsSum;

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

        public decimal Cost
        {
            get { return _cost; }
            set { _cost = value; 
                ValueChangedEvent(); 
            }
        }

        public decimal PolishAmount
        {
            get { return _polishAmount; }
            set { _polishAmount = value;
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

        #endregion

        public delegate void ValueChanged();
        public event ValueChanged ValueChangedEvent;

        public PaintMath(decimal amount, decimal cost, decimal polishAmount, decimal prepay)
        {
            _amount = amount;
            _cost = cost;
            _polishAmount = polishAmount;
            _prepay = prepay;

            ValueChanged handler = ValueChangedHandlerMethod;
            ValueChangedEvent += handler;
        }

        private void ValueChangedHandlerMethod()
        {
            
        }

        //Вычисляет сумму за товар
        private void CountGoodsSum()
        {
            
        }
    }
}
