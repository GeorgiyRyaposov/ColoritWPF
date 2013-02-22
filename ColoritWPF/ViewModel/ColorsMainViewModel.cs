using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using ColoritWPF.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

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
            }
        }
        private ColorITEntities colorItEntities;
        
        private Paints _currentPaint;
        //private PaintName _currentPaintName;

        public ObservableCollection<Client> Clients { get; set; }
        public ObservableCollection<PaintName> PaintNameList { get; set; }
        public ObservableCollection<CarModels> CarModels { get; set; }
        public ObservableCollection<Paints> Paints { get; set; }

        #region Radio buttons

        private bool _lsb;
        private bool _l2k;
        private bool _abp;
        private bool _polish;
        private bool _other;
        private bool _white;
        private bool _red;
        private bool _color;
        private bool _byCode;
        private bool _selection;


        public bool LSB
        {
            get { return _lsb; }
            set { _lsb = value;
            base.RaisePropertyChanged("LSB");}
        }

        public bool L2K
        {
            get { return _l2k; }
            set { _l2k = value;
                //SetPaint();
            base.RaisePropertyChanged("L2K");}
        }

        public bool ABP
        {
            get { return _abp; }
            set
            {
                _abp = value; SetPaint();
            base.RaisePropertyChanged("ABP");}
        }

        public bool Polish
        {
            get { return _polish; }
            set
            {
                _polish = value; SetPaint();
            base.RaisePropertyChanged("Polish");}
        }

        public bool Other
        {
            get { return _other; }
            set
            {
                _other = value; SetPaint();
            base.RaisePropertyChanged("Other");}
        }

        public bool White
        {
            get { return _white; }
            set
            {
                _white = value; SetPaint();
            base.RaisePropertyChanged("White");}
        }

        public bool Red
        {
            get { return _red; }
            set
            {
                _red = value; SetPaint();
            base.RaisePropertyChanged("Red");}
        }

        public bool Color
        {
            get { return _color; }
            set
            {
                _color = value; SetPaint();
            base.RaisePropertyChanged("Color");}
        }

        public bool ThreeLayers
        {
            get { return CurrentPaint.PaintName.ThreeLayers; }
            set
            {
                CurrentPaint.PaintName.ThreeLayers = value; SetPaint();
            base.RaisePropertyChanged("ThreeLayers");
            }
        }

        public bool Package
        {
            get { return CurrentPaint.PaintName.Package; }
            set
            {
                CurrentPaint.PaintName.Package = value; SetPaint();
                base.RaisePropertyChanged("Package");
            }
        }

        public bool ByCode
        {
            get { return CurrentPaint.ServiceByCode; }
            set
            {
                CurrentPaint.ServiceByCode = value; SetPaint();
                base.RaisePropertyChanged("ByCode");
            }
        }

        public bool Selection
        {
            get { return CurrentPaint.ServiceSelection; }
            set
            {
                CurrentPaint.ServiceSelection = value; SetPaint();
                base.RaisePropertyChanged("Selection");
            }
        }

        public bool Colorist
        {
            get { return CurrentPaint.ServiceColorist; }
            set
            {
                CurrentPaint.ServiceColorist = value; SetPaint();
                base.RaisePropertyChanged("Selection");
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
        

        private void GetData()
        {
            SetDefaultValues();

            Paints = new ObservableCollection<Paints>(colorItEntities.Paints.ToList());

            Clients = new ObservableCollection<Client>(colorItEntities.Client.ToList());
            PaintNameList = new ObservableCollection<PaintName>(colorItEntities.PaintName.ToList());
            CarModels = new ObservableCollection<CarModels>(colorItEntities.CarModels.ToList());
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
            ps.Salary = 0;
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


        //Initialize commands
        private void AddCommands()
        {
            ReCalcCommand = new RelayCommand(ReCalc);
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

            CurrentPaint.ReCalcAll(work);
        }

        #endregion

        #endregion

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
            }
        }

        public void SetPaint()
        {
            using (ColorITEntities colorItEntities = new ColorITEntities())
            {
                int pName;
                if (L2K)
                {
                    string l2KType = string.Empty;
                    if (White)
                        l2KType = "White";
                    if(Color)
                        l2KType = "Color";
                    if (Red)
                        l2KType = "Red";
                    pName = (from paintName in colorItEntities.PaintName
                                 where (
                                           (paintName.PaintType == "L2K") &&
                                           (paintName.L2KType == l2KType) &&
                                           (paintName.Package == Package) &&
                                           (paintName.ThreeLayers == ThreeLayers)
                                       )
                                 select paintName.ID).First();
                }
                else
                {
                    string paint = String.Empty;
                    if (LSB)
                        paint = "LSB";
                    if (ABP)
                        paint = "ABP";
                    if (Other)
                        paint = "Other";
                    if (Polish)
                        paint = "Polish";
                    pName = (from paintName in colorItEntities.PaintName
                                    where (
                                            (paintName.PaintType == paint) &&
                                            (paintName.Package == Package) &&
                                            (paintName.ThreeLayers == ThreeLayers)
                                        )
                                    select paintName.ID).First();                    
                }
                CurrentPaint.NameID = pName;
            }
        }
        /*
        public void GetSelectedPaint()
        {
            using (ColorITEntities colorItEntities = new ColorITEntities())
            {
                if (CurrentPaintName == null)
                    return;
                
                if (CurrentPaintName.L2K)
                {
                    if (CurrentPaintName.White)
                    {
                        var paint = (from _paint in colorItEntities.PaintName
                                     where _paint.ID == 1
                                     select _paint).FirstOrDefault();
                        CurrentPaintName = paint;
                    }
                    if (CurrentPaintName.Color)
                    {
                        var paint = (from _paint in colorItEntities.PaintName
                                     where _paint.ID == 2
                                     select _paint).FirstOrDefault();
                        CurrentPaintName = paint;
                    }
                    if (CurrentPaintName.Red)
                    {
                        var paint = (from _paint in colorItEntities.PaintName
                                     where _paint.ID == 3
                                     select _paint).FirstOrDefault();
                        CurrentPaintName = paint;
                    }
                }
                if (CurrentPaintName.LSB)
                {
                    if (CurrentPaintName.ThreeLayers)
                    {
                        var paint = (from _paint in colorItEntities.PaintName
                                     where _paint.ID == 6
                                     select _paint).FirstOrDefault();
                        CurrentPaintName = paint;
                    }
                    if (CurrentPaintName.ThreeLayers == false)
                    {
                        var paint = (from _paint in colorItEntities.PaintName
                                     where _paint.ID == 4
                                     select _paint).FirstOrDefault();
                        CurrentPaintName = paint;
                    }
                }
                if (CurrentPaintName.ABP)
                {
                    if (CurrentPaintName.ThreeLayers)
                    {
                        var paint = (from _paint in colorItEntities.PaintName
                                     where _paint.ID == 22
                                     select _paint).FirstOrDefault();
                        CurrentPaintName = paint;
                    }
                    if (CurrentPaintName.ThreeLayers == false)
                    {
                        var paint = (from _paint in colorItEntities.PaintName
                                     where _paint.ID == 20
                                     select _paint).FirstOrDefault();
                        CurrentPaintName = paint;
                    }
                }
            
            }
         
        }
         */
    }
}

