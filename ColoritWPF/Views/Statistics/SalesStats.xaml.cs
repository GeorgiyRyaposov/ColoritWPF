using System.Windows;
using System.Windows.Controls;

namespace ColoritWPF.Views.Statistics
{
    /// <summary>
    /// Interaction logic for SalesStats.xaml
    /// </summary>
    public partial class SalesStats : Window
    {
        public SalesStats()
        {
            InitializeComponent();
        }

        private void Grid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }
}
