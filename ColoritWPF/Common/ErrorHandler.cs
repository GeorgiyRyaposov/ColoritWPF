using System;
using System.Windows;

namespace ColoritWPF.Common
{
    public static class ErrorHandler
    {
        public static void ShowError(string errorText, Exception ex)
        {
            var message = String.Format("{0}\n{1}\n{2} ", errorText, ex.Message, ex.InnerException);
            MessageBox.Show("Произошла ошибка", message, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static void ShowError(string errorText)
        {
            MessageBox.Show("Произошла ошибка", errorText, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
