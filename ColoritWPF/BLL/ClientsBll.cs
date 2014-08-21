using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
