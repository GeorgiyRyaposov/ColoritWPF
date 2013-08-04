using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;

namespace ColoritWPF.Views.Products
{
    /// <summary>
    /// Interaction logic for TransferProductsSelector.xaml
    /// </summary>
    public partial class TransferProductsSelector : Window
    {
        public TransferProductsSelector()
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage>(this, (nm) =>
            {
                if (nm.Notification == "CloseWindowsBoundToMe")
                {
                    //if (nm.Sender == this.DataContext)
                    this.Close();
                }
            });
        }

        private void dg_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
