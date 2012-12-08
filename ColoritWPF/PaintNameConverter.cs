using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace ColoritWPF
{
    class PaintNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                using (ColorITEntities сolorItEntities = new ColorITEntities())
                {
                    var grId = (from paint in сolorItEntities.PaintName
                                where paint.ID == (int)value
                                select paint.Name).First();

                    return grId.ToString(CultureInfo.InvariantCulture);
                }
            }
            else
                return "Наименование краски не найдено";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
