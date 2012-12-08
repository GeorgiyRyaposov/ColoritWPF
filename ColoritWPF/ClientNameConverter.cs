using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace ColoritWPF
{
    class ClientNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                using (ColorITEntities сolorItEntities = new ColorITEntities())
                {
                    var grId = (from client in сolorItEntities.Client
                                where client.ID == (int)value
                                select client.Name).First();

                    return grId.ToString(CultureInfo.InvariantCulture);
                }
            }
            else
                return "Клиент не найден";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
