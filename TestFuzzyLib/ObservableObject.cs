using System;
using System.ComponentModel;
using TestFuzzyLib.Annotations;

namespace TestFuzzyLib
{
    public abstract class ObservableObject<T> : MarshalByRefObject, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        public virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}