using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace ColoritWPF.ViewModel
{
    public class ClientEditorViewModel : ViewModelBase
    {
        public ClientEditorViewModel()
        {
            colorItEntities = new ColorITEntities();
            ClientsList = new ObservableCollection<Client>(colorItEntities.Client.ToList());
            SaveCommand = new RelayCommand(SaveClient);
            _newClient = new Client();
            ClientsListView = CollectionViewSource.GetDefaultView(ClientsList);
            ClientsListView.Filter = Filter;
        }

        private bool Filter(object o)
        {
            Client client = o as Client;
            return client.Name.Contains(NameFilter);
        }

        private void SaveClient()
        {
            NewClient.Balance = NewClient.Balance + LoanDeposit;
            Messenger.Default.Send<Client>(NewClient);
            
            try
            {
                colorItEntities.SaveChanges();
                LoanDeposit = 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось сохранить данные\n" + ex.Message);
            }
        }

        private ColorITEntities colorItEntities;

        public ObservableCollection<Client> ClientsList { get; set; }
        public ICollectionView ClientsListView { get; private set; }

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

        private string _nameFilter = String.Empty;
        public string NameFilter
        {
            get { return _nameFilter; }
            set 
            { 
                _nameFilter = value;
                base.RaisePropertyChanged("NameFilter");
                ClientsListView.Refresh();
            }
        }

        private Decimal _loanDeposit;
        public Decimal LoanDeposit
        {
            get { return _loanDeposit; }
            set
            {
                _loanDeposit = value;
                base.RaisePropertyChanged("LoanDeposit");
            }
        }

        #region Commands

        public RelayCommand SaveCommand
        {
            get;
            private set;
        }



        #endregion
    }
}
