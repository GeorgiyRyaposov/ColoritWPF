using System;
using System.Windows;
using ColoritWPF.Common;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;

namespace ColoritWPF.ViewModel
{
    public class ProducerEditorViewModel : BaseVmWithBlls
    {
        public ProducerEditorViewModel()
        {
            AddProducerCommand = new RelayCommand(AddNewProducer, AddProducerCanExecute);
            RemoveProducerCommand = new RelayCommand(RemoveNewProducer, RemoveProducerCanExecute);
            UpdateProducerCommand = new RelayCommand(UpdateNewProducer, RemoveProducerCanExecute);
            Producers = new ObservableCollection<Producers>(ProducerBll.GetProducers());
        }

        #region Properties

        private string _newProducerName;
        public string NewProducerName
        {
            get { return _newProducerName; }
            set
            {
                _newProducerName = value;
                OnPropertyChanged("NewProducerName");
            }
        }

        private Producers _currentProducer;
        public Producers CurrentProducer
        {
            get { return _currentProducer; }
            set
            {
                _currentProducer = value;
                OnPropertyChanged("CurrentProducer");
            }
        }

        private ObservableCollection<Producers> _producers;
        public ObservableCollection<Producers> Producers
        {
            get { return _producers; }
            set
            {
                _producers = value;
                OnPropertyChanged("Producers");
            }
        }

        public RelayCommand AddProducerCommand { get; private set; }
        public RelayCommand RemoveProducerCommand { get; private set; }
        public RelayCommand UpdateProducerCommand { get; private set; }

        #endregion

        #region Methods

        private void AddNewProducer()
        {
            ProducerBll.AddProducer(NewProducerName);
        }

        private bool AddProducerCanExecute()
        {
            return !String.IsNullOrEmpty(CurrentProducer.Name);
        }

        private void UpdateNewProducer()
        {
            SaveChanges();
        }

        private void RemoveNewProducer()
        {
            if (MessageBox.Show("Вы уверены что хотите удалить производителя?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                ProducerBll.RemoveProducer(CurrentProducer.Id);
            }
        }

        private bool RemoveProducerCanExecute()
        {
            return CurrentProducer != null;
        }

        #endregion
    }
}
