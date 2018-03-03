using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using FuzzyLibUI.Annotations;

namespace FuzzyLibUI
{
    public class FuzzyTermModel : INotifyPropertyChanged
    {
        private string _name;
        public string Name { get { return _name; } set { _name = value; OnPropertyChanged(); } }

        private FuzzyShapeModel _shape;
        public FuzzyShapeModel Shape { get { return _shape; } set { _shape = value; OnPropertyChanged(); }}

        private ObservableCollection<FuzzyTermParameterModel> _parameters;
        public ObservableCollection<FuzzyTermParameterModel> Parameters
        {
            get { return _parameters ?? (_parameters = new ObservableCollection<FuzzyTermParameterModel>()); }
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
