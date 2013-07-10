using ColoritWPF.Views;
using GalaSoft.MvvmLight.Command;

namespace ColoritWPF.ViewModel
{
    public class MenuItemsViewModel
    {
        public MenuItemsViewModel()
        {
            AddNewClientCommand = new RelayCommand(AddNewClientCmd);
            AddNewCarModelCommand = new RelayCommand(AddNewCarModelCmd);
            EditClientCommand = new RelayCommand(EditClientCmd);
            EditPaintsCommand = new RelayCommand(EditPaintsCmd);
            SettingsCommand = new RelayCommand(SettingsCmd);
            PaintsSalesWatcherCommand = new RelayCommand(PaintsSalesWatcherCmd);
            DensityCounterCommand = new RelayCommand(DensityCounterCmd);
            AddNewDensityCommand = new RelayCommand(AddNewDensityCmd);
        }
        
        #region fields

        public RelayCommand AddPaintCommand
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
    }
}
