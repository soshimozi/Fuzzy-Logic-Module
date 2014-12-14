using System.ComponentModel;

namespace FuzzyLib
{
    public class ObservableFuzzyObject<T> : FuzzyObject<T> where T : INotifyPropertyChanged
    {
        public ObservableFuzzyObject(T obj) : base(obj)
        {
            (obj as INotifyPropertyChanged).PropertyChanged += (sender, args) =>
            {
                var name = args.PropertyName;

                if (!VariableReferences.ContainsKey(name)) return;

                var pi = VariableReferences[name].PropertyInfo;
                Module.Fuzzify(pi.Name, DoubleSafeCaster.Convert(pi.GetValue(WrappedObject, null)));
            };

        }
    }
}