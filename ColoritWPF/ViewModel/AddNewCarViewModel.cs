using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace ColoritWPF.ViewModel
{
    public class AddNewCarViewModel : ViewModelBase
    {
        public AddNewCarViewModel()
        {
            if (IsInDesignMode)
            {

            }
            else
            {
                colorItEntities = new ColorITEntities();
                _newCarModel = new CarModels();
                AddCarCommand = new RelayCommand(AddCarCmd, AddCarCmdCanExecute);
            }
        }

        private bool AddCarCmdCanExecute()
        {
            return !String.IsNullOrEmpty(Name);
        }

        private void AddCarCmd()
        {
            colorItEntities.CarModels.AddObject(NewCarModel);
            Messenger.Default.Send<CarModels>(NewCarModel);
            try
            {
                colorItEntities.SaveChanges();
                NewCarModel = new CarModels();
                Name = String.Empty;
            }
            catch (Exception ex)
            {
                throw new Exception("Не могу сохранить данные\n" + ex.Message);
            }
        }

        private ColorITEntities colorItEntities;

        private CarModels _newCarModel;

        public CarModels NewCarModel
        {
            get { return _newCarModel; }
            set
            {
                _newCarModel = value;
                base.RaisePropertyChanged("NewCarModel");
            }
        }

        public string Name
        {
            get { return NewCarModel.ModelName; }
            set
            { 
                NewCarModel.ModelName = value;
                base.RaisePropertyChanged("Name");
            }
        }

        public RelayCommand AddCarCommand
        {
            get;
            private set;
        }

    }
}
