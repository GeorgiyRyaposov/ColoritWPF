using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace ColoritWPF
{
    class ClientGroupConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                using (ColorITEntities colorEnt = new ColorITEntities())
                {
                    var grId = (from gr in colorEnt.ClientGroups
                                where gr.ID == (int)value
                                select gr.Name).First();

                    return grId.ToString();
                }
            }
            else
                return "Без группы";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
