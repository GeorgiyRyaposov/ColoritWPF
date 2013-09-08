using System;
using System.Linq;
    
namespace ColoritWPF
{
    partial class PurchaseDocument
    {
        #region Properties

        public string DisplayDocumentNumber
        {
            get { return DocumentNumber.ToString("00000"); }
        }

        #endregion

        #region Methods

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
