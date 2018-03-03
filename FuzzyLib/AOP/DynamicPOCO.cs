using System;
using System.ComponentModel;


namespace FuzzyLib.AOP
{
    public abstract class DynamicPOCO<T> : MarshalByRefObject, INotifyPropertyChanged where T : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}