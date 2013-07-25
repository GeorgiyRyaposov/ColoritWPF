using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using ColoritWPF.Views.Products;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Excel = Microsoft.Office.Interop.Excel;

namespace ColoritWPF.ViewModel.Products
{
    public class ProductsViewModel : ViewModelBase
    {
        private ColorITEntities colorItEntities;

        public ProductsViewModel()
        {
            if (IsInDesignMode)
            {
                //SetDefaultValues();
            }
            else
            {
                colorItEntities = new ColorITEntities();

                GetData();
                InitializeCommands();
                

                SaleProductsList.CollectionChanged += CollectionChanged;
            }
        }
        
        #region Properties

        public ObservableCollection<Client> Clients { get; set; }
        public ObservableCollection<Product> SaleProductsList { get; set; }
        public ObservableCollection<SaleDocument> SaleDocuments { get; set; }

        private Client _currentClient;
        public Client CurrentClient
        {
            get
            {
                if (CurrentSaleDocument == null)
                    return Clients.First(client => client.PrivatePerson);
                return _currentClient;
            }
            set
            {
                CurrentSaleDocument.Client = value;
                _currentClient = value;
                CurrentDiscount = _currentClient.Discount;
                base.RaisePropertyChanged("CurrentClient");
                UpdateDiscountValue();
            }
        }

        private bool _includeClientBalanceToTotal;
        public bool IncludeClientBalanceToTotal
        {
            get { return _includeClientBalanceToTotal; }
            set
            {
                _includeClientBalanceToTotal = value;
                base.RaisePropertyChanged("IncludeClientBalanceToTotal");
                Recalc();
            }
        }

        public double CurrentDiscount
        {
            get
            {
                if (CurrentSaleDocument == null)
                    return 0;
                return CurrentSaleDocument.Discount;
            }
            set
            {
                CurrentSaleDocument.Discount = value;
                base.RaisePropertyChanged("CurrentDiscount");
                UpdateDiscountValue();
            }
        }

        private bool _prepay;
        public bool Prepay
        {
            get { return _prepay; }
            set
            {
                _prepay = value;
                base.RaisePropertyChanged("Prepay");
            }
        }

        private bool _confirmed;
        public bool Confirmed
        {
            get { return _confirmed; }
            set
            {
                _confirmed = value;
                base.RaisePropertyChanged("Confirmed");
            }
        }

        private SaleDocument _currentSaleDocument;
        public SaleDocument CurrentSaleDocument
        {
            get { return _currentSaleDocument; }
            set { 
                    _currentSaleDocument = value;
                    base.RaisePropertyChanged("CurrentSaleDocument");
                    
                    _currentClient = _currentSaleDocument.Client;
                    base.RaisePropertyChanged("CurrentClient");
                    base.RaisePropertyChanged("CurrentDiscount");
                    
                    _confirmButtonContent = value.Confirmed ? "Разпровести" : "Провести";
                    base.RaisePropertyChanged("ConfirmButtonContent");

                    UpdateListOfProductsForSale();
                }
        }

        private string _confirmButtonContent = "Провести";
        public string ConfirmButtonContent
        {
            get
            {
                return _confirmButtonContent;
            }
            set { 
                    _confirmButtonContent = value;
                    base.RaisePropertyChanged("ConfirmButtonContent");
                }
        }

        #endregion

        #region Methods

        private void GetData()
        {
            Clients = new ObservableCollection<Client>(colorItEntities.Client.ToList());
            SaleDocuments = new ObservableCollection<SaleDocument>(colorItEntities.SaleDocument.ToList());
            //CurrentSaleDocument = new SaleDocument();
            SaleProductsList = new ObservableCollection<Product>();

        }

        private void UpdateListOfProductsForSale()
        {
            if (SaleProductsList != null)
            {
                if (CurrentSaleDocument.Prepay || CurrentSaleDocument.Confirmed)
                {
                    SaleProductsList.Clear();
                    var saleProducts =
                        new ObservableCollection<Sale>(
                            colorItEntities.Sale.Where(product => product.SaleListNumber == CurrentSaleDocument.Id)
                                           .ToList());
                    foreach (Sale saleProduct in saleProducts)
                    {
                        saleProduct.Product.Amount = saleProduct.Amount;
                        SaleProductsList.Add(saleProduct.Product);
                    }
                }
            }
        }

        //Этот метод позволяет узнать когда изменился элемент коллекции
        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (Product item in e.OldItems)
                {
                    //Removed items
                    item.PropertyChanged -= Recalc;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (Product item in e.NewItems)
                {
                    //Added items
                    item.PropertyChanged += Recalc;
                }
            }
            Recalc();
            UpdateDiscountValue();
        }
        //А этот метод уже делает что надо когда узнает что элемент был изменен
        public void Recalc(object sender, PropertyChangedEventArgs e)
        {
            CurrentSaleDocument.CleanTotal = 0;
            CurrentSaleDocument.Total = 0;
            foreach (Product product in SaleProductsList)
            {
                CurrentSaleDocument.CleanTotal += product.CleanTotal;
                CurrentSaleDocument.Total += product.Total;
            }
        }
        public void Recalc()
        {
            if (CurrentSaleDocument != null)
            {
                CurrentSaleDocument.CleanTotal = 0;
                CurrentSaleDocument.Total = 0;
                foreach (Product product in SaleProductsList)
                {
                    CurrentSaleDocument.CleanTotal += product.CleanTotal;
                    CurrentSaleDocument.Total += product.Total;
                }
                if (IncludeClientBalanceToTotal)
                    CurrentSaleDocument.Total -= CurrentClient.Balance;
            }
        }

        /// <summary>
        /// Обновляет значение скидки для каждого продукта
        /// </summary>
        private void UpdateDiscountValue()
        {
            if (CurrentClient == null)
                return;

            foreach (Product product in SaleProductsList)
            {
                product.CurrentDiscount = CurrentDiscount;
            }
        }

        #endregion

        #region Commands

        public RelayCommand OpenSelectionCommand
        {
            get;
            private set;
        }

        public RelayCommand PrintDocumentCommand
        {
            get;
            private set;
        }


        public RelayCommand ConfirmDocumentCommand
        {
            get;
            private set;
        }

        public RelayCommand PrepayDocumentCommand
        {
            get;
            private set;
        }

        public RelayCommand MoveProductDlgCommand
        {
            get;
            private set;
        }

        private void InitializeCommands()
        {
            OpenSelectionCommand = new RelayCommand(OpenSelection);
            PrintDocumentCommand = new RelayCommand(PrintDocument, ConfirmDocumentCanExecute);
            ConfirmDocumentCommand = new RelayCommand(ConfirmDocument, ConfirmDocumentCanExecute);
            MoveProductDlgCommand = new RelayCommand(MoveProductDlg);
            PrepayDocumentCommand = new RelayCommand(PrepayDocument);
        }

        private bool ConfirmDocumentCanExecute()
        {
            return CurrentSaleDocument != null;
        }

        private void MoveProductDlg()
        {
            TransferProductView transferProductView = new TransferProductView();
            transferProductView.ShowDialog();
        }

        private void PrepayDocument()
        {
            Prepay = true;
            Confirmed = false;
            SaveDocument();
        }

        private void ConfirmDocument()
        {
            if (MessageBox.Show("Вы уверены что хотите "+ ConfirmButtonContent.ToLower()+" документ?",
                                "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (!CurrentSaleDocument.Confirmed)
                {
                    Prepay = false;
                    Confirmed = true;

                    if (CurrentSaleDocument.Id != 0) //Если документ уже есть в базе, просто проводим
                    {
                        OnlyConfirmDocument();
                    }
                    else // Иначе сохраняем документ, список продуктов и проводим
                    {
                        SaveDocument();
                    }
                }
                else
                {
                    Prepay = true;
                    Confirmed = false;
                    UnConfirmDocument();
                }
            }
        }

        /// <summary>
        /// Используется при проведении и сохранении документа, в зависимости от значений Confirmed и Prepay
        /// </summary>
        private void SaveDocument()
        {
            if (IsAllProductInStock() == false)
                return;

            var previousSaleDoc = (from n in colorItEntities.SaleDocument
                                   orderby n.Id descending
                                   select n).FirstOrDefault();

            if (previousSaleDoc == null)
                previousSaleDoc = new SaleDocument{SaleListNumber = 0};

            int num = previousSaleDoc.SaleListNumber;
            
            if (previousSaleDoc.DateCreated.Month != DateTime.Now.Month)
            {
                num = 0;
            }
            
            num++;

            CurrentSaleDocument.ClientId = CurrentClient.ID;
            CurrentSaleDocument.SaleListNumber = num;
            CurrentSaleDocument.Prepay = Prepay;
            CurrentSaleDocument.Confirmed = Confirmed;
            CurrentSaleDocument.DateCreated = DateTime.Now;

            //colorItEntities.SaleDocument.AddObject(CurrentSaleDocument);
            //SaleDocuments.Add(CurrentSaleDocument);

            try
            {
                colorItEntities.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось сохранить список продаж\n" + ex.Message + "\n" + ex.InnerException);
            }
            
            foreach (Product product in SaleProductsList)
            {
                Sale sale = new Sale
                {
                    ProductID = product.ID,
                    Amount = product.Amount,
                    Discount = product.CurrentDiscount,
                    Date = DateTime.Now,
                    ClientID = CurrentClient.ID,
                    FromStorage = product.Storage,
                    FromWareHouse = product.Warehouse,
                    Cost = product.Cost,
                    SaleListNumber = CurrentSaleDocument.Id,
                };

                if (Confirmed)
                {
                    var productStorage = colorItEntities.Product.First(pr => pr.ID == product.ID);
                    productStorage.Storage = productStorage.Storage - product.Amount;
                }
                
                colorItEntities.Sale.AddObject(sale);
            }

            if (IncludeClientBalanceToTotal && Confirmed)
            {
                Client client = colorItEntities.Client.First(cl => cl.ID == CurrentClient.ID);
                client.Balance = 0;
            }

            try
            {
                colorItEntities.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось сохранить список продаж\n"+ex.Message+"\n"+ex.InnerException);
            }
        }

        /// <summary>
        /// Используется при разпроведении документа
        /// </summary>
        private void UnConfirmDocument()
        {
            CurrentSaleDocument.Prepay = Prepay;
            CurrentSaleDocument.Confirmed = Confirmed;

            try
            {
                colorItEntities.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось сохранить список продаж\n" + ex.Message + "\n" + ex.InnerException);
            }

            foreach (Product product in SaleProductsList)
            {
                var productStorage = colorItEntities.Product.First(pr => pr.ID == product.ID);
                productStorage.Storage = productStorage.Storage + product.Amount;
            }

            //TODO спросить серегу что делать с деньгами клиента при перепроведении
            //if (IncludeClientBalanceToTotal)
            //{
            //    Client client = colorItEntities.Client.First(cl => cl.ID == CurrentClient.ID);
            //    client.Balance = 0;
            //}

            try
            {
                colorItEntities.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось сохранить список продаж\n" + ex.Message + "\n" + ex.InnerException);
            }
        }

        /// <summary>
        /// Используется при проведении документа, т.е. документ уже сохранен и его осталось только провести, или перепровести
        /// </summary>
        private void OnlyConfirmDocument()
        {
            if (IsAllProductInStock() == false)
                return;

            CurrentSaleDocument.Prepay = Prepay;
            CurrentSaleDocument.Confirmed = Confirmed;

            try
            {
                colorItEntities.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось сохранить список продаж\n" + ex.Message + "\n" + ex.InnerException);
            }

            foreach (Product product in SaleProductsList)
            {
                var productStorage = colorItEntities.Product.First(pr => pr.ID == product.ID);
                productStorage.Storage = productStorage.Storage - product.Amount;
            }

            if (IncludeClientBalanceToTotal)
            {
                Client client = colorItEntities.Client.First(cl => cl.ID == CurrentClient.ID);
                client.Balance = 0;
            }

            try
            {
                colorItEntities.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось сохранить список продаж\n" + ex.Message + "\n" + ex.InnerException);
            }
        }

        /// <summary>
        /// Проверяет наличие каждого товара из списка
        /// </summary>
        /// <returns>Возвращает false если товара в магазине не достаточно</returns>
        private bool IsAllProductInStock()
        {
            ObservableCollection<Product> notEnough = new ObservableCollection<Product>();
            foreach (Product product in SaleProductsList)
            {
                if (product.Storage < product.Amount)
                    notEnough.Add(product);
            }

            if (notEnough.Count > 0)
            {
                string msgText = String.Format("Недостаточно товара в магазине:\n");
                msgText = notEnough.Aggregate(msgText, (current, product) => current + product.Name + "\n");

                MessageBox.Show(msgText, "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        private void OpenSelection()
        {
            Messenger.Reset();
            Messenger.Default.Register<Product>(this, selectedItem => SaleProductsList.Add(selectedItem));

            CurrentSaleDocument = new SaleDocument
            {
                DateCreated = DateTime.Now,
                Client = Clients.First(client => client.PrivatePerson)
            };
            
            SaleDocuments.Add(CurrentSaleDocument);
            
            SaleProductsList.Clear();

            ProductsSelectorView productsSelector = new ProductsSelectorView();
            productsSelector.ShowDialog();

        }

        private void PrintDocument()
        {
            object misValue = System.Reflection.Missing.Value;

            Excel.ApplicationClass excel = new Excel.ApplicationClass();
            excel.Visible = true;

            Excel.Worksheet ws = Activate(excel);

            ws.get_Range("A1", misValue).Formula = "Расходная накладная №" + CurrentSaleDocument.DocumentNumber +
                                                                 " от " + CurrentSaleDocument.DateCreated;

            string inclInTotalBalance = IncludeClientBalanceToTotal ? "Включить баланс в оплату: да" : "Включить баланс в оплату: нет";

            ws.get_Range("A2", misValue).Formula = "Клиент: \t" + CurrentSaleDocument.Client.Name +
                                                                 " Баланс клиента: " + CurrentSaleDocument.Client.Balance.ToString("c") + inclInTotalBalance;

            ws.Range[ws.Cells[1, 1], ws.Cells[1, 8]].Merge();
            ws.Range[ws.Cells[2, 1], ws.Cells[2, 8]].Merge();

            string[] headers = { "№", "Артикул", "Наименование", "Количество", "Розн. цена", "Сумма", "% Скидки" };

            int row = 4;
            int col = 1;
            for (int i = 0; i < headers.Count(); i++)
            {
                AddItemToSpreadsheet(row, col, ws, headers[i]);
                AutoFitColumn(ws, col);
                CellOutLine(row, col, ws);
                col++;
            }

            row = 5;
            
            int index = 1;
            foreach (Product product in SaleProductsList)
            {
                col = 1;
                
                CellOutLine(row, col, ws);
                AddItemToSpreadsheet(row, col, ws, index.ToString()); col++; CellOutLine(row, col, ws);
                AddItemToSpreadsheet(row, col, ws, product.Article); col++; CellOutLine(row, col, ws);
                AddItemToSpreadsheet(row, col, ws, product.Name); AutoFitColumn(ws, col); col++; CellOutLine(row, col, ws); 
                AddItemToSpreadsheet(row, col, ws, product.Amount.ToString()); col++; CellOutLine(row, col, ws);
                AddItemToSpreadsheet(row, col, ws, product.Cost.ToString("c")); col++; CellOutLine(row, col, ws);
                AddItemToSpreadsheet(row, col, ws, product.Total.ToString("c")); AutoFitColumn(ws, col); col++; CellOutLine(row, col, ws);
                AddItemToSpreadsheet(row, col, ws, product.CurrentDiscount.ToString());

                index++;
                row++;
            }

            col = 6;
            row++;
            AddItemToSpreadsheet(row, col, ws, "Итого: " + CurrentSaleDocument.Total.ToString("c"));
            
            BoldRow(4, ws);
            BoldRow(row, ws);
        }
        #endregion
        
        
        public Excel.Worksheet Activate(Excel.ApplicationClass excel)
        {

            // open new excel spreadsheet

            Excel.Workbook workbook = excel.Workbooks.Add(Type.Missing);

            Excel.Worksheet ws = (Excel.Worksheet)excel.ActiveSheet;

            ws.Activate();

            return ws;

        }

        public void AddItemToSpreadsheet(int row, int column, Excel.Worksheet ws, string item)
        {

            ((Excel.Range)ws.Cells[row, column]).Value2 = item;

        }

        public void BoldRow(int row, Excel.Worksheet ws)
        {

            ((Excel.Range)ws.Cells[row, 1]).EntireRow.Font.Bold = true;

        }

        public void ColorRow(int row, int col, Excel.Worksheet ws)
        {

            ((Excel.Range)ws.Cells[row, col]).Interior.Color = System.Drawing.Color.FromArgb(255, 120, 120).ToArgb();

        }

        public void ColorAltRow(int row, int col, Excel.Worksheet ws)
        {

            ((Excel.Range)ws.Cells[row, col]).Interior.Color = System.Drawing.Color.FromArgb(255, 180, 180).ToArgb();

        }

        public void CellOutLine(int row, int col, Excel.Worksheet ws)
        {

            ((Excel.Range)ws.Cells[row, col]).Borders.ColorIndex = 1;

        }

        public void FormatColumn(Excel.Worksheet ws, int col, string format)
        {

            ((Excel.Range)ws.Cells[1, col]).EntireColumn.NumberFormat = format;

        }

        public void FormatColumnText(Excel.Worksheet ws, int col)
        {

            ((Excel.Range)ws.Cells[1, col]).EntireColumn.NumberFormat = "@";
            
        }

        public void SetColumnWidth(Excel.Worksheet ws, int col, int width)
        {

            ((Excel.Range)ws.Cells[1, col]).EntireColumn.ColumnWidth = width;

        }

        public void AutoFitColumn(Excel.Worksheet ws, int col)
        {

            ((Excel.Range)ws.Cells[1, col]).EntireColumn.AutoFit();

        }
    }
}
