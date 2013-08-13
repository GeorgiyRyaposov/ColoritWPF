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

        private string _sender;
        public string Sender
        {
            get
            {
                if (_sender == null)
                {
                    _sender = "Не выбрано";
                }
                return _sender;
            }
            set
            {
                _sender = value;
                OnPropertyChanged("Sender");
            }
        }

        private string _receiver;
        public string Receiver
        {
            get
            {
                if (_receiver == null)
                {
                    _receiver = "Не выбрано";
                }
                
                return _receiver;
            }
            set
            {
                _receiver = value;
                OnPropertyChanged("Receiver");
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

        partial void OnToStorageChanged()
        {
            if (ToStorage)
                Sender = "Склад";
            if (!ToStorage)
                Sender = "Магазин";
        }

        partial void OnToWarehouseChanged()
        {
            if (ToWarehouse)
                Receiver = "Магазин";
            if (!ToWarehouse)
                Receiver = "Склад";
        }

        #endregion
    }
}
