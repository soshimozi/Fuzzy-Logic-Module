using System;
using System.ComponentModel;
using FuzzyLib.AOP;
using FuzzyLib.Infrastructure;
using FuzzyLib.Object.Generic;
using FuzzyLib.Observables;
using System.Linq;
using System.Reflection;

namespace FuzzyLib.Object
{
    public class ObservableFuzzyObject<T> : FuzzyObject<T> where T : MarshalByRefObject, INotifyPropertyChanged
    {
        public ObservableFuzzyObject(T proxy, FuzzyModule module) : base(proxy, module)
        {
            Proxy = proxy;

            proxy.PropertyChanged += (sender, args) =>
            {
                var name = args.PropertyName;

                if (!VariableReferences.ContainsKey(name)) return;

                var pi = VariableReferences[name].PropertyInfo;
                Module.Fuzzify(pi.Name, DoubleSafeCaster.Convert(pi.GetValue(WrappedObject, null)));
            };

            DefineVariables();
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

            DefineVariables();
        }

        public T Proxy
        {
            get;
            private set;
        }

        private void DefineVariables()
        {
            //var properties = typeof(T).GetMethods()
            //    //.Where(y => y.GetCustomAttributes(false).OfType<ObservableAttribute>().Any())
            //    .ToList();

            //foreach(var property in properties)
            //{
            //    DefineVariable(property.Name);
            //}

        }
    }
}