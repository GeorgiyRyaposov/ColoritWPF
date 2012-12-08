using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace ColoritWPF
{
    public class ProductGroupConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            using (ColorITEntities CIentity = new ColorITEntities())
            {
                if (value != null)
                {
                    var grList = CIentity.Group.FirstOrDefault(c => c.ID == (int)value);
                    return ((Group)grList).Name.ToString();
                }
                else
                    return "Без группы";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException("Этот конвертер только для группировки");
        }
    }
}
