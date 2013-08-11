using System;
using System.Linq;
using System.Windows.Media;

namespace ColoritWPF
{
    public partial class MoveProductDocument
    {
        #region Properties

        public string DisplayDocumentNumber
        {
            get { return DocumentNumber.ToString("00000"); }
        }

        public string ShortDate
        {
            get { return Date.ToShortDateString(); }
        }

        public string ShortTime
        {
            get { return Date.ToShortTimeString(); }
        }

        public string Sender
        {
            get
            {
                if (ToStorage)
                    return "Склад";
                return "Магазин";
            }
        }
        
        public string Receiver
        {
            get
            {
                if (ToWarehouse)
                    return "Магазин";
                return "Склад";
            }
        }

        private Brush _rowColor = Brushes.White;
        public Brush StorageRowColor
        {
            get
            {
                if (Confirmed)
                    _rowColor = Brushes.LightGreen;
                if (!Confirmed)
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

        public void GenerateDocNumber()
        {
            using (ColorITEntities colorItEntities = new ColorITEntities())
            {
                var previousSaleDoc = (from n in colorItEntities.MoveProductDocument
                                       orderby n.Id descending
                                       select n).FirstOrDefault();

                if (previousSaleDoc == null)
                    previousSaleDoc = new MoveProductDocument { DocumentNumber = 0 };

                int num = previousSaleDoc.DocumentNumber;

                if (previousSaleDoc.Date.Month != DateTime.Now.Month)
                {
                    num = 0;
                }

                num++;

                DocumentNumber = num;
            }
        }

        partial void OnConfirmedChanged()
        {
            if (Confirmed)
                _rowColor = Brushes.LightGreen;
            if (!Confirmed)
                _rowColor = Brushes.LightPink;
            OnPropertyChanged("StorageRowColor");
        }
        
        #endregion
    }
}
