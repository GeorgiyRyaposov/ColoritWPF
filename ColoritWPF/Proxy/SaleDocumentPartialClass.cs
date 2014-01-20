using System;
using System.Linq;

namespace ColoritWPF
{
    public partial class SaleDocument
    {
        #region Properties
        
        public string DocumentNumber
        {
            get { return SaleListNumber.ToString("00000"); }
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

        #endregion

        #region Methods

        public void GenerateDocNumber()
        {
            using (ColorITEntities colorItEntities = new ColorITEntities())
            {
                var previousSaleDoc = (from n in colorItEntities.SaleDocument
                                       orderby n.Id descending
                                       select n).FirstOrDefault();

                if (previousSaleDoc == null)
                    previousSaleDoc = new SaleDocument();
                
                SaleListNumber = 0;
                

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
