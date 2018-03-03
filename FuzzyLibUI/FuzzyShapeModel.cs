using System.ComponentModel;
using System.Runtime.CompilerServices;
using FuzzyLibUI.Annotations;

namespace FuzzyLibUI
{
    public class FuzzyShapeModel : INotifyPropertyChanged
    {
        private string _name;
        public string Name { get { return _name; } set { _name = value; OnPropertyChanged(); }}

        private string _type;
        public string Type { get { return _type; } set { _type = value; OnPropertyChanged(); } }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}