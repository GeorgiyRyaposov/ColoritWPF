using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace ColoritWPF.ViewModel
{
    public class PaintEditorViewModel : ViewModelBase
    {
        public PaintEditorViewModel()
        {
            colorItEntities = new ColorITEntities();
            PaintsList = new ObservableCollection<PaintName>(colorItEntities.PaintName.ToList());
            _currentPaint = new PaintName();
            SaveChangesCommand = new RelayCommand(SaveChangesCmd);
            PaintsView = CollectionViewSource.GetDefaultView(PaintsList);
            PaintsView.Filter = Filter;
        }

        private void SaveChangesCmd()
        {
            try
            {
                colorItEntities.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось сохранить измненения\n"+ex.Message + ex.InnerException);
            }
        }

        public RelayCommand SaveChangesCommand
        {
            get;
            private set;
        }

        private bool Filter(object o)
        {
            PaintName paint = o as PaintName;
            return paint.Name.Contains(PaintNameFilter);
        }

        private ColorITEntities colorItEntities;

        public ObservableCollection<PaintName> PaintsList { get; set; }
        public ICollectionView PaintsView { get; private set; }

        private PaintName _currentPaint;
        public PaintName CurrentPaint
        {
            get { return _currentPaint; }
            set
            {
                _currentPaint = value;
                base.RaisePropertyChanged("CurrentPaint");
            }
        }

        private string _paintNameFilter = string.Empty;
        public string PaintNameFilter
        {
            get { return _paintNameFilter; }
            set
            {
                _paintNameFilter = value;
                base.RaisePropertyChanged("PaintNameFilter");
                PaintsView.Refresh();
            }
        }
    }
}
