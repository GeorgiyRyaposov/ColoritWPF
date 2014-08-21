using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColoritWPF.Common;

namespace ColoritWPF.BLL
{
    public class PaintNameBll
    {
        public List<PaintName> GetOtherPaints()
        {
            using (var dataContext = new ColorITEntities())
            {
                return dataContext.PaintName.Where(item => item.PaintType == PaintType.Other).ToList();
            }
        }
    }
}
