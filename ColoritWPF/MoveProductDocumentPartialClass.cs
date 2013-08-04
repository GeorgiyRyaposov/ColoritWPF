using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace ColoritWPF
{
    public partial class MoveProductDocument
    {
        #region Properties

        //public ObservableCollection<Product> TransferProductsList { get; set; }

        private ObservableCollection<MoveProduct> _moveProducts;
        public ObservableCollection<MoveProduct> MoveProductsList
        {
            get
            {
                if (_moveProducts == null)
                    GetProducts();
                return _moveProducts;
            }
            set { _moveProducts = value; }
        }

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
                return "Магаин";
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

                this.DocumentNumber = num;
            }
        }

        private void GetProducts()
        {
            using (ColorITEntities colorItEntities = new ColorITEntities())
            {
                var productsList = (from product in colorItEntities.MoveProduct
                                    where product.DocNumber == this.Id
                                    select product).ToList();

                _moveProducts = new ObservableCollection<MoveProduct>();
                foreach (MoveProduct moveProduct in productsList)
                {
                    _moveProducts.Add(moveProduct);
                }
            }
        }

        #endregion
    }
}
