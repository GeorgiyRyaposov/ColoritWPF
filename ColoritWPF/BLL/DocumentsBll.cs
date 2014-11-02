using System;
using System.Collections.Generic;
using ColoritWPF.Common;
using System.Linq;

namespace ColoritWPF.BLL
{
    public class DocumentsBll
    {
        public void SaveSaleDocument(SaleDocument docToSave)
        {
            using (var dataContext = new ColorITEntities())
            {
                try
                {
                    dataContext.SaleDocument.AddObject(docToSave);
                    dataContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    ErrorHandler.ShowError("Не удалось сохранить документ", ex);
                }
            }
        }

        /// <summary>
        /// Возвращает документы о продажах за последние 10 дней
        /// </summary>
        /// <returns></returns>
        public List<SaleDocument> GetSaleDocument()
        {
            using (var dataContext = new ColorITEntities())
            {
                return dataContext.SaleDocument.Where(x =>
                               x.DateCreated.Year == DateTime.Now.Year
                            && x.DateCreated.Month == DateTime.Now.Month
                            && x.DateCreated.Day >= (DateTime.Now.Day - 10)).ToList();
             
            }
        }

        public void UpdateSaleProduct(Sale updatedProduct, bool isDocConfirmed)
        {
            using (var dataContext = new ColorITEntities())
            {
                var saleProductToUpdate = dataContext.Sale.First(item => item.ID == updatedProduct.ID);
                if (saleProductToUpdate == null)
                {
                    ErrorHandler.ShowError("Не удалось найти продукт из списка продаж");
                    return;
                }

                try
                {
                    saleProductToUpdate.Amount = updatedProduct.Amount;
                    saleProductToUpdate.Discount = updatedProduct.CurrentDiscount;
                    saleProductToUpdate.Cost = updatedProduct.Cost;

                    //Вычитаем товар со склада т.к. документ проводится
                    if (isDocConfirmed)
                    {
                        var productStorage = dataContext.Product.First(pr => pr.ID == saleProductToUpdate.Product.ID);
                        if (productStorage.Storage < saleProductToUpdate.Amount)
                        {
                            ErrorHandler.ShowError("В магазине не достаточно товара!");
                        }
                        productStorage.Storage = productStorage.Storage - saleProductToUpdate.Amount;
                    }

                    dataContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    ErrorHandler.ShowError("Не удалось сохранить изменения для продукта из списка", ex);
                }
            }
        }

        public void UpdateSaleProducts(IEnumerable<Sale> updatedProducts, bool isDocConfirmed)
        {
            using (var dataContext = new ColorITEntities())
            {
                foreach (var updatedProduct in updatedProducts)
                {
                    var saleProductToUpdate = dataContext.Sale.First(item => item.ID == updatedProduct.ID);
                    if (saleProductToUpdate == null)
                    {
                        ErrorHandler.ShowError("Не удалось найти продукт из списка продаж");
                        return;
                    }

                    try
                    {
                        saleProductToUpdate.Amount = updatedProduct.Amount;
                        saleProductToUpdate.Discount = updatedProduct.CurrentDiscount;
                        saleProductToUpdate.Cost = updatedProduct.Cost;

                        //Вычитаем товар со склада т.к. документ проводится
                        if (isDocConfirmed)
                        {
                            var productStorage = dataContext.Product.First(pr => pr.ID == saleProductToUpdate.Product.ID);
                            if (productStorage.Storage < saleProductToUpdate.Amount)
                            {
                                ErrorHandler.ShowError("В магазине не достаточно товара!");
                            }
                            productStorage.Storage = productStorage.Storage - saleProductToUpdate.Amount;
                        }

                        dataContext.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        ErrorHandler.ShowError("Не удалось сохранить изменения для продукта из списка", ex);
                    }
                }
            }
        }

        public void RemoveItemFromSaleDocument(long id)
        {
            using (var dataContext = new ColorITEntities())
            {
                var itemToRemove =  dataContext.Sale.First(item => item.ID == id);
                if (itemToRemove == null)
                {
                    //ErrorHandler.ShowError("Не удалось найти товар из списка");
                    //Не нужно ошибки, если список не был сохранен в базе, то и товара в нем еще нет
                    return;
                }
                try
                {
                    dataContext.Sale.DeleteObject(itemToRemove);
                    dataContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    ErrorHandler.ShowError("Не удалось удалить товар из списка", ex);
                }
            }
        }

        public void RemoveSaleDocument(long id)
        {
            using (var dataContext = new ColorITEntities())
            {
                var listToRemove = dataContext.SaleDocument.First(item => item.Id == id);

                var saleItemsToRemove = dataContext.Sale.Where(item => item.SaleListNumber == listToRemove.SaleListNumber).ToList();
                try
                {
                    //Удаляем все продукты принадлежащие списку
                    foreach (var item in saleItemsToRemove)
                    {
                        dataContext.Sale.DeleteObject(item);
                    }
                    //Удаляем список
                    dataContext.SaleDocument.DeleteObject(listToRemove);
                    dataContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    ErrorHandler.ShowError("Не удалось удалить список товаров ", ex);
                }
            }
        }

        public void UpdateSaleDocument(SaleDocument saleDocument)
        {
            using (var dataContext = new ColorITEntities())
            {
                var docToUpdate = dataContext.SaleDocument.First(doc => doc.Id == saleDocument.Id);
                if (docToUpdate == null)
                {
                    ErrorHandler.ShowError("Не удалось найти документ");
                    return;
                }
                
                try
                {
                    docToUpdate.ClientId = saleDocument.ClientId;
                    docToUpdate.Prepay = saleDocument.Prepay;
                    docToUpdate.Confirmed = saleDocument.Confirmed;
                    docToUpdate.ClientBalancePartInTotal = saleDocument.ClientBalancePartInTotal;

                    dataContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    ErrorHandler.ShowError("Не удалось сохранить изменения в документе", ex);
                }
            }
        }

        public SaleDocument AddSaleDocument(int clientId)
        {
            using (var datacontext = new ColorITEntities())
            {
                var newSaleDocument = new SaleDocument
                {
                    DateCreated = DateTime.Now,
                    Confirmed = false,
                    Prepay = false,
                    ClientId = clientId
                };

                try
                {
                    datacontext.SaleDocument.AddObject(newSaleDocument);
                    datacontext.SaveChanges();
                    return newSaleDocument;
                }
                catch (Exception exception)
                {
                    ErrorHandler.ShowError("Не удалось добавить документ в базу", exception);
                    return null;
                }
            }
        }

        public void AddProductsToSaleDocument(long documentId, IEnumerable<Product> selectedProducts)
        {
            using (var dataContext = new ColorITEntities())
            {
                foreach (var product in selectedProducts)
                {
                    var saleProduct = new Sale
                    {
                        ProductID = product.ID,
                        Amount = product.Amount,
                        Cost = product.Cost,
                        SaleListNumber = documentId
                    };

                    dataContext.Sale.AddObject(saleProduct);
                }

                try
                {
                    dataContext.SaveChanges();
                }
                catch (Exception exception)
                {
                    ErrorHandler.ShowError("Не удалось сохранить список продуктов в базу", exception);
                }
            }
        }
    }
}
