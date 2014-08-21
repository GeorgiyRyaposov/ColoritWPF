using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ColoritWPF.BLL
{
    public class PaintsBll
    {
        /// <summary>
        /// Return list of sold paints for last 10 days
        /// </summary>
        /// <returns></returns>
        public List<Paints> GetPaints()
        {
            using (var dataContext = new ColorITEntities())
            {
                return dataContext.Paints.Where(x =>  
                               x.Date.Year == DateTime.Now.Year
                            && x.Date.Month == DateTime.Now.Month
                            && x.Date.Day >= (DateTime.Now.Day-10)).ToList();
            }
        }


    }
}
