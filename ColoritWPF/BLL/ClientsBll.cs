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
    }
}
