using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
//using LinqToExcel;
using Remotion.Data.Linq;
using System.Data;
using System.Data.OleDb;
using Remotion.Data.Linq.Collections;

namespace ColoritWPF
{
    /// <summary>
    /// Interaction logic for ImportFromExcel.xaml
    /// </summary>
    public partial class ImportFromExcel : Window
    {
        public ImportFromExcel()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            ListOfProducts listOfPr = new ListOfProducts();
            Product pr;
            // Initialize the linq to excel provider
            LinqToExcelProvider provider = new LinqToExcelProvider(@"C:\Price.xls");

            // Query the worksheet
            var query = from p in provider.GetWorkSheet("Sheet1")
                        select new
                        {
                            Name = Convert.ToString(p.Field<object>("Name")),
                            SelfCost = Convert.ToDecimal(p.Field<object>("SelfCost")),
                            Cost = Convert.ToDecimal(p.Field<object>("Cost")),
                            Warehouse = Convert.ToDouble(p.Field<object>("Warehouse"))
                        };


            using (ColorITEntities CIentity = new ColorITEntities())
            {
                foreach (var row in query)
                {
                    pr = new Product();
                    pr.Name = row.Name;
                    pr.SelfCost = row.SelfCost;
                    pr.Cost = row.Cost;
                    pr.Warehouse = row.Warehouse;
                    pr.Storage = 0;
                    pr.Bottled = false;
                    pr.MaxDiscount = 0;

                    if (!String.IsNullOrEmpty(row.Name))
                    {
                        listOfPr.Add(pr);
                        CIentity.AddToProduct(pr);
                    }
                }
                try
                {
                    CIentity.SaveChanges();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            dgv_Result.ItemsSource = listOfPr;
        }
    }

    public class ListOfProducts : ObservableCollection<Product> { }

    public class LinqToExcelProvider
    {
        /// <summary>
        /// Gets or sets the Excel filename
        /// </summary>
        private string FileName { get; set; }

        /// <summary>
        /// Template connectionstring for Excel connections
        /// </summary>
        private const string ConnectionStringTemplate = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=Excel 8.0;";

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="fileName">The Excel file to process</param>
        public LinqToExcelProvider(string fileName)
        {
            FileName = fileName;
        }

        /// <summary>
        /// Returns a worksheet as a linq-queryable enumeration
        /// </summary>
        /// <param name="sheetName">The name of the worksheet</param>
        /// <returns>An enumerable collection of the worksheet</returns>
        public EnumerableRowCollection<DataRow> GetWorkSheet(string sheetName)
        {
            // Build the connectionstring
            string connectionString = string.Format(ConnectionStringTemplate, FileName);

            // Query the specified worksheet
            OleDbDataAdapter dataAdapter = new OleDbDataAdapter(string.Format("SELECT * FROM [{0}$]", sheetName), connectionString);

            // Fill the dataset from the data adapter
            DataSet myDataSet = new DataSet();
            dataAdapter.Fill(myDataSet, "ExcelInfo");

            // Initialize a data table which we can use to enumerate the contents based on the dataset
            DataTable dataTable = myDataSet.Tables["ExcelInfo"];

            // Return the data table contents as a queryable enumeration
            return dataTable.AsEnumerable();
        }
    }
}
