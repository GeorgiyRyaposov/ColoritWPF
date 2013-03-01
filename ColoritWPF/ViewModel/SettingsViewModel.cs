using System;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace ColoritWPF.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {
        public SettingsViewModel()
        {
            colorItEntities = new ColorITEntities();
            Settings = colorItEntities.Settings.First();
            SaveCommand = new RelayCommand(SaveSettings);
        }

        private void SaveSettings()
        {
            Messenger.Default.Send<Settings>(Settings);

            try
            {
                colorItEntities.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось сохранить данные\n" + ex.Message);
            }
        }

        private ColorITEntities colorItEntities;
        private Settings _settings;

        public RelayCommand SaveCommand
        {
            get;
            private set;
        }

        public Settings Settings
        {
            get { return _settings; }
            set
            {
                _settings = value;
                base.RaisePropertyChanged("Settings");
            }
        }

        public decimal ByCode
        {
            get { return Settings.ByCodeCost; }
            set
            {
                Settings.ByCodeCost = value;
                base.RaisePropertyChanged("ByCode");
            }
        }

        public decimal Selection
        {
            get { return Settings.SelectionCost; }
            set
            {
                Settings.SelectionCost = value;
                base.RaisePropertyChanged("Selection");
            }
        }

        public decimal SelectionAndTL
        {
            get { return Settings.SelectionAndThreeLayers; }
            set
            {
                Settings.SelectionAndThreeLayers = value;
                base.RaisePropertyChanged("SelectionAndTL");
            }
        }
    }
}
