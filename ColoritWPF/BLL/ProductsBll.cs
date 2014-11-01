using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
