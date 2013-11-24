using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace ColoritWPF
{
    public class PercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string value_str = value.ToString();
            if (String.IsNullOrWhiteSpace(value_str)) return null;

            value_str = value_str.TrimEnd(culture.NumberFormat.PercentSymbol.ToCharArray());

            double result;
            if (Double.TryParse(value_str, out result))
            {
                return result / 100.0;
            }
            return value;
        }
    }

    public class EnumToBoolConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value,
            Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            return parameter.Equals(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return parameter;
        }
        #endregion

    }

    public class CarModelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                using (ColorITEntities сolorItEntities = new ColorITEntities())
                {
                    var grId = (from car in сolorItEntities.CarModels
                                where car.ID == (int)value
                                select car.ModelName).FirstOrDefault();
                    if (grId == null)
                        return "Добавьте модель авто";
                    return grId.ToString(CultureInfo.InvariantCulture);
                }
            }
            else
                return "Модель не найдена";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class PaintNameConverter : IValueConverter
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
            return "Наименование краски не найдено";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ProductGroupConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            using (ColorITEntities CIentity = new ColorITEntities())
            {
                if (value != null)
                {
                    var grList = CIentity.Group.FirstOrDefault(c => c.ID == (int)value);
                    if (grList != null) 
                        return grList.Name;
                    return "Без группы";
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

    public class ClientNameConverter : IValueConverter
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

    public enum PaintTypes
    {
        LSB,
        L2K,
        ABP,
        Polish,
        Other
    }

    public enum L2KTypes
    {
        White,
        Red,
        Color
    }
}
