using System.Windows;
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
    }
}
