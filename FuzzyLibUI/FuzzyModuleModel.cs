using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using FuzzyLibUI.Annotations;

namespace FuzzyLibUI
{
    public class FuzzyModuleModel : INotifyPropertyChanged
    {
        private ObservableCollection<FuzzyVariableModel> _variables;

        private string _name;
        public string Name { get { return _name;  } set { _name = value; OnPropertyChanged(); } }

        private string _type;
        public string Type { get { return _type; } set { _type = value; OnPropertyChanged(); } }

        public ObservableCollection<FuzzyVariableModel> Variables
        {
            get { return _variables ?? (_variables = new ObservableCollection<FuzzyVariableModel>()); }
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
