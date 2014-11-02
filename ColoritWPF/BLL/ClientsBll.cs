using System;
using System.Collections.Generic;
using System.Linq;
using ColoritWPF.Common;

namespace ColoritWPF.BLL
{
    public class ClientsBll
    {
        public List<Client> GetClients()
        {
            using (var dataContext = new ColorITEntities())
            {
                return dataContext.Client.ToList();
            }
        }

        /// <summary>
        /// Обнуляет баланс клиента, например, в случае если в оплату товара входит баланс клиента
        /// </summary>
        /// <param name="id"></param>
        public void SetBalanceToZero(int id)
        {
            using (var dataContext = new ColorITEntities())
            {
                var client = dataContext.Client.First(cl => cl.ID == id);
                if (client == null)
                {
                    ErrorHandler.ShowError("Не удалось найти клиента");
                    return;
                }

                if (client.PrivatePerson)
                {
                    return;
                }

                try
                {
                    client.Balance = 0;
                    dataContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    ErrorHandler.ShowError("Не удалось сохранить изменения", ex);
                }
            }
        }

        public void SaveClient(Client newClient)
        {
            using (var dataContext = new ColorITEntities())
            {
                dataContext.Client.AddObject(newClient);

                try
                {
                    dataContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    ErrorHandler.ShowError("Не удалось сохранить нового клиента", ex);
                }
            }
        }

        public void ReturnCash(int clientId, decimal clientBalancePartInTotal)
        {
            using (var dataContext = new ColorITEntities())
            {
                var client = dataContext.Client.First(cl => cl.ID == clientId);
                if (client == null)
                {
                    ErrorHandler.ShowError("Не удалось найти клиента в базе");
                    return;
                }
                
                try
                {
                    client.Balance += clientBalancePartInTotal;
                    dataContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    ErrorHandler.ShowError("Не удалось вернуть деньги клиенту и сохранить изменения", ex);
                }
            }
        }
    }
}
