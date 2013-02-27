using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;
using ColoritWPF.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace ColoritWPF.ViewModel
{
    public class ColorsMainViewModel : ViewModelBase
    {
        public ColorsMainViewModel()
        {
            if (IsInDesignMode)
            {
                SetDefaultValues();
            }
            else
            {
                colorItEntities = new ColorITEntities();
                GetData();
                AddCommands();
                Messenger.Default.Register<Client>(this, delegate(Client curClient) { Clients.Add(curClient); });
                Messenger.Default.Register<CarModels>(this, delegate(CarModels carModel) { CarModels.Add(carModel); });
            }
        }
        private ColorITEntities colorItEntities;
        private Paints _currentPaint;

        public ObservableCollection<Client> Clients { get; set; }
        public ObservableCollection<CarModels> CarModels { get; set; }
        public ObservableCollection<Paints> Paints { get; set; }
        public ObservableCollection<PaintName> OtherPaints { get; set; }
        
        #region Radio buttons

        private bool _lsb = true;
        private bool _l2k;
        private bool _abp;
        private bool _polish;
        private bool _other;
        private bool _white = true;
        private bool _red;
        private bool _color;
        private bool _package;
        private bool _threeLayers;

        public bool LSB
        {
            get { return _lsb; }
            set 
            { 
                _lsb = value;
                IsThreeLayers = value || ABP;
                base.RaisePropertyChanged("LSB");
            }
        }

        public bool L2K
        {
            get { return _l2k; }
            set 
            { 
                _l2k = value;
                base.RaisePropertyChanged("L2K");
            }
        }

        public bool ABP
        {
            get { return _abp; }
            set
            {
                _abp = value;
                IsThreeLayers = value || LSB;
                base.RaisePropertyChanged("ABP");
            }
        }

        public bool Polish
        {
            get { return _polish; }
            set
            {
                Package  = _polish = value;
                base.RaisePropertyChanged("Polish");
            }
        }

        public bool Other
        {
            get { return _other; }
            set
            {
                _other = value;
                base.RaisePropertyChanged("Other");
            }
        }

        public bool White
        {
            get { return _white; }
            set
            {
                _white = value;
                base.RaisePropertyChanged("White");
            }
        }

        public bool Red
        {
            get { return _red; }
            set
            {
                _red = value;
                base.RaisePropertyChanged("Red");
            }
        }

        public bool Color
        {
            get { return _color; }
            set
            {
                _color = value;
                base.RaisePropertyChanged("Color");
            }
        }

        public bool ThreeLayers
        {
            get { return _threeLayers; }
            set
            {
                _threeLayers = value; 
                base.RaisePropertyChanged("ThreeLayers");
            }
        }

        public bool Package
        {
            get { return _package; }
            set
            {
                _package = value; 
                base.RaisePropertyChanged("Package");
            }
        }

        public bool ByCode
        {
            get { return CurrentPaint.ServiceByCode; }
            set
            {
                CurrentPaint.ServiceByCode = value; 
                base.RaisePropertyChanged("ByCode");
            }
        }

        public bool Selection
        {
            get { return CurrentPaint.ServiceSelection; }
            set
            {
                CurrentPaint.ServiceSelection = value; 
                base.RaisePropertyChanged("Selection");
            }
        }

        public bool Colorist
        {
            get { return CurrentPaint.ServiceColorist; }
            set
            {
                CurrentPaint.ServiceColorist = value; 
                base.RaisePropertyChanged("Colorist");
            }
        }

        #endregion

        public Paints CurrentPaint
        {
            get
            {
                return _currentPaint;
            }
            set
            {
                _currentPaint = value;
                base.RaisePropertyChanged("CurrentPaint");
                SetRadio();
            }
        }

        private bool _docConfirmed;
        public bool DocConfirmed
        {
            get { return _docConfirmed; }
            set { _docConfirmed = value;
            base.RaisePropertyChanged("DocConfirmed");
            }
        }

        private double _discount;
        public double Discount
        {
            get { return _discount; }
            set
            {
                _discount = value;
                base.RaisePropertyChanged("Discount");
            }
        }

        private bool _isThreeLayers;
        public bool IsThreeLayers
        {
            get { return _isThreeLayers; }
            set
            {
                _isThreeLayers = value;
                base.RaisePropertyChanged("IsThreeLayers");
            }
        }

        private int _currentClientId = 7;
        public int CurrentClientId
        {
            get { return _currentClientId; }
            set
            {
                _currentClientId = value;
                CurrentPaint.ClientID = value;

                SetDiscount();
                ReCalc();
                base.RaisePropertyChanged("CurrentClientId");
            }
        }

        private void GetData()
        {
            SetDefaultValues();

            Paints = new ObservableCollection<Paints>(colorItEntities.Paints.ToList());
            
            Clients = new ObservableCollection<Client>(colorItEntities.Client.ToList());
            CarModels = new ObservableCollection<CarModels>(colorItEntities.CarModels.ToList());
            OtherPaints = new ObservableCollection<PaintName>(colorItEntities.PaintName.Where(item => item.PaintType == "Other").ToList());
        }

        private void SetDefaultValues()
        {
            PaintName pn = new PaintName();
            pn.PaintType = "LSB";
            pn.Package = false;
            pn.ThreeLayers = false;

            Paints ps = new Paints();
            ps.CarModelID = 3;
            ps.PaintCode = "Введите код краски";
            ps.NameID = 4;
            ps.Sum = 0;
            ps.Prepay = 0;
            ps.Total = 0;
            ps.ClientID = 7;
            ps.ServiceByCode = false;
            ps.ServiceSelection = true;
            ps.ServiceColorist = true;
            ps.PaintName = pn;

            CurrentPaint = ps;
        }

        #region Commands

        #region fields
        public RelayCommand ReCalcCommand
        {
            get;
            private set;
        }
        public RelayCommand SetPaintCommand
        {
            get;
            private set;
        }
        public RelayCommand AddPaintCommand
        {
            get;
            private set;
        }
        public RelayCommand SaveChangesCommand
        {
            get;
            private set;
        }
        public RelayCommand ConfirmDocumentCommand
        {
            get;
            private set;
        }
        public RelayCommand UnConfirmDocumentCommand
        {
            get;
            private set;
        }
        public RelayCommand PreorderCommand
        {
            get;
            private set;
        }
        public RelayCommand AddNewClientCommand
        {
            get;
            private set;
        }
        public RelayCommand EditClientCommand
        {
            get;
            private set;
        }

        public RelayCommand AddNewCarModelCommand
        {
            get;
            private set;
        }

        //Initialize commands
        private void AddCommands()
        {
            ReCalcCommand = new RelayCommand(ReCalc);
            SetPaintCommand = new RelayCommand(SetPaintAndRecalc, SetPaintCanExecute);
            AddPaintCommand = new RelayCommand(AddPaint);
            SaveChangesCommand = new RelayCommand(SaveChanges, SaveChangesCanExecute);
            ConfirmDocumentCommand = new RelayCommand(ConfirmDoc, ConfirmDocCanExecute);
            UnConfirmDocumentCommand = new RelayCommand(UnConfirmDoc, UnConfirmDocCanExecute);
            PreorderCommand = new RelayCommand(Preorder, PreorderCanExecute);
            AddNewClientCommand = new RelayCommand(AddNewClientCmd);
            AddNewCarModelCommand = new RelayCommand(AddNewCarModelCmd);
            EditClientCommand = new RelayCommand(EditClientCmd);
        }

        private void EditClientCmd()
        {
            ClientEditor clientEditor = new ClientEditor();
            clientEditor.ShowDialog();
        }

        private void AddNewCarModelCmd()
        {
            AddNewCarModel addNewCar = new AddNewCarModel();
            addNewCar.ShowDialog();
        }

        private void AddNewClientCmd()
        {
            AddNewClient addNewClient = new AddNewClient();
            addNewClient.ShowDialog();
        }

        private void Preorder()
        {
            ReCalc();
            if (!CurrentPaint.Client.PrivatePerson)
                CurrentPaint.Client.Balance = CurrentPaint.Client.Balance - CurrentPaint.Total;
            SaveWithoutRecalc();
        }

        private bool PreorderCanExecute()
        {
            return !CurrentPaint.DocState;
        }

        private bool SaveChangesCanExecute()
        {
            return !CurrentPaint.DocState;
        }

        private bool ConfirmDocCanExecute()
        {
            return !CurrentPaint.DocState;
        }

        private bool UnConfirmDocCanExecute()
        {
            return CurrentPaint.DocState;
        }

        private void SetPaintAndRecalc()
        {
            SetPaint();
            ReCalc();
        }

        private bool SetPaintCanExecute()
        {
            return !CurrentPaint.DocState;
        }

        private void ConfirmDoc()
        {
            if (MessageBox.Show("Вы уверены что хотите провести документ?",
                                "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                DocConfirmed = false;
                CurrentPaint.DocState = true;
                CurrentPaint.PhoneNumber = CurrentPaint.Client.PhoneNumber;
                CurrentPaint.Client.Balance = CurrentPaint.Client.Balance + CurrentPaint.Total;
                SaveWithoutRecalc();
            }
        }

        //Распровести
        private void UnConfirmDoc()
        {
            if (MessageBox.Show("Вы уверены что хотите разпровести документ?",
                                "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (MessageBox.Show("Вы точно-точно уверены что хотите разпровести документ?",
                                    "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    DocConfirmed = true;
                    CurrentPaint.DocState = false;
                    SaveWithoutRecalc();
                }
            }
        }
        
        //Сохранение без пересчета
        private void SaveWithoutRecalc()
        {
            try
            {
                colorItEntities.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Сохранить изменения не удалось\n" + ex.Message);
            }
        }

        //Сохранение с пересчетом
        private void SaveChanges()
        {
            try
            {
                ReCalc();
                CurrentPaint.PhoneNumber = CurrentPaint.Client.PhoneNumber;
                colorItEntities.SaveChanges();
            }
            catch (OptimisticConcurrencyException ex)
            {
                throw new OptimisticConcurrencyException("Сохранить изменения не удалось\n" + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Сохранить изменения не удалось\n"+ex.Message);
            }
        }

        private void AddPaint()
        {
            Paints ps = new Paints
                            {
                                Date = DateTime.Now,
                                CarModelID = 3,
                                PaintCode = "Введите код краски",
                                NameID = 4,
                                Amount = 0,
                                Sum = 0,
                                ClientID = 7,
                                DocState = false,
                                PhoneNumber = String.Empty,
                                ServiceByCode = false,
                                ServiceSelection = true,
                                ServiceColorist = true,
                                AmountPolish = 0,
                                Prepay = 0,
                                Total = 0
                            };

            Paints.Add(ps);
            CurrentPaint = ps;

            colorItEntities.Paints.AddObject(ps);
        }

        private void ReCalc()
        {
            decimal work = 0;
            if (ByCode)
                work = 50;
            if(Selection)
                work = 330;
            if (Selection && ThreeLayers)
                work = 380;

            CurrentPaint.ReCalcAll(work, (decimal)Discount);
        }

        #endregion

        #endregion

        //Выставляет радио кнопки в зависимости от выбраной краски
        private void SetRadio()
        {
            if (CurrentPaint.PaintName != null)
            {
                switch (CurrentPaint.PaintName.PaintType)
                {
                    case "LSB":
                        LSB = true;
                        break;
                    case "L2K":
                        L2K = true;
                        break;
                    case "ABP":
                        ABP = true;
                        break;
                    case "Polish":
                        Polish = true;
                        break;
                    case "Other":
                        Other = true;
                        break;
                }
                switch (CurrentPaint.PaintName.L2KType)
                {
                    case "White":
                        White = true;
                        break;
                    case "Red":
                        Red = true;
                        break;
                    case "Color":
                        Color = true;
                        break;
                    case "None":
                        break;
                }

                ThreeLayers = CurrentPaint.PaintName.ThreeLayers;
                Package = CurrentPaint.PaintName.Package;
                ByCode = CurrentPaint.ServiceByCode;
                Selection = CurrentPaint.ServiceSelection;
                Colorist = CurrentPaint.ServiceColorist;
                DocConfirmed = !CurrentPaint.DocState;
                CurrentClientId = CurrentPaint.ClientID;
                SetDiscount();
            }
        }

        //Выставляет скидку в зависимости от выбраного клиента
        private void SetDiscount()
        {
            if(CurrentPaint.Client != null)
                Discount = !CurrentPaint.Client.PrivatePerson ? CurrentPaint.PaintName.MaxDiscount : 0;
        }

        //Выставляет краски в зависимости от выбраной радио кнопки
        private void SetPaint()
        {
            using (ColorITEntities colorItEntities = new ColorITEntities())
            {
                int pName;

                if (LSB || ABP)
                {
                    string paint = String.Empty;
                    if (LSB)
                        paint = "LSB";
                    if (ABP)
                        paint = "ABP";

                    pName = (from paintName in colorItEntities.PaintName
                             where (
                                       (paintName.PaintType == paint) &&
                                       (paintName.Package == Package) &&
                                       (paintName.ThreeLayers == ThreeLayers)
                                   )
                             select paintName.ID).First();
                    CurrentPaint.NameID = pName;
                    SetDiscount();
                    return;
                }

                if (_l2k)
                {
                    string l2KType = string.Empty;
                    if (_white)
                        l2KType = "White";
                    if(_color)
                        l2KType = "Color";
                    if (_red)
                        l2KType = "Red";
                    pName = (from paintName in colorItEntities.PaintName
                                 where (
                                           (paintName.PaintType == "L2K") &&
                                           (paintName.L2KType == l2KType) &&
                                           (paintName.Package == Package) &&
                                           (paintName.ThreeLayers == ThreeLayers)
                                       )
                                 select paintName.ID).First();
                    CurrentPaint.NameID = pName;
                    SetDiscount();
                    return;
                }

                if (Polish)
                {   
                    pName = (from paintName in colorItEntities.PaintName
                             where (
                                     (paintName.L2KType == "Polish") &&
                                     (paintName.Package == Package) &&
                                     (paintName.ThreeLayers == ThreeLayers)
                                 )
                             select paintName.ID).First();
                    CurrentPaint.NameID = pName;
                    SetDiscount();
                    return;
                }

                if(Other)
                    SetDiscount();
            }
        }
    }
}

