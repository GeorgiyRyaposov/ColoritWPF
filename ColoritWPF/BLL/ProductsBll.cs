using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using ColoritWPF.Common;

namespace ColoritWPF.BLL
{
    public class ProductsBll
    {
        public List<Product> GetProducts()
        {
            using (var dataContext = new ColorITEntities())
            {
                return dataContext.Product.ToList();
            }
        }

        /// <summary>
        /// Проверяет наличие каждого товара из списка
        /// </summary>
        /// <returns>Возвращает false если товара в магазине не достаточно</returns>
        public bool IsAllProductInStock(IEnumerable<Sale> saleProductsList)
        {
            var notEnough = new List<string>();
            foreach (var saleProduct in saleProductsList)
            {
                if (saleProduct.Product.Storage < saleProduct.Amount)
                    notEnough.Add(saleProduct.Name);
            }

            if (notEnough.Count <= 0) 
                return true;

            var msgText = String.Format("Недостаточно товара в магазине:\n");
            msgText = notEnough.Aggregate(msgText, (current, name) => current + name + "\n");

            MessageBox.Show(msgText, "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        public void ReturnProductToStorage(IEnumerable<Sale> saleProductsList)
        {
            using (var dataContext = new ColorITEntities())
            {
                foreach (var saleProduct in saleProductsList)
                {
                    var productStorage = dataContext.Product.First(pr => pr.ID == saleProduct.Product.ID);
                    if (productStorage == null)
                    {
                        ErrorHandler.ShowError("Не удалось найти продукт в базе");
                        return;
                    }
                    
                    try
                    {
                        productStorage.Storage = productStorage.Storage + saleProduct.Amount;
                        dataContext.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        ErrorHandler.ShowError("Не удалось сохранить изменения", ex);
                    }
                }
            }
        }
    }
}
