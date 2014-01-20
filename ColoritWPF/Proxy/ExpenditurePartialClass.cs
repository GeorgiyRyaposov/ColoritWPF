namespace ColoritWPF
{
    partial class Expenditure
    {
        private int _typeId;

        public int TypeId
        {
            get { return _typeId; }
            set
            {
                _typeId = value;
                OnPropertyChanged("TypeId");
            }
        }

        private string _other;
        public string Other
        {
            get { return _other; }
            set
            {
                _other = value;
                OnPropertyChanged("Other");
            }
        }   
    }
}
