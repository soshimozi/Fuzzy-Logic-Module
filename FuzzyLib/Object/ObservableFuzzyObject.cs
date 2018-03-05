using System;
using System.ComponentModel;
using FuzzyLib.Infrastructure;
using FuzzyLib.Object.Generic;
using FuzzyLib.Observables;

namespace FuzzyLib.Object
{
    public class ObservableFuzzyObject<T> : FuzzyObject<T> where T : MarshalByRefObject, INotifyPropertyChanged
    {
        public ObservableFuzzyObject(T target, FuzzyModule module) : base(target, module)
        {
            Proxy = ObservableDynamicProxy<T>.Marshal(target);

            Proxy.PropertyChanged += (sender, args) =>
            {
                var name = args.PropertyName;

                if (!VariableReferences.ContainsKey(name)) return;

                var pi = VariableReferences[name].PropertyInfo;
                Module.Fuzzify(pi.Name, DoubleSafeCaster.Convert(pi.GetValue(WrappedObject, null)));
            };
        }

        public ObservableFuzzyObject(FuzzyModule module, params object []  creationArgs) : base(module)
        {
            WrappedObject = Proxy = ObservableDynamicProxy<T>.Create(creationArgs); ;
            Proxy.PropertyChanged += (sender, args) =>
            {
                var name = args.PropertyName;

                if (!VariableReferences.ContainsKey(name)) return;

                var pi = VariableReferences[name].PropertyInfo;
                Module.Fuzzify(pi.Name, DoubleSafeCaster.Convert(pi.GetValue(WrappedObject, null)));
            };
        }

        public T Proxy
        {
            get;
            private set;
        }
    }
}