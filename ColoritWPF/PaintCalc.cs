using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace ColoritWPF
{
    public partial class MainWindow
    {
        //Вариант когда добавляется краска
        public void AddPaint()
        {
            double amount = Convert.ToDouble(txtbxPaintAmount.Text);

            decimal sum = 0;// = SumAddition();
            decimal work;
            decimal ColoristSalary = 0;
            using (ColorITEntities colorItEntities = new ColorITEntities())
            {
                var _work = (from _paint in colorItEntities.PaintName
                            where _paint.ID == 32
                            select _paint.Cost).FirstOrDefault();
                work = decimal.Parse(_work.ToString());
            }
            if ((bool)rbCode.IsChecked)
            {
                sum = CountSumByCode();
                if((bool)cbColorist.IsChecked)
                {
                    ColoristSalary = work;
                }
            }

            PaintName paint = GetSelectedPaintName();
            using (ColorITEntities colorItEntities = new ColorITEntities())
            {
                Paints record = new Paints();
                record.Date = DateTime.Today.Date;
                record.CarModelID = (int)cmbxCarModel.SelectedValue;
                record.PaintCode = txtbxPaintCode.Text;
                if (cbThreeLayers.IsChecked != null) record.ThreeLayers = cbThreeLayers.IsChecked;
                record.NameID = paint.ID;
                record.TypeID = CodeOrSelection();
                record.Amount = amount;
                record.Sum = sum;
                record.Salary = ColoristSalary;
                record.ClientID = (int)cbClient.SelectedValue;
                record.DocState = true;
                record.PhoneNumber = txtbx_PhonNum.Text;
                try
                {
                    colorItEntities.AddToPaints(record);
                    colorItEntities.SaveChanges();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }

            }
        }

        //Плюсует в сумму код, подбор, 3слойную
        public decimal SumAddition()
        {
            decimal sum = 0;
            if (rbCode.IsChecked != null && (bool)rbCode.IsChecked)
                sum += 50;
            if (rbSelection.IsChecked != null && (bool)rbSelection.IsChecked)
                sum += 300;
            if (cbThreeLayers.IsChecked != null && (bool)cbThreeLayers.IsChecked)
                sum += 350;

            return sum;
        }

        //Проверякт по коду или подбор
        public int CodeOrSelection()
        {
            if (rbCode.IsChecked != null && (bool)rbCode.IsChecked)
                return 1;
            if (rbSelection.IsChecked != null && (bool)rbSelection.IsChecked)
                return 2;
            else
            {
                return 0;
            }
        }

        //Проверка зп колориста
        public decimal IsColoristSalary()
        {
            if (cbColorist.IsChecked != null && (bool)cbColorist.IsChecked)
                return 350;
            else
            {
                return 0;
            }
        }

        //Считает сумму по кол-ву краски + тара + 50 за работу
        public decimal CountSumByCode()
        {
            decimal amount = decimal.Parse(txtbxPaintAmount.Text);
            PaintName paint = GetSelectedPaintName();

            decimal sum = paint.Cost * amount + decimal.Parse(paint.Container.ToString());
            return sum;
        }
        
        //Вычисляет ВСЁ!
        public decimal CalculateTotalSumForPaints()
        {
            decimal polishSum = 0;
            decimal sum = 0;
            decimal amount = 0;
            decimal polishAmount = 0;
            decimal work = 0;
            decimal census = 0;
            decimal container = 0;
            PaintName paint = new PaintName();

            if (txtbxPaintAmount == null)
                return 0;
            if (!String.IsNullOrEmpty(txtbxPaintAmount.Text))
                amount = decimal.Parse(txtbxPaintAmount.Text, CultureInfo.InvariantCulture.NumberFormat);

            if(!String.IsNullOrEmpty(txtbxPolishAmount.Text))
                polishAmount = decimal.Parse(txtbxPolishAmount.Text);

            decimal discount = GetDiscount();
            if (cbPackage.IsChecked != null && (bool)cbPackage.IsChecked)
            {
                polishSum = AddToSumPackagePolish();
            }


            //вычисляет краску (если не выбран лак) и если по коду
            if (rbPolish.IsChecked != null && (rbCode.IsChecked != null && ((bool)rbPolish.IsChecked == false && (bool)rbCode.IsChecked)) && (rbOther.IsChecked != null && (bool)rbOther.IsChecked == false))
            {
                paint = GetSelectedPaintName();
                work = GetWorkValue(paint);
                container = decimal.Parse(paint.Container.ToString());
                sum = paint.Cost * amount + polishSum + container + work;
                return sum;
            }

            //вычисляет краску (если не выбран лак)
            if ((rbOther.IsChecked != null && rbPolish.IsChecked != null) && ((bool)rbPolish.IsChecked == false && (bool)rbOther.IsChecked == false))
            {
                paint = GetSelectedPaintName();
                census = GetCensus(paint);
                work = GetWorkValue(paint);
                container = decimal.Parse(paint.Container.ToString());
                sum = ((paint.Cost * (amount + census) + polishSum) * discount) + work + container;
                return sum;
            }

            //Вычисляет если выбран лак
            if (rbPolish.IsChecked != null && (bool)rbPolish.IsChecked)
            {
                paint = GetPolish();
                container = decimal.Parse(paint.Container.ToString());
                sum = ((paint.Cost * polishAmount) * discount) + container;
                return sum;
            }

            //вычисляет другое
            if (rbOther.IsChecked != null && (bool)rbOther.IsChecked)
            {
                paint = GetOther();
                if (paint == null)
                {
                    MessageBox.Show("Не могу найти краску из бокса 'другие'");
                    return 0;
                }

                container = decimal.Parse(paint.Container.ToString());
                sum = ((paint.Cost * amount) * discount) + container;
                return sum;
            }
            return 0;
        }

        //Достает сумму лак комплект (цена литр * кол-во + тара)
        public decimal AddToSumPackagePolish()
        {
            if (!String.IsNullOrEmpty(txtbxPolishAmount.Text))
            {
                decimal amount = Decimal.Parse(txtbxPolishAmount.Text);
                PaintName polish;
                using (ColorITEntities colorItEntities = new ColorITEntities())
                {
                    var getPolish = (from _paint in colorItEntities.PaintName
                                     where _paint.ID == 7
                                     select _paint).FirstOrDefault();
                    polish = getPolish;
                }
                return (polish.Cost*amount + (decimal) polish.Container);
            }
            return 0;
        }

        //Если по коду то возвращает сумму работы
        public decimal GetWorkValue(PaintName paint)
        {
            if (rbCode.IsChecked != null && (bool)rbCode.IsChecked)
            {
                using (ColorITEntities colorItEntities = new ColorITEntities())
                {
                    var _work = (from _paint in colorItEntities.PaintName
                                 where _paint.ID == 32
                                 select _paint.Cost).FirstOrDefault();
                    return decimal.Parse(_work.ToString());
                }
            }
            using (ColorITEntities colorItEntities = new ColorITEntities())
            {
                var _work = (from _paint in colorItEntities.PaintName
                                where _paint.ID == paint.ID
                                select _paint.Cost).FirstOrDefault();
                return decimal.Parse(_work.ToString());
            }
            
        }

        //Достает значение скидки клиента c учетом АВР и частной рожи
        public decimal GetDiscount()
        {
            if (cbClient.SelectedItem == null)
                return 1;
            if (cbClient.SelectedValue != null)
            {
                if ((int) cbClient.SelectedValue == 7) //частная рожа
                    return 1;

                decimal discount = 0;
                float abp = 0.02f;
                if (((Client) cbClient.SelectedItem).Discount != null)
                {
                    discount = (decimal)((Client) cbClient.SelectedItem).Discount;
                }
                if (rbABP.IsChecked != null && (bool) rbABP.IsChecked)
                    return (((100 - discount)/100) - (decimal) abp);
                return ((100 - discount)/100);
            }
            return 1;
        }
        
        //Добыть перепыл
        public decimal GetCensus(PaintName _paint)
        {
            if (String.IsNullOrEmpty(txtbxPaintAmount.Text))
                return 0;
            float f = 0.25f;
            if (decimal.Parse(txtbxPaintAmount.Text, CultureInfo.InvariantCulture.NumberFormat) < (decimal)f)
            {
                if (_paint.Census1 != null) 
                    return (decimal)_paint.Census1;
            }
            else
            {
                if (_paint.Census1 != null)
                    return (decimal)_paint.Census2;
            }
            return 0;
        }

        //Смотрит какая краска выбрана
        public PaintName GetSelectedPaintName()
        {
            using (ColorITEntities colorItEntities = new ColorITEntities())
            {
                if (rbL2K.IsChecked != null && (bool)rbL2K.IsChecked)
                {
                    if (rb_White.IsChecked != null && (bool)rb_White.IsChecked)
                    {
                     var paint = (from _paint in colorItEntities.PaintName
                                 where _paint.ID == 1
                                 select _paint).FirstOrDefault();
                        return paint;
                    }
                    if (rb_Color.IsChecked != null && (bool)rb_Color.IsChecked)
                    {
                        var paint = (from _paint in colorItEntities.PaintName
                                    where _paint.ID == 2
                                     select _paint).FirstOrDefault();
                        return paint;
                    }
                    if (rb_Red.IsChecked != null && (bool)rb_Red.IsChecked)
                    {
                        var paint = (from _paint in colorItEntities.PaintName
                                    where _paint.ID == 3
                                     select _paint).FirstOrDefault();
                        return paint;
                    }
                }
                if (rbLSB.IsChecked != null && (bool)rbLSB.IsChecked)
                {
                    if (cbThreeLayers.IsChecked != null && (bool)cbThreeLayers.IsChecked)
                    {
                        var paint = (from _paint in colorItEntities.PaintName
                                    where _paint.ID == 6
                                     select _paint).FirstOrDefault();
                        return paint;
                    }
                    if (cbThreeLayers.IsChecked != null && (bool)cbThreeLayers.IsChecked == false)
                    {
                        var paint = (from _paint in colorItEntities.PaintName
                                    where _paint.ID == 4
                                     select _paint).FirstOrDefault();
                        return paint;
                    }
                }
                if (rbABP.IsChecked != null && (bool)rbABP.IsChecked)
                {
                    if (cbThreeLayers.IsChecked != null && (bool)cbThreeLayers.IsChecked)
                    {
                        var paint = (from _paint in colorItEntities.PaintName
                                    where _paint.ID == 22
                                    select _paint).FirstOrDefault();
                        return paint;
                    }
                    if (cbThreeLayers.IsChecked != null && (bool)cbThreeLayers.IsChecked == false)
                    {
                        var paint = (from _paint in colorItEntities.PaintName
                                    where _paint.ID == 20
                                     select _paint).FirstOrDefault();
                        return paint;
                    }
                }
                return null;
            }
        }

        public void UpdateSelectedPaint(PaintMath _paintMath)
        {
            using (ColorITEntities colorItEntities = new ColorITEntities())
            {
                if (rbL2K.IsChecked != null && (bool)rbL2K.IsChecked)
                {
                    if (rb_White.IsChecked != null && (bool)rb_White.IsChecked)
                    {
                        var paint = (from _paint in colorItEntities.PaintName
                                     where _paint.ID == 1
                                     select _paint).FirstOrDefault();
                        _paintMath.SelectedPaint = paint;
                    }
                    if (rb_Color.IsChecked != null && (bool)rb_Color.IsChecked)
                    {
                        var paint = (from _paint in colorItEntities.PaintName
                                     where _paint.ID == 2
                                     select _paint).FirstOrDefault();
                        _paintMath.SelectedPaint = paint;
                    }
                    if (rb_Red.IsChecked != null && (bool)rb_Red.IsChecked)
                    {
                        var paint = (from _paint in colorItEntities.PaintName
                                     where _paint.ID == 3
                                     select _paint).FirstOrDefault();
                        _paintMath.SelectedPaint = paint;
                    }
                }
                if (rbLSB.IsChecked != null && (bool)rbLSB.IsChecked)
                {
                    if (cbThreeLayers.IsChecked != null && (bool)cbThreeLayers.IsChecked)
                    {
                        var paint = (from _paint in colorItEntities.PaintName
                                     where _paint.ID == 6
                                     select _paint).FirstOrDefault();
                        _paintMath.SelectedPaint = paint;
                    }
                    if (cbThreeLayers.IsChecked != null && (bool)cbThreeLayers.IsChecked == false)
                    {
                        var paint = (from _paint in colorItEntities.PaintName
                                     where _paint.ID == 4
                                     select _paint).FirstOrDefault();
                        _paintMath.SelectedPaint = paint;
                    }
                }
                if (rbABP.IsChecked != null && (bool)rbABP.IsChecked)
                {
                    if (cbThreeLayers.IsChecked != null && (bool)cbThreeLayers.IsChecked)
                    {
                        var paint = (from _paint in colorItEntities.PaintName
                                     where _paint.ID == 22
                                     select _paint).FirstOrDefault();
                        _paintMath.SelectedPaint = paint;
                    }
                    if (cbThreeLayers.IsChecked != null && (bool)cbThreeLayers.IsChecked == false)
                    {
                        var paint = (from _paint in colorItEntities.PaintName
                                     where _paint.ID == 20
                                     select _paint).FirstOrDefault();
                        _paintMath.SelectedPaint = paint;
                    }
                }
            }
        }

        public PaintName GetOther()
        {
            if (rbOther.IsChecked != null && (bool)rbOther.IsChecked)
            {
                using (ColorITEntities colorItEntities = new ColorITEntities())
                {
                    var paint = (from _paint in colorItEntities.PaintName
                                 where _paint.ID == (int) cmbxPaintName.SelectedValue
                                 select _paint).FirstOrDefault();
                    return paint;
                }
            }
            return null;
        }

        public PaintName GetPolish()
        {
            using (ColorITEntities colorItEntities = new ColorITEntities())
            {
                if ((bool) cbPackage.IsChecked)
                {
                    var paint = (from _paint in colorItEntities.PaintName
                                 where _paint.ID == 7
                                 select _paint).FirstOrDefault();
                    return paint;
                }
                if ((bool) cbPackage.IsChecked == false)
                {
                    var paint = (from _paint in colorItEntities.PaintName
                                 where _paint.ID == 8
                                 select _paint).FirstOrDefault();
                    return paint;
                }
                    return null;
            }
        }

    }

    public class Paint : INotifyPropertyChanged
    {
        
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }

        private Client GetDefaultClient()
        {
            using (ColorITEntities colorItEntities = new ColorITEntities())
            {
                var _client = (from clients in colorItEntities.Client
                               where clients.ID == 7
                               select clients).FirstOrDefault();
                return _client;
            }
        }

        public Paint()
        {
            _colorCode = String.Empty;
            _phoneNumber = String.Empty;
            _client = GetDefaultClient();
            _amount = 0;
            _polishAmount = 0;
            _isLsb = true;
            _isThreeLayers = false;
            _isPackage = false;
            _isCode = true;
            _isColorist = false;
            _other = new PaintName();
            _isL2K = false;
            _isAbp = false;
            _isPolish = false;
            _isOther = false;
            _isSelection = false;
            _isWhite = false;
            _isRed = false;
            _isColor = false;
            _sumOfGoods = 0;
        }

        #region Fields

        private string _colorCode;
        private string _phoneNumber;
        private Client _client;
        private PaintName _other;
        private bool _isLsb;
        private bool _isL2K;
        private bool _isAbp;
        private bool _isPolish;
        private bool _isOther;
        private bool _isPackage;
        private bool _isThreeLayers;
        private bool _isCode;
        private bool _isSelection;
        private bool _isColorist;
        private bool _isWhite;
        private bool _isColor;
        private bool _isRed;
        private decimal _amount;
        private decimal _polishAmount;
        private decimal _sumOfGoods;
        private decimal _prePay;
        private decimal _total;

        #endregion

        public PaintType PaintTypeProp { get; set; }
        public ServiceTypeEnum ServiceTypeProp { get; set; }
        public L2KTypeEnum L2KTypeProp { get; set; }

        public string ColorCode
        {
            get { return _colorCode; }
            set
            {
                _colorCode = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ColorCode"));
            }
        }

        public string PhoneNumber
        {
            get { return _phoneNumber; }
            set { _phoneNumber = value;
            OnPropertyChanged(new PropertyChangedEventArgs("PhoneNumber"));
            }
        }

        public bool IsLsb
        {
            get { return _isLsb; }
            set { _isLsb = value;
            OnPropertyChanged(new PropertyChangedEventArgs("LSB"));
            }
        }

        public bool IsL2K
        {
            get { return _isL2K; }
            set { _isL2K = value;
            OnPropertyChanged(new PropertyChangedEventArgs("L2K"));
            }
        }

        public bool IsAbp
        {
            get { return _isAbp; }
            set { _isAbp = value;
            OnPropertyChanged(new PropertyChangedEventArgs("ABP"));
            }
        }

        public bool IsPolish
        {
            get { return _isPolish; }
            set { _isPolish = value;
            OnPropertyChanged(new PropertyChangedEventArgs("Polish"));
            }
        }

        public bool IsOther
        {
            get { return _isOther; }
            set { _isOther = value;
            OnPropertyChanged(new PropertyChangedEventArgs("Other"));
            }
        }

        public bool IsPackage
        {
            get { return _isPackage; }
            set { _isPackage = value;
            OnPropertyChanged(new PropertyChangedEventArgs("Package"));
            }
        }

        public bool IsThreeLayers
        {
            get { return _isThreeLayers; }
            set { _isThreeLayers = value;
            OnPropertyChanged(new PropertyChangedEventArgs("ThreeLayers"));
            }
        }

        public bool IsCode
        {
            get { return _isCode; }
            set { _isCode = value;
            OnPropertyChanged(new PropertyChangedEventArgs("IsCode"));
            }
        }

        public bool IsSelection
        {
            get { return _isSelection; }
            set { _isSelection = value;
            OnPropertyChanged(new PropertyChangedEventArgs("Selection"));
            }
        }

        public bool IsColorist
        {
            get { return _isColorist; }
            set { _isColorist = value;
            OnPropertyChanged(new PropertyChangedEventArgs("Colorist"));
            }
        }

        public bool IsWhite
        {
            get { return _isWhite; }
            set { _isWhite = value;
            OnPropertyChanged(new PropertyChangedEventArgs("White"));
            }
        }

        public bool IsColor
        {
            get { return _isColor; }
            set { _isColor = value;
            OnPropertyChanged(new PropertyChangedEventArgs("Color"));
            }
        }

        public bool IsRed
        {
            get { return _isRed; }
            set { _isRed = value;
            OnPropertyChanged(new PropertyChangedEventArgs("Red"));
            }
        }

        public decimal Amount
        {
            get { return _amount; }
            set { _amount = value; OnPropertyChanged(new PropertyChangedEventArgs("Amount")); }
        }

        public decimal PolishAmount
        {
            get { return _polishAmount; }
            set { _polishAmount = value; OnPropertyChanged(new PropertyChangedEventArgs("PolishAmount")); }
        }

        public decimal SumOfGoods
        {
            get { return _sumOfGoods; }
            set { _sumOfGoods = value; OnPropertyChanged(new PropertyChangedEventArgs("SumOfGoods")); }
        }

        public decimal PrePay
        {
            get { return _prePay; }
            set { _prePay = value; OnPropertyChanged(new PropertyChangedEventArgs("PrePay")); }
        }

        public decimal Total
        {
            get { return _total; }
            set { _total = value; OnPropertyChanged(new PropertyChangedEventArgs("Total")); }
        }


        public Client Client
        {
            get { return _client; }
            set { _client = value; OnPropertyChanged(new PropertyChangedEventArgs("Client")); }
        }

        public PaintName Other
        {
            get { return _other; }
            set { _other = value; OnPropertyChanged(new PropertyChangedEventArgs("Other")); }
        }
    }

    public enum PaintType
    {
        LSB,
        L2K,
        ABP,
        Polish,
        Other
    };

    public enum ServiceTypeEnum
    {
        Code,
        Selection
    };

    public enum L2KTypeEnum
    {
        White,
        Color,
        Red
    };
}
