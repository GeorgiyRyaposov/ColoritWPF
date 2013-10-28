using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace ColoritWPF.ViewModel
{
    public class CashViewModel : ViewModelBase
    {
        public CashViewModel()
        {
            if (IsInDesignMode)
            {
                
            }
            else
            {
                colorItEntities = new ColorITEntities();
                Settings = colorItEntities.Settings.First();
                SaveCommand = new RelayCommand(SaveSettings);
                EnableCashCommand = new RelayCommand(EnableCash);
            }
        }



        private ColorITEntities colorItEntities;

        private Settings _settings;
        public Settings Settings
        {
            get { return _settings; }
            set
            {
                _settings = value;
                base.RaisePropertyChanged("Settings");
            }
        }

        private bool _isCashEnabled = false;
        public bool IsCashEnabled
        {
            get { return _isCashEnabled; }
            set
            {
                _isCashEnabled = value;
                base.RaisePropertyChanged("IsCashEnabled");
            }
        }

        public RelayCommand SaveCommand
        {
            get;
            private set;
        }

        public RelayCommand EnableCashCommand
        {
            get;
            private set;
        }

        private void SaveSettings()
        {
            try
            {
                colorItEntities.SaveChanges();
                IsCashEnabled = false;
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось сохранить данные\n" + ex.Message);
            }
        }

        private void EnableCash()
        {
            if (MessageBox.Show("Вы уверены что хотите изменить кол-во денег в кассе?",
                                "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                IsCashEnabled = !IsCashEnabled;
            }
        }
    }
}
