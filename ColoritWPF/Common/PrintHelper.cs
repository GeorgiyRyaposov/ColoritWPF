using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ColoritWPF.Common
{
    public class PrintHelper
    {
        public void PrintSaleDocument(Visual v, SaleDocument saleDocument)
        {
            var printDlg = new PrintDialog();

            if (printDlg.ShowDialog() != true)
            {
                return;
            }

            var myPanel = new StackPanel {Margin = new Thickness(5, 5, 5, 5)};

            var myBlock = new TextBlock
            {
                Text = "Расходная накладная №" + saleDocument.DocumentNumber +
                       " от " + saleDocument.DateCreated,
                TextAlignment = TextAlignment.Left,
                Margin = new Thickness(5, 5, 5, 5)
            };

            myPanel.Children.Add(myBlock);

            var clientBalance = new TextBlock
            {
                Text = "Клиент: \t" + saleDocument.Client.Name,
                Margin = new Thickness(5, 5, 5, 5)
            };

            myPanel.Children.Add(clientBalance);

            var dataGrid = v as DataGrid;
            var printDataGrid = new DataGrid {Margin = new Thickness(5, 5, 5, 5)};
            printDataGrid.LoadingRow += LoadingRow;

            printDataGrid.RowHeaderTemplate = dataGrid.RowHeaderTemplate;
            printDataGrid.RowHeaderWidth = dataGrid.RowHeaderWidth;

            foreach (var dataGridColumn in dataGrid.Columns)
            {
                var item = (DataGridTextColumn) dataGridColumn;
                printDataGrid.Columns.Add(new DataGridTextColumn
                {
                    Width = new DataGridLength(1.0, DataGridLengthUnitType.Auto),
                    Header = item.Header,
                    Binding = item.Binding
                });
            }

            foreach (Sale item in dataGrid.Items)
            {
                printDataGrid.Items.Add(new Sale
                {
                    ID = item.ID,
                    Product = item.Product,
                    ProductID = item.ProductID,
                    Amount = item.Amount,
                    Cost = item.Cost,
                    CurrentDiscount = item.CurrentDiscount,
                    Total = item.Total
                });
            }

            myPanel.Children.Add(printDataGrid);

            var totalValue = new TextBlock
            {
                Text = "Итого: " + saleDocument.Total.ToString("c"),
                Margin = new Thickness(5, 5, 5, 5)
            };

            myPanel.Children.Add(totalValue);

            var grid = new Grid();
            var columnDefinition = new ColumnDefinition {Width = new GridLength(1.0, GridUnitType.Auto)};
            grid.Children.Add(myPanel);

            grid.ColumnDefinitions.Add(columnDefinition);
            printDlg.PrintVisual(grid, "Grid Printing.");
        }

        private void LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString(CultureInfo.InvariantCulture);
        }
    }
}
