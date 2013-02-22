using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ColoritWPF
{
    public partial class Paints
    {
        private decimal _goodsSum;
        public decimal GoodsSum
        {
            get
            {
                return _goodsSum;
            }
            set
            {
                _goodsSum = value;
                OnPropertyChanged("GoodsSum");
            }
        }

        public decimal PolishSum
        {
            get { return AddToSumPackagePolish(); }
        }

        public Client ClientValues
        {
            get
            {
                using (ColorITEntities colorItEntities = new ColorITEntities())
                {
                    var client = (from clients in colorItEntities.Client
                                      where clients.ID == ClientID
                                      select clients).First();
                    return client;
                }
            }
        }

        public PaintName PaintValues
        {
            get
            {
                using (ColorITEntities colorItEntities = new ColorITEntities())
                {
                    var paint = (from paints in colorItEntities.PaintName
                                 where paints.ID == NameID
                                 select paints).First();
                    return paint;
                }
            }
        }

        private decimal _totalValue;
        public decimal TotalValue
        {
            get { return (GoodsSum + Prepay + ClientValues.Balance); }
            set { _totalValue = value; }
        }

        public void ReCalcAll(decimal work)
        {
            double census = GetCensus();
            _goodsSum = ((PaintValues.Cost * (Decimal.Parse(Amount.ToString()) + Decimal.Parse(census.ToString())) + PolishSum) * (1 - (decimal)ClientValues.Discount)) + work + PaintValues.Container;
            OnPropertyChanged("GoodsSum");

            _totalValue = GoodsSum + Prepay + ClientValues.Balance;
            OnPropertyChanged("TotalValue");
        }

        //Добыть перепыл
        private double GetCensus()
        {
            //float f = 0.25f;
            if (Amount < 0.25)
            {
                return PaintValues.Census1;
            }
            return PaintValues.Census2;
        }

        //Достает сумму лак комплект (цена литр * кол-во + тара)
        public decimal AddToSumPackagePolish()
        {
            if (AmountPolish == 0)
                return 0;
            PaintName polish;
            using (ColorITEntities colorItEntities = new ColorITEntities())
            {
                var getPolish = (from _paint in colorItEntities.PaintName
                                 where _paint.ID == 7
                                 select _paint).FirstOrDefault();
                polish = getPolish;
            }
            return (polish.Cost * (decimal)AmountPolish + polish.Container);
        }
    }
}
