using System;
using System.ComponentModel;
using Observables;

namespace TestFuzzyLib
{
    public class Enemy : MarshalByRefObject, INotifyPropertyChanged
    {
        public double Health { get; set; }

        public int Age { get; set; }
        
        public double Hunger { get; set; }

        [Observable]
        public double DistanceToTarget { get; set; }

        [Observable]
        public int AmmoStatus { get; set; }

        [Observable]
        public double Skill { get; set; }

        public double Desireability { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}