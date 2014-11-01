using System;
using ColoritWPF.BLL;
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
                _clientsBll = new ClientsBll();
                
                AddClientCommand = new RelayCommand(AddNewClientCmd, AddNewClientCmdCanExecute);
                _newClient = new Client
                                 {
                                     Balance = 0,
                                     Discount = 0,
                                 };
            }
        }


        private ClientsBll _clientsBll;


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
            _clientsBll.SaveClient(NewClient);
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