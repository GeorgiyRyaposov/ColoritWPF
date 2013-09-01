using System;
using System.Linq;
using System.Windows.Media;
    
namespace ColoritWPF
{
    partial class PurchaseDocument
    {
        #region Properties

        public string DisplayDocumentNumber
        {
            get { return DocumentNumber.ToString("00000"); }
        }

        partial void OnPrepayChanged()
        {
            UpdateBrush();
        }

        partial void OnConfirmedChanged()
        {
            UpdateBrush();
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
                var previousPurchDoc = (from n in colorItEntities.PurchaseDocument
                                       orderby n.Id descending
                                       select n).FirstOrDefault();

                if (previousPurchDoc == null)
                    previousPurchDoc = new PurchaseDocument();
                
                DocumentNumber = 0;
                

                int num = previousPurchDoc.DocumentNumber;

                if (previousPurchDoc.Date.Month != DateTime.Now.Month)
                {
                    num = 0;
                }

                num++;

                DocumentNumber = num;
            }
        }

        #endregion

        
    }
}
