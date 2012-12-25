using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ColoritWPF.Models
{
    public class ClientsListModel : INotifyPropertyChanged
    {
        public ClientsListModel()
        {
            using (ColorITEntities _colorItEntities = new ColorITEntities())
            {
                _currentClient = _colorItEntities.Client.SingleOrDefault(client => client.ID == 7);
            }            
        }
        public ObservableCollection<Client> AllClients { get; set; }
        private Client _currentClient;

        public Client CurrentClient
        {
            get { return _currentClient; }
            set
            {
                _currentClient = value;
                OnPropertyChanged("CurrentClient");
            }
        }

        #region INotifyPropertyChanged members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
