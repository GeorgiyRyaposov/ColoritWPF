using System;
using System.Linq;
using System.Windows.Media;

namespace ColoritWPF
{
    public partial class Paints
    {
        public decimal PolishSum
        {
            get { return AddToSumPackagePolish(); }
        }

        private Brush _rowColor;
        public Brush RowColor
        {
            get
            {
                _rowColor = Brushes.White;
                if (DocState)
                    _rowColor = Brushes.LightGreen;
                if (!DocState && (Prepay == 0))
                    _rowColor = Brushes.LightPink;
                if (!DocState && (Prepay > 0))
                    _rowColor = Brushes.LightGoldenrodYellow;
                return _rowColor;
            }
            set
            {
                _rowColor = value;
                OnPropertyChanged("RowColor");
            }
        }

        partial void OnDocStateChanged()
        {
            UpdateRowColor();
        }

        partial void OnPrepayChanged()
        {
            UpdateRowColor();
        }

        private void UpdateRowColor()
        {
            if (DocState)
                RowColor = Brushes.LightGreen;
            if (!DocState && (Prepay == 0))
                RowColor = Brushes.LightPink;
            if (!DocState && (Prepay > 0))
                RowColor = Brushes.LightGoldenrodYellow;
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
        
        public void ReCalcAll(decimal work, decimal discount)
        {
            double census = GetCensus();
            Sum = ((PaintValues.Cost * (Decimal.Parse(Amount.ToString()) + Decimal.Parse(census.ToString())) + PolishSum) * (1 - discount)) + work + PaintValues.Container;
            Total = Sum + ClientValues.Balance - Prepay;
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
