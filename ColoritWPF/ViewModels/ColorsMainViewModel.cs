using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using ColoritWPF.Models;

namespace ColoritWPF.ViewModels
{
    internal class ColorsMainViewModel : INotifyPropertyChanged
    {
        public ColorsMainViewModel()
        {
            colorItEntities = new ColorITEntities();
            GetData();
        }

        private ColorITEntities colorItEntities;
        private List<CarModels> _carModels;
        private List<Paints> _paints;
        private Paints _currentPaint;
        private PaintName _currentPaintName;

        public ObservableCollection<Client> Clients { get; set; }
        public ObservableCollection<PaintName> PaintNameList { get; set; }
        
        public Paints CurrentPaint
        {
            get { return _currentPaint; }
            set
            {
                if (_currentPaint == value) return;                
                _currentPaint = value;
                GetCurrentPaintName();
                OnPropertyChanged("CurrentPaint");
            }
        }

        public List<CarModels> CarModels
        {
            get { return _carModels; }
            set { _carModels = value; }
        }
        
        public List<Paints> Paints
        {
            get { return _paints; }
        }

        public PaintName CurrentPaintName
        {
            get { return _currentPaintName; }
            set
            {
                if (_currentPaintName == value) return;
                _currentPaintName = value;
                OnPropertyChanged("CurrentPaintName");
            }
        }


        private void GetData()
        {
            _paints = colorItEntities.Paints.ToList();

            Clients = new ObservableCollection<Client>(colorItEntities.Client.ToList());
            PaintNameList = new ObservableCollection<PaintName>(colorItEntities.PaintName.ToList());
            _carModels = colorItEntities.CarModels.ToList();
        }

        //Get current paint name by paintnameID from grid
        private void GetCurrentPaintName()
        {
            var _paintName = colorItEntities.PaintName.FirstOrDefault(item => item.ID == _currentPaint.NameID);
            CurrentPaintName = _paintName;
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
