using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;

namespace ColoritWPF.Views.Products
{
    /// <summary>
    /// Interaction logic for ProductsSelectorView.xaml
    /// </summary>
    public partial class ProductsSelectorView : Window
    {
        public ProductsSelectorView()
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void dg_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }
}
