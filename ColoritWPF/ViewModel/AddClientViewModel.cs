using System;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace ColoritWPF.ViewModel
{
    public class AddClientViewMode : ViewModelBase
    {
        public AddClientViewMode()
        {
            if (IsInDesignMode)
            {
                
            }
            else
            {
                colorItEntities = new ColorITEntities();
                ClientGroup = new ObservableCollection<ClientGroups>(colorItEntities.ClientGroups.ToList());
                AddClientCommand = new RelayCommand(AddNewClientCmd, AddNewClientCmdCanExecute);
                _newClient = new Client
                                 {
                                     Balance = 0,
                                     Discount = 0,
                                     GroupID = 1
                                 };
            }
        }



        private ColorITEntities colorItEntities;

        public ObservableCollection<ClientGroups> ClientGroup { get; set; }

        private Client _newClient;
        public Client NewClient
        {
            get { return _newClient; }
            set 
            { 
                _newClient = value;
                base.RaisePropertyChanged("NewClient");
            }
        }

        public String Name
        {
            get { return NewClient.Name; }
            set 
            { 
                NewClient.Name = value;
                base.RaisePropertyChanged("Name");
            }
        }

        public String Info
        {
            get { return NewClient.Info; }
            set
            {
                NewClient.Info = value;
                base.RaisePropertyChanged("Info");
            }
        }

        public String Phone
        {
            get { return NewClient.PhoneNumber; }
            set
            {
                NewClient.PhoneNumber = value;
                base.RaisePropertyChanged("Phone");
            }
        }

        public Decimal Balance
        {
            get { return NewClient.Balance; }
            set
            {
                NewClient.Balance = value;
                base.RaisePropertyChanged("Balance");
            }
        }

        public double Discount
        {
            get { return NewClient.Discount; }
            set
            {
                NewClient.Discount= value;
                base.RaisePropertyChanged("Discount");
            }
        }

        public int? GroupID
        {
            get { return NewClient.GroupID; }
            set
            {
                NewClient.GroupID = value;
                base.RaisePropertyChanged("GroupID");
            }
        }

        #region Commands

        public RelayCommand AddClientCommand
        {
            get;
            private set;
        }

        private void AddNewClientCmd()
        {
            colorItEntities.Client.AddObject(NewClient);
            Messenger.Default.Send<Client>(NewClient);
            try
            {
                colorItEntities.SaveChanges();
                NewClient = new Client();
                Name = String.Empty;
                Info = String.Empty;
                Phone = String.Empty;
                Balance = 0;
                Discount = 0;
                GroupID = 1;
            }
            catch (Exception ex)
            {
                throw new Exception("Не могу сохранить данные\n" + ex.Message);
            }
        }

        private bool AddNewClientCmdCanExecute()
        {
            return !String.IsNullOrEmpty(Name);
        }

        #endregion
    }
}