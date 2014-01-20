using System.Windows.Controls;

namespace ColoritWPF.Controls
{
    /// <summary>
    /// Interaction logic for AddExpenditure.xaml
    /// </summary>
    public partial class AddExpenditure : UserControl
    {
        public AddExpenditure()
        {
            InitializeComponent();
        }

        private void Grid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }
}
