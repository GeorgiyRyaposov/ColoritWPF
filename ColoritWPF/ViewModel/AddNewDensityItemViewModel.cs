using System;
using System.Collections.ObjectModel;
using System.Linq;
using ColoritWPF.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace ColoritWPF.ViewModel
{
    public class AddNewDensityItemViewModel : ViewModelBase
    {
        public AddNewDensityItemViewModel()
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
                AddCommands();
                //Messenger.Default.Register<Client>(this, curClient => Clients.Add(curClient));
                //Messenger.Default.Register<CarModels>(this, carModel => CarModels.Add(carModel));
                //Messenger.Default.Register<Settings>(this, setting => settings = setting);
            }
        }

        #region Properties

        public ObservableCollection<DensityType> DenTypes { get; set; }
        public ObservableCollection<Density> Thinners { get; set; }
        public ObservableCollection<Density> Hardeners { get; set; }

        private ColorITEntities colorItEntities;
        private DensityType _currentType;
        private string _name;
        private double _densityValue;
        private double _proportionThinner;
        private double _proportionHardener;
        private Density _currentThinner;
        private Density _currentHardener;

        public DensityType CurrentType
        {
            get { return _currentType; }
            set
            {
                _currentType = value;
                base.RaisePropertyChanged("CurrentType");
            }
        }

        public string Name
        {
            get { return _name; }
            set 
            { 
                _name = value;
                base.RaisePropertyChanged("Name");
            }
        }

        public double DensityValue
        {
            get { return _densityValue; }
            set 
            { 
                _densityValue = value;
                base.RaisePropertyChanged("DensityValue");
            }
        }

        public Density CurrentThinner
        {
            get { return _currentThinner; }
            set
            {
                _currentThinner = value;
                base.RaisePropertyChanged("CurrentThinner");
            }
        }

        public Density CurrentHardener
        {
            get { return _currentHardener; }
            set
            {
                _currentHardener = value;
                base.RaisePropertyChanged("CurrentHardener");
            }
        }

        public double ProportionThinner
        {
            get { return _proportionThinner; }
            set
            {
                _proportionThinner = value;
                base.RaisePropertyChanged("ProportionThinner");
            }
        }

        public double ProportionHardener
        {
            get { return _proportionHardener; }
            set
            {
                _proportionHardener = value;
                base.RaisePropertyChanged("ProportionHardener");
            }
        }

        #endregion

        private void GetData()
        {
            //Добавление типов плотности для первого комбобокса с типами
            DenTypes = new ObservableCollection<DensityType>
                {
                    new DensityType {Id = (int) DensityViewModel.DensityTypes.Mobihel, Name = "Mobihel"},
                    new DensityType {Id = (int) DensityViewModel.DensityTypes.Grunts, Name = "Грунты"},
                    new DensityType {Id = (int) DensityViewModel.DensityTypes.Thinner, Name = "Разбавитель"},
                    new DensityType {Id = (int) DensityViewModel.DensityTypes.Hardener, Name = "Отвердитель"},
                    new DensityType {Id = (int) DensityViewModel.DensityTypes.Others, Name = "Прочее"}
                };

            Thinners = new ObservableCollection<Density>(colorItEntities.Density.Where(den => den.Type == (int)DensityViewModel.DensityTypes.Thinner).ToList());
            Hardeners = new ObservableCollection<Density>(colorItEntities.Density.Where(den => den.Type == (int)DensityViewModel.DensityTypes.Hardener).ToList());

            CurrentHardener = new Density{ ID = 0, Name = "Отсутствует" };
            CurrentThinner = new Density { ID = 0, Name = "Отсутствует" };
            
            Thinners.Add(CurrentThinner);
            Hardeners.Add(CurrentHardener);

            ProportionHardener = 0;
            ProportionThinner = 0;
        }

        #region Commands

        #region fields
        public RelayCommand SaveCommand
        {
            get;
            private set;
        }
        #endregion

        //Initialize commands
        private void AddCommands()
        {
            SaveCommand = new RelayCommand(SaveCmd, SaveChangesCanExecute);
        }

        private void SaveCmd()
        {
            Density newDensity = new Density
                {
                    Type = CurrentType.Id,
                    Name = Name,
                    DensityValue = DensityValue,
                    AccordingThinner = CurrentThinner.ID,
                    AccordingHardener = CurrentHardener.ID,
                    ProportionThinner = ProportionThinner,
                    ProportionHardener = ProportionHardener
                };
            
            try
            {
                colorItEntities.Density.AddObject(newDensity);
                colorItEntities.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Сохранить изменения не удалось\n" + ex.Message);
            }
        }

        private bool SaveChangesCanExecute()
        {
            return !String.IsNullOrEmpty(Name);
        }

        #endregion
    }
}
