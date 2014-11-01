using System;
using System.Linq;
using ColoritWPF.Common;

namespace ColoritWPF.BLL
{
    public class SettingsBll
    {
        public void AddCashToStorageBalance(Decimal cash)
        {
            using (var dataContext = new ColorITEntities())
            {
                var settings = dataContext.Settings.First();
                settings.Cash += cash;

                try
                {
                    dataContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    ErrorHandler.ShowError("Не удалось сохранить данные при изменении кассы", ex);
                    throw;
                }
            }
        }

    }
}
