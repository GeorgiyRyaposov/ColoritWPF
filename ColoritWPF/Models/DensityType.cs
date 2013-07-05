using System.ComponentModel;

namespace ColoritWPF.Models
{
    public class DensityType : INotifyPropertyChanged
    {
        private int _id;
        private string _name;

        public int Id
        {
            get { return _id; }
            set { _id = value; 
            OnPropertyChanged("Id");}
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; 
            OnPropertyChanged("Name");}
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event 
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
