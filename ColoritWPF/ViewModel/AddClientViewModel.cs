using System;
using ColoritWPF.Common;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace ColoritWPF.ViewModel
{
    public class AddClientViewMode : BaseVmWithBlls
    {
        public AddClientViewMode()
        {
            AddClientCommand = new RelayCommand(AddNewClientCmd, AddNewClientCmdCanExecute);
            _newClient = new Client
                                {
                                    Balance = 0,
                                    Discount = 0,
                                };
            
        }

        private Client _newClient;
        public Client NewClient
        {
            get { return _newClient; }
            set 
            { 
                _newClient = value;
                OnPropertyChanged("NewClient");
            }
        }

        public String Name
        {
            get { return NewClient.Name; }
            set 
            { 
                NewClient.Name = value;
                OnPropertyChanged("Name");
            }
        }

        public String Info
        {
            get { return NewClient.Info; }
            set
            {
                NewClient.Info = value;
                OnPropertyChanged("Info");
            }
        }

        public String Phone
        {
            get { return NewClient.PhoneNumber; }
            set
            {
                NewClient.PhoneNumber = value;
                OnPropertyChanged("Phone");
            }
        }

        public Decimal Balance
        {
            get { return NewClient.Balance; }
            set
            {
                NewClient.Balance = value;
                OnPropertyChanged("Balance");
            }
        }

        public double Discount
        {
            get { return NewClient.Discount; }
            set
            {
                NewClient.Discount= value;
                OnPropertyChanged("Discount");
            }
        }

        private void ClearFields()
        {
            NewClient = new Client();
            Name = String.Empty;
            Info = String.Empty;
            Phone = String.Empty;
            Balance = 0;
            Discount = 0;
        }

        #region Commands

        public RelayCommand AddClientCommand
        {
            get;
            private set;
        }

        private void AddNewClientCmd()
        {
            ClientsBll.SaveClient(NewClient);
            ClearFields();
            Messenger.Default.Send<Client>(NewClient);
        }

        private bool AddNewClientCmdCanExecute()
        {
            return !String.IsNullOrEmpty(Name);
        }

        #endregion
    }
}