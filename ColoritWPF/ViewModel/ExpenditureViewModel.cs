using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ColoritWPF.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace ColoritWPF.ViewModel
{
    public class ExpenditureViewModel : ViewModelBase
    {
        public ExpenditureViewModel()
        {
            if (IsInDesignMode)
            {
                //SetDefaultValues();
            }
            else
            {
                colorItEntities = new ColorITEntities();
                
                GetData();
                InitializeCommands();
            }
        }

        #region Properties
        
        private ColorITEntities colorItEntities;
        public List<ExpenditureType> ExpenditureTypes { get; set; }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged(()=>Name);
            }
        }

        private ObservableCollection<Expenditure> _expenditures;
        public ObservableCollection<Expenditure> Expenditures
        {
            get { return _expenditures; }
            set
            {
                _expenditures = value;
                RaisePropertyChanged(() => Expenditures);
            }
        }

        private Expenditure _currentExpenditure;
        public Expenditure CurrentExpenditure
        {
            get { return _currentExpenditure; }
            set
            {
                _currentExpenditure = value;
                RaisePropertyChanged(() => CurrentExpenditure);
            }
        }

        private string _comments;
        public string Comments
        {
            get { return _comments; }
            set
            {
                _comments = value;
                RaisePropertyChanged(()=>Comments);
            }
        }

        private string _other;
        public string Other
        {
            get { return _other; }
            set
            {
                _other = value;
                RaisePropertyChanged(() => Other);
            }
        }

        private decimal _sum;
        public decimal Sum
        {
            get { return _sum; }
            set
            {
                _sum = value;
                RaisePropertyChanged(()=>Sum);
            }
        }

        private ExpenditureType _selectedExpenditureType;
        public ExpenditureType SelectedExpenditureType
        {
            get { return _selectedExpenditureType; }
            set
            {
                IsOtherEditable = value.Name == "Другое";

                _selectedExpenditureType = value;
                RaisePropertyChanged(()=>SelectedExpenditureType);
            }
        }

        private Expenditure _selectedExpenditure;
        public Expenditure SelectedExpenditure
        {
            get { return _selectedExpenditure; }
            set
            {
                _selectedExpenditure = value;
                RaisePropertyChanged(() => SelectedExpenditure);
            }
        }

        private bool _isOtherEditable = false;
        public bool IsOtherEditable
        {
            get { return _isOtherEditable; }
            set
            {
                _isOtherEditable = IsControlsEditable && value;
                RaisePropertyChanged(()=>IsOtherEditable);
            }
        }

        private bool _isControlsEditable;
        public bool IsControlsEditable
        {
            get { return _isControlsEditable; }
            set
            {
                _isControlsEditable = value;
                RaisePropertyChanged(() => IsControlsEditable);
            }
        }

        public string ConfirmedBtnName
        {
            get
            {
                if (CurrentExpenditure!= null)
                    return CurrentExpenditure.Confirmed ? "Разпровести" : "Провести";
                return "Загружаемся..";
            }
        }

        

        #endregion //Properties

        #region Methods

        private void GetData()
        {
            colorItEntities = new ColorITEntities();
            Expenditures = new ObservableCollection<Expenditure>(colorItEntities.Expenditure.ToList());
            InitExpenditureTypes();

            CurrentExpenditure = new Expenditure
                {
                    Amount = 0,
                    Confirmed = false,
                    Name = ExpenditureTypes.FirstOrDefault().Name
                };

            RaisePropertyChanged(()=> ConfirmedBtnName);
        }

        private void InitExpenditureTypes()
        {
            ExpenditureTypes = new List<ExpenditureType>
                {
                    new ExpenditureType("Аренда", (int)ExpenditureTypeEnum.Rent),
                    new ExpenditureType("Зарплата", (int)ExpenditureTypeEnum.Salary),
                    new ExpenditureType("Налоги", (int)ExpenditureTypeEnum.Tax),
                    new ExpenditureType("Хоз нужды", (int)ExpenditureTypeEnum.Expenses),
                    new ExpenditureType("Другое", (int)ExpenditureTypeEnum.Other)
                };
        }

        private string GetSelectedExpenditureName()
        {
            if (SelectedExpenditureType.Name != "Другое")
                return SelectedExpenditureType.Name;
            return Other;
        }

        #endregion //Methods

        #region Commands
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand ConfirmCommand { get; set; }

        private void InitializeCommands()
        {
            SaveCommand = new RelayCommand(Save);
            ConfirmCommand = new RelayCommand(ChangeDocumentState);
        }

        private void Save()
        {
            var itemToSave = new Expenditure
                {
                    Name = GetSelectedExpenditureName(),
                    Amount = Sum,
                    Comment = Comments,
                    Confirmed = false,
                    DateCreated = DateTime.Now
                };

            try
            {
                colorItEntities.Expenditure.AddObject(itemToSave);
                colorItEntities.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось сохранить запись в базу\n" + ex.Message);
            }
        }

        private void ChangeDocumentState()
        {
            var searchItem = colorItEntities.Expenditure.FirstOrDefault(item => item.Id == SelectedExpenditure.Id);
            if (searchItem == null)
            {
                Save();
            }
            if (SelectedExpenditure.Confirmed)
            {
                UnConfirm();
            }
            else
            {
                Confirm();
            }

            RaisePropertyChanged(() => ConfirmedBtnName);
        }

        private void Confirm()
        {
            var recordToConfirm = colorItEntities.Expenditure.FirstOrDefault(item => item.Id == SelectedExpenditure.Id);
            if (recordToConfirm == null)
            {
                throw new Exception("Не возможно провести документ: не удалось найти запись в базе");
            }

            recordToConfirm.Amount = Sum;
            recordToConfirm.Comment = Comments;
            recordToConfirm.Name = Name;
            recordToConfirm.Confirmed = true;
            
            var settings = colorItEntities.Settings.FirstOrDefault();
            if (settings != null)
            {
                settings.Cash = settings.Cash - Sum;
            }

            SaveChanges();
        }

        private void UnConfirm()
        {
            var recordToConfirm = colorItEntities.Expenditure.FirstOrDefault(item => item.Id == SelectedExpenditure.Id);
            if (recordToConfirm == null)
            {
                throw new Exception("Не возможно провести документ: не удалось найти запись в базе");
            }

            recordToConfirm.Confirmed = false;

            var settings = colorItEntities.Settings.FirstOrDefault();
            if (settings != null)
            {
                settings.Cash = settings.Cash + Sum;
            }

            SaveChanges();
        }

        private void SaveChanges()
        {
            try
            {
                colorItEntities.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось сохранить изменения\n" + ex.Message);
            }
        }
        #endregion //Commands
    }
}
