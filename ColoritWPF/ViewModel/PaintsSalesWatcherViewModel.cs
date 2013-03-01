using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using GalaSoft.MvvmLight;

namespace ColoritWPF.ViewModel
{
    public class PaintsSalesWatcherViewModel : ViewModelBase
    {
        public PaintsSalesWatcherViewModel()
        {
            colorItEntities = new ColorITEntities();
            PaintsList = new ObservableCollection<Paints>(colorItEntities.Paints.ToList());
            PaintsListView = CollectionViewSource.GetDefaultView(PaintsList);
            PaintsListView.Filter = Filter;
        }

        private bool Filter(object obj)
        {
            Paints paints = obj as Paints;
            if (paints.Date >= FromDate &&
                paints.Date <= ToDate &&
                paints.Client.Name.Contains(ClientName) &&
                (paints.DocState == Confirmed))
                return true;
            return false;
        }

        private ColorITEntities colorItEntities;
        private DateTime _fromDate = DateTime.Now;
        private DateTime _toDate = DateTime.Now;
        private string _clientName = String.Empty;
        private bool _confirmed;

        public ObservableCollection<Paints> PaintsList { get; set; }
        public ICollectionView PaintsListView { get; private set; }

        public DateTime FromDate
        {
            get { return _fromDate; }
            set
            {
                _fromDate = value;
                base.RaisePropertyChanged("FromDate");
                PaintsListView.Refresh();
            }
        }

        public DateTime ToDate
        {
            get { return _toDate; }
            set
            {
                _toDate = value;
                base.RaisePropertyChanged("ToDate");
                PaintsListView.Refresh();
            }
        }

        public string ClientName
        {
            get { return _clientName; }
            set
            {
                _clientName = value;
                base.RaisePropertyChanged("ClientName");
                PaintsListView.Refresh();
            }
        }

        public bool Confirmed
        {
            get { return _confirmed; }
            set
            {
                _confirmed = value;
                base.RaisePropertyChanged("Confirmed");
                PaintsListView.Refresh();
            }
        }

    }
}
