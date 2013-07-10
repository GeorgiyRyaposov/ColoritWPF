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
                settings = colorItEntities.Settings.First();
                GetData();
                AddCommands();
                Messenger.Default.Register<Client>(this, curClient => Clients.Add(curClient));
                Messenger.Default.Register<CarModels>(this, carModel => CarModels.Add(carModel));
                Messenger.Default.Register<Settings>(this, setting => settings = setting);
            }
        }
        private ColorITEntities colorItEntities;
        private Paints _currentPaint;
        private Settings settings;

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
        //private bool _package;
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
                /*Package  = */_polish = value;
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

        //public bool Package
        //{
        //    get { return _package; }
        //    set
        //    {
        //        _package = value; 
        //        base.RaisePropertyChanged("Package");
        //    }
        //}

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

        private bool _isEnabled;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { _isEnabled = value;
            base.RaisePropertyChanged("IsEnabled");
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
                PhoneNumber = CurrentPaint.Client.PhoneNumber;
                SetDiscount();
                ReCalc();
                base.RaisePropertyChanged("CurrentClientId");
            }
        }

        private string _phoneNumber = String.Empty;
        public string PhoneNumber
        {
            get { return _phoneNumber; }
            set
            {
                _phoneNumber = value;
                base.RaisePropertyChanged("PhoneNumber");
            }
        }
        
        private void GetData()
        {
            Paints = new ObservableCollection<Paints>(colorItEntities.Paints.ToList());
            
            Clients = new ObservableCollection<Client>(colorItEntities.Client.ToList());
            CarModels = new ObservableCollection<CarModels>(colorItEntities.CarModels.ToList());
            OtherPaints = new ObservableCollection<PaintName>(colorItEntities.PaintName.Where(item => item.PaintType == "Other").ToList());

            //SetDefaultValues();
            CurrentPaint = Paints.LastOrDefault();
        }

        private void SetDefaultValues()
        {
            Paints ps = new Paints
                            {
                                Date = DateTime.Now,
                                CarModelID = 3,
                                PaintCode = "Введите код краски",
                                NameID = 4,
                                Sum = 0,
                                Prepay = 0,
                                Total = 0,
                                ServiceByCode = false,
                                ServiceSelection = true,
                                ServiceColorist = true,
                                ClientID = colorItEntities.Client.First(i => i.PrivatePerson).ID
                            };

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

        public RelayCommand CancelPreorderCommand
        {
            get;
            private set;
        }

        public RelayCommand EditPaintsCommand
        {
            get;
            private set;
        }

        public RelayCommand SettingsCommand
        {
            get;
            private set;
        }

        public RelayCommand PaintsSalesWatcherCommand
        {
            get;
            private set;
        }

        public RelayCommand DensityCounterCommand
        {
            get;
            private set;
        }

        public RelayCommand AddNewDensityCommand
        {
            get;
            private set;
        }
        
        #endregion

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
            CancelPreorderCommand = new RelayCommand(CancelPreorderCmd, CancelPreorderCanExecute);
            EditPaintsCommand = new RelayCommand(EditPaintsCmd);
            SettingsCommand = new RelayCommand(SettingsCmd);
            PaintsSalesWatcherCommand = new RelayCommand(PaintsSalesWatcherCmd);
            DensityCounterCommand = new RelayCommand(DensityCounterCmd);
            AddNewDensityCommand = new RelayCommand(AddNewDensityCmd);
        }

        private void AddNewDensityCmd()
        {
            AddNewDensityItem addNewDensity = new AddNewDensityItem();
            addNewDensity.ShowDialog();
        }

        private void DensityCounterCmd()
        {
            DensityView densityView = new DensityView();
            densityView.ShowDialog();
        }

        private void PaintsSalesWatcherCmd()
        {
            PaintsSalesWatcherView paintsSalesWatcherView = new PaintsSalesWatcherView();
            paintsSalesWatcherView.ShowDialog();
        }

        private void SettingsCmd()
        {
            SettingsView settingsView = new SettingsView();
            settingsView.ShowDialog();
        }

        private void EditPaintsCmd()
        {
            PaintsEditor paintsEditor = new PaintsEditor();
            paintsEditor.ShowDialog();
        }

        private bool CancelPreorderCanExecute()
        {
            return CurrentPaint.IsPreorder;
        }

        private void CancelPreorderCmd()
        {
            if (CurrentPaint.Client.PrivatePerson == false)
                CurrentPaint.Client.Balance = CurrentPaint.Client.Balance + CurrentPaint.Total;
            CurrentPaint.IsPreorder = false;
            SaveWithoutRecalc();
            IsEnabled = true;
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
            CurrentPaint.IsPreorder = true;
            SaveWithoutRecalc();
            IsEnabled = false;
        }

        private bool PreorderCanExecute()
        {
            return !CurrentPaint.IsPreorder;
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
            if(IsEnabled)
                ReCalc();
        }

        private bool SetPaintCanExecute()
        {
            if (CurrentPaint.DocState || CurrentPaint.IsPreorder)
                return false;
            return true;
        }

        private void ConfirmDoc()
        {
            if (MessageBox.Show("Вы уверены что хотите провести документ?",
                                "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                CurrentPaint.DocState = true;
                CurrentPaint.Client.Balance = CurrentPaint.Client.Balance + CurrentPaint.Total;
                SaveWithoutRecalc();
                IsEnabled = false;
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
                    IsEnabled = true;
                    CurrentPaint.DocState = false;
                    CurrentPaint.IsPreorder = false;
                    SaveWithoutRecalc();
                }
            }
        }
        
        //Сохранение без пересчета
        private void SaveWithoutRecalc()
        {
            try
            {
                CurrentPaint.PhoneNumber = PhoneNumber;
                colorItEntities.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Сохранить изменения не удалось\n" + ex.Message + "\n" + ex.InnerException);
            }
        }

        //Сохранение с пересчетом
        private void SaveChanges()
        {
            try
            {
                ReCalc();
                CurrentPaint.PhoneNumber = PhoneNumber;
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
                                ClientID = colorItEntities.Client.First(i => i.PrivatePerson).ID,
                                DocState = false,
                                IsPreorder = false,
                                PhoneNumber = String.Empty,
                                ServiceByCode = false,
                                ServiceSelection = true,
                                ServiceColorist = true,
                                AmountPolish = 0,
                                Prepay = 0,
                                Total = 0
                            };

            Paints.Add(ps);
            colorItEntities.Paints.AddObject(ps);
            CurrentPaint = ps;
        }

        private void ReCalc()
        {
            if (IsEnabled)
            {
                decimal work = 0;
                if (ByCode)
                    work = settings.ByCodeCost;
                if (Selection)
                    work = settings.SelectionCost;
                if (Selection && ThreeLayers)
                    work = settings.SelectionAndThreeLayers;

                CurrentPaint.ReCalcAll(work, (decimal)Discount, ByCode);
            }
        }

        

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

                if (CurrentPaint.DocState || CurrentPaint.IsPreorder)
                    IsEnabled = false;
                else
                    IsEnabled = true;

                ThreeLayers = CurrentPaint.PaintName.ThreeLayers;
                //Package = CurrentPaint.PaintName.Package;
                ByCode = CurrentPaint.ServiceByCode;
                Selection = CurrentPaint.ServiceSelection;
                Colorist = CurrentPaint.ServiceColorist;
                CurrentClientId = CurrentPaint.ClientID;
                PhoneNumber = CurrentPaint.PhoneNumber;
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
                                       //(paintName.Package == Package) &&
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
                                           //(paintName.Package == Package) &&
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
                                     //(paintName.Package == Package) &&
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

