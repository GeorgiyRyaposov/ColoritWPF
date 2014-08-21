using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ColoritWPF.BLL
{
    public class CarModelsBll
    {
        public List<CarModels> GetCarModelses()
        {
            using (var dataContext = new ColorITEntities())
            {
                return dataContext.CarModels.ToList();
            }
        }
    }
}
