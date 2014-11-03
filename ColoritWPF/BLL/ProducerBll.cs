using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColoritWPF.Common;

namespace ColoritWPF.BLL
{
    public class ProducerBll
    {
        public List<Producers> GetProducers()
        {
            using (var dataContext = new ColorITEntities())
            {
                return dataContext.Producers.ToList();
            }
        }

        public void AddProducer(string newProducerName)
        {
            using (var dataContext = new ColorITEntities())
            {
                try
                {
                    dataContext.Producers.AddObject(new Producers { Name = newProducerName });
                    dataContext.SaveChanges();
                }
                catch (Exception exception)
                {
                    ErrorHandler.ShowError("Не удалось сохранить изменения", exception);
                }
            }
        }

        public void UpdateProducer(Producers updateProducer)
        {
            using (var dataContext = new ColorITEntities())
            {
                var producerToUpdate = dataContext.Producers.First(prod => prod.Id == updateProducer.Id);
                if (producerToUpdate == null)
                {
                    ErrorHandler.ShowError("Не удалось найти производителя в базе");
                    return;
                }

                try
                {
                    producerToUpdate.Name = updateProducer.Name;
                    dataContext.SaveChanges();
                }
                catch (Exception exception)
                {
                    ErrorHandler.ShowError("Не удалось сохранить изменения", exception);
                }
            }
        }

        public void RemoveProducer(int producerId)
        {
            using (var dataContext = new ColorITEntities())
            {
                var producerToRemove = dataContext.Producers.First(prod => prod.Id == producerId);
                if (producerToRemove == null)
                {
                    ErrorHandler.ShowError("Не удалось найти производителя в базе");
                    return;
                }

                var productsWithProducer = dataContext.Product.Where(product => product.ProducerId == producerId).ToList();
                if (productsWithProducer.Count > 0)
                {
                    
                    var msgBuilder = new StringBuilder("Нельзя удалить производителя, он используется в продуктах:\n");
                    foreach (var product in productsWithProducer)
                    {
                        msgBuilder.Append(product.Name + "\n");
                    }

                    ErrorHandler.ShowError(msgBuilder.ToString());
                    return;
                }
                try
                {

                    dataContext.Producers.DeleteObject(producerToRemove);
                    dataContext.SaveChanges();
                }
                catch (Exception exception)
                {
                    ErrorHandler.ShowError("Не удалось удалить производителя", exception);
                }
            }
        }
    }
}
