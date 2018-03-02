using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using FuzzyLibUI.Annotations;

namespace FuzzyLibUI
{
    public class FuzzyVariableModel : INotifyPropertyChanged
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged(); }
        }

        private ObservableCollection<FuzzyTermModel> _terms;

        public ObservableCollection<FuzzyTermModel> Terms
        {
            get { return _terms ?? (_terms = new ObservableCollection<FuzzyTermModel>()); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}