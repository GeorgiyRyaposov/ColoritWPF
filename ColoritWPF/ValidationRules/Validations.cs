using System.Globalization;
using System.Windows.Controls;

namespace ColoritWPF.ValidationRules
{
    class DensityProportionValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            ValidationResult result = new ValidationResult(true, null);
            int proportion;
            //if (value == null)
            //    return new ValidationResult(false, "Укажите значение пропорции");

            if (int.TryParse(value.ToString(), out proportion))
            {
                if (proportion > 0)
                {
                    return result;   
                }
            }
            return new ValidationResult(false, "Значение пропорции не может быть равным нулю!!!");
            
        }
    }
}
