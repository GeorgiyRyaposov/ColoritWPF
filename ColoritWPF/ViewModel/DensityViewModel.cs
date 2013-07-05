using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using ColoritWPF.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace ColoritWPF.ViewModel
{
    public class DensityViewModel : ViewModelBase
    {
        private ColorITEntities colorItEntities;

        public DensityViewModel()
        {
            if (IsInDesignMode)
            {
                //SetDefaultValues();
            }
            else
            {
                colorItEntities = new ColorITEntities();
                //settings = colorItEntities.Settings.First();
                GetData();
                //AddCommands();
                //Messenger.Default.Register<Client>(this, curClient => Clients.Add(curClient));
                //Messenger.Default.Register<CarModels>(this, carModel => CarModels.Add(carModel));
                //Messenger.Default.Register<Settings>(this, setting => settings = setting);
            }
        }

        #region Properties

        private DensityType _currentType;
        private Density _currentDensity;
        private Density _currentGruntMixDensity;
        private double _volume;
        private double _mass;
        private double _volHardener;
        private double _volThinner;
        private double _massHardener;
        private double _massThinner;
        private double _leftProportion;
        private double _rightProportion;
        private ICollectionView _densitiesComboView;
        private ICollectionView _gruntDensitiesComboView;
        private string leftGruntMass;
        private string rightGruntMass;
        
        public ObservableCollection<DensityType> DenTypes { get; set; }
        public ObservableCollection<Density> Densities { get; set; }
        public ObservableCollection<Density> GruntDensities { get; set; }

        public DensityType CurrentType
        {
            get { return _currentType; }
            set { _currentType = value;
                base.RaisePropertyChanged("CurrentType");
                _densitiesComboView.Refresh();}
        }

        public Density CurrentDensity
        {
            get { return _currentDensity; }
            set { _currentDensity = value;
            base.RaisePropertyChanged("CurrentDensity");
            _gruntDensitiesComboView.Refresh();}
        }

        public Density CurrentGruntMixDensity
        {
            get { return _currentGruntMixDensity; }
            set { _currentGruntMixDensity = value;
            base.RaisePropertyChanged("CurrentGruntMixDensity");}
        }

        public double Volume
        {
            get { return _volume; }
            set { _volume = value;
            base.RaisePropertyChanged("Volume");
            TotalRecalc();}
        }

        public double Mass
        {
            get { return _mass; }
            set{ _mass = value;
                 base.RaisePropertyChanged("Mass");}
        }
        
        public double VolHardener
        {
            get { return _volHardener; }
            set
            {_volHardener = value;
            base.RaisePropertyChanged("VolHardener");
            }
        }

        public double VolThinner
        {
            get { return _volThinner; }
            set { _volThinner = value;
            base.RaisePropertyChanged("VolThinner");}
        }

        public double MassHardener
        {
            get { return _massHardener; }
            set { _massHardener = value;
            base.RaisePropertyChanged("MassHardener");}
        }

        public double MassThinner
        {
            get { return _massThinner; }
            set { _massThinner = value;
            base.RaisePropertyChanged("MassThinner");}
        }

        public double LeftProportion
        {
            get { return _leftProportion; }
            set {
                    if (value < 0 || value > 5)
                    {
                        LeftProportion = 5;
                    }
                    else
                    {
                        _leftProportion = value;   
                    }
                    base.RaisePropertyChanged("LeftProportion");
                    RightProportion = 5 - value;}
        }

        public double RightProportion
        {
            get { return _rightProportion; }
            set { _rightProportion = value;
            base.RaisePropertyChanged("RightProportion");}
        }

        #region RadioButtons

        //private bool _leftWhite;
        //private bool _leftGray;
        //private bool _leftBlack;
        //private bool _rightWhite;
        //private bool _rightGray;
        //private bool _rightBlack;

        //public bool LeftWhite
        //{
        //    get { return _leftWhite; }
        //    set
        //    {
        //        _leftWhite = value;
        //        base.RaisePropertyChanged("LeftWhite");
        //    }
        //}

        //public bool LeftGray
        //{
        //    get { return _leftGray; }
        //    set
        //    {
        //        _leftGray = value;
        //        base.RaisePropertyChanged("LeftGray");
        //    }
        //}

        //public bool LeftBlack
        //{
        //    get { return _leftBlack; }
        //    set
        //    {
        //        _leftBlack = value;
        //        base.RaisePropertyChanged("LeftBlack");
        //    }
        //}

        //public bool RightWhite
        //{
        //    get { return _rightWhite; }
        //    set
        //    {
        //        _rightWhite = value;
        //        base.RaisePropertyChanged("RightWhite");
        //    }
        //}

        //public bool RightGray
        //{
        //    get { return _rightGray; }
        //    set
        //    {
        //        _rightGray = value;
        //        base.RaisePropertyChanged("RightGray");
        //    }
        //}

        //public bool RightBlack
        //{
        //    get { return _rightBlack; }
        //    set
        //    {
        //        _rightBlack = value;
        //        base.RaisePropertyChanged("RightBlack");
        //    }
        //}

        #endregion

        public ICollectionView DensitiesComboView
        {
            get { return _densitiesComboView; }
        }

        public ICollectionView GruntDensitiesComboView
        {
            get { return _gruntDensitiesComboView; }
        }

        public string LeftGruntMass
        {
            get { return leftGruntMass; }
            set { leftGruntMass = value;
            base.RaisePropertyChanged("LeftGruntMass");}
        }

        public string RightGruntMass
        {
            get { return rightGruntMass; }
            set { rightGruntMass = value;
            base.RaisePropertyChanged("RightGruntMass");}
        }

        #endregion

        #region Methods

        private void GetData()
        {
            //Добавление типов плотности для первого комбобокса с типами
            DenTypes = new ObservableCollection<DensityType>
                {
                    new DensityType {Id = (int) DensityTypes.Mobihel, Name = "Mobihel"},
                    new DensityType {Id = (int) DensityTypes.Grunts, Name = "Грунты"},
                    new DensityType {Id = (int) DensityTypes.Thinner, Name = "Разбавитель"},
                    new DensityType {Id = (int) DensityTypes.Hardener, Name = "Отвердитель"},
                    new DensityType {Id = (int) DensityTypes.Others, Name = "Прочее"}
                };
            
            //Достать все из таблицы плотностей чтобы забиндить на 2й комбобокс
            Densities = new ObservableCollection<Density>(colorItEntities.Density.ToList());

            GruntDensities = 
                new ObservableCollection<Density>(colorItEntities.Density.
                    Where(gr => gr.Type == (int)DensityTypes.Grunts).ToList());


            _densitiesComboView = CollectionViewSource.GetDefaultView(Densities);
            _densitiesComboView.Filter = DensityFilter;

            _gruntDensitiesComboView = CollectionViewSource.GetDefaultView(GruntDensities);
            _gruntDensitiesComboView.Filter = GruntDensityFilter;

            AddCommands();
        }

        private bool DensityFilter(object item)
        {
            if (_currentType == null)
                return true;

            Density density = item as Density;
            return density != null && density.Type == _currentType.Id;
        }

        //Жестокий фильтр для грунтов предназначеных для смешивания..
        private bool GruntDensityFilter(object item)
        {
            Density density = item as Density;

            if (CurrentDensity == null || density == null)
                return false;

            int[] FillerWet = { 34, 35, 36 };
            int[] FillerDry = { 51, 52, 53 };
            int[] FillerHB = { 37, 38, 39 };
            int[] compareWith = { CurrentDensity.ID };
            
            if (FillerWet.Contains(CurrentDensity.ID))
                return FillerWet.Except(compareWith).Contains(density.ID);
            if (FillerDry.Contains(CurrentDensity.ID))
                return FillerDry.Except(compareWith).Contains(density.ID);
            if (FillerHB.Contains(CurrentDensity.ID))
                return FillerHB.Except(compareWith).Contains(density.ID);

            return false;
        }

        //Главный метод который обновляет все поля для расчета
        private void TotalRecalc()
        {
            if(CurrentGruntMixDensity == null)
                PaintMassCounter();
            else
                GruntMassCounter();

            UpdateVolThinner();
            UpdateMassThinner();

            UpdateVolHardener();
            UpdateMassHardener();
        }

        //Высчитывает массу краски\прочего, не грунта
        private void PaintMassCounter()
        {
            Mass = CurrentDensity.DensityValue * Volume;
        }

        //Высчитывает массу грунта c пропорциями
        private void GruntMassCounter()
        {
            double onePart = Volume/5;
            double leftGrunt = onePart*LeftProportion*CurrentDensity.DensityValue;
            double rightGrunt = onePart*RightProportion*CurrentGruntMixDensity.DensityValue;
            Mass = leftGrunt + rightGrunt;

            LeftGruntMass = String.Format("Масса {0} = {1}", CurrentDensity.Name, leftGrunt);
            RightGruntMass = String.Format("Масса {0} = {1}", CurrentGruntMixDensity.Name, rightGrunt);
        }
        
        //Считает объем разбавителя
        private void UpdateVolThinner()
        {
            VolThinner = Volume*CurrentDensity.ProportionThinner;
        }

        //Считает массу разбавителя
        private void UpdateMassThinner()
        {
            double thinnerValue = 0;
            var firstOrDefault = colorItEntities.Density.FirstOrDefault(density => density.ID == CurrentDensity.AccordingThinner);
            if (firstOrDefault != null)
            {
                thinnerValue = firstOrDefault.DensityValue;
            }

            MassThinner = thinnerValue * VolThinner;
        }

        //Считает объем отвердителя
        private void UpdateVolHardener()
        {
            VolHardener = Volume * CurrentDensity.ProportionHardener;
        }

        //Считает массу отвердителя
        private void UpdateMassHardener()
        {
            double hardenerValue = 0;
            var firstOrDefault = colorItEntities.Density.FirstOrDefault(density => density.ID == CurrentDensity.AccordingHardener);
            if (firstOrDefault != null)
            {
                hardenerValue = firstOrDefault.DensityValue;
            }

            MassHardener = hardenerValue * VolHardener;
        }

        //Очищает все поля
        private void ClearAllFields()
        {
            CurrentGruntMixDensity = null;
            Volume = 0;
            LeftGruntMass = String.Empty;
            RightGruntMass = String.Empty;
            LeftProportion = 0;
            RightProportion = 0;
        }

        #endregion

        #region Commands
        
        public RelayCommand CleanAllCommand
        {
            get;
            private set;
        }
        
        //Initialize commands
        private void AddCommands()
        {
            CleanAllCommand = new RelayCommand(ClearAllFields);
        }

        #endregion

        public enum DensityTypes
        {
            Mobihel = 0,
            Grunts = 1,
            Others = 2,
            Thinner = 3,
            Hardener = 4
        }
    }

 
}
