using System;
using System.Linq;
using System.Windows.Media;

namespace ColoritWPF
{
    public partial class SaleDocument
    {
        #region Properties
        
        public string DocumentNumber
        {
            get { return SaleListNumber.ToString("00000"); }
        }

        partial void OnPrepayChanged()
        {
            UpdateBrush();
        }

        partial void OnConfirmedChanged()
        {
            UpdateBrush();
        }

        private double _discount;
        public double Discount
        {
            get { return _discount; }
            set
            {
                _discount = value;
                OnPropertyChanged("Discount");
            }
        }

        private Brush _rowColor = Brushes.White;
        public Brush StorageRowColor
        {
            get
            {
                if (Confirmed)
                    _rowColor = Brushes.LightGreen;
                if (Prepay)
                    _rowColor = Brushes.LightGoldenrodYellow;
                if (!Confirmed && !Prepay)
                    _rowColor = Brushes.LightPink;
                return _rowColor;
            }
            set
            {
                _rowColor = value;
                OnPropertyChanged("StorageRowColor");
            }
        }

        #endregion

        #region Methods

        private void UpdateBrush()
        {
            if (Confirmed)
                _rowColor = Brushes.LightGreen;
            if (Prepay)
                _rowColor = Brushes.LightGoldenrodYellow;
            if (!Confirmed && !Prepay)
                _rowColor = Brushes.LightPink;
            OnPropertyChanged("StorageRowColor");
        }

        public void GenerateDocNumber()
        {
            using (ColorITEntities colorItEntities = new ColorITEntities())
            {
                var previousSaleDoc = (from n in colorItEntities.SaleDocument
                                       orderby n.Id descending
                                       select n).FirstOrDefault();

                if (previousSaleDoc == null)
                    previousSaleDoc = new SaleDocument();
                {
                    SaleListNumber = 0;
                }

                int num = previousSaleDoc.SaleListNumber;

                if (previousSaleDoc.DateCreated.Month != DateTime.Now.Month)
                {
                    num = 0;
                }

                num++;

                SaleListNumber = num;
            }
        }

        #endregion
    }
}
