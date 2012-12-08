using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace ColoritWPF
{
    //Radio buttons converter   
    public class EnumToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.Equals(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.Equals(true) ? parameter : Binding.DoNothing;
        }
    }

    class ServiceTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                using (ColorITEntities сolorItEntities = new ColorITEntities())
                {
                    var grId = (from service in сolorItEntities.ServiceTypes
                                where service.ID == (int)value
                                select service.ServiceName).First();

                    return grId.ToString(CultureInfo.InvariantCulture);
                }
            }
            return "Сервис не найден";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class CarModelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                using (ColorITEntities сolorItEntities = new ColorITEntities())
                {
                    var grId = (from car in сolorItEntities.CarModels
                                where car.ID == (int)value
                                select car.ModelName).First();

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
}
