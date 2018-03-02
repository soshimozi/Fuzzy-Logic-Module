using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FuzzyLib.Decorator;
using FuzzyLib.Infrastructure;

namespace FuzzyLib.Object.Generic
{
    public class FuzzyObject<T>
    {
        protected readonly FuzzyModule Module;
        protected readonly T WrappedObject;

        protected readonly Dictionary<string, FuzzyVariableReference> VariableReferences = new Dictionary<string, FuzzyVariableReference>();

        protected readonly Dictionary<string, FuzzySetTermProxy> FuzzySets = new Dictionary<string, FuzzySetTermProxy>();

        public FuzzyObject(T obj, FuzzyModule module)
        {
            WrappedObject = obj;
            Module = module;
        }

        public FuzzyObject<T> AddRule(FuzzyTerm antecedent, FuzzyTerm consequence)
        {
            Module.AddRule(antecedent, consequence);
            return this;
        }

        public FuzzyObject<T> AddRule<TAntecendent, TConsequence>(FuzzyTermDecorator<TAntecendent> antecedent, FuzzyTermDecorator<TConsequence> consequence) where TAntecendent : FuzzyTerm where TConsequence : FuzzyTerm
        {
            Module.AddRule(antecedent.Wrapped, consequence.Wrapped);
            return this;
        }

        public FuzzyTermDecorator<FuzzySetTermProxy> WrapSet(string name)
        {
            return FuzzySets.ContainsKey(name) ? new FuzzyTermDecorator<FuzzySetTermProxy>(FuzzySets[name]) : null;
        }


        //public FuzzyObject<T> AddFuzzySet<TProp, TFuzzy>(string name, Expression<Func<T, TProp>> expr, Func<double, double, double, TFuzzy> setfunc, int min, int peak, int max) where TFuzzy : IFuzzySet
        //{
        //    var pi = expr.GetPropertyInfo();
        //    if (VariableReferences.ContainsKey(pi.Name) && !FuzzySets.ContainsKey(name))
        //    {
        //        FuzzySets.Add(name, VariableReferences[pi.Name].Variable.AddFuzzySet(name, setfunc.Invoke(min, peak, max)));

        //    }

        //    return this;
        //}

        public FuzzyObject<T> AddFuzzySet<TProp>(string name, Expression<Func<T, TProp>> expr, IFuzzySet set)
        {
            var pi = expr.GetPropertyInfo();
            if (VariableReferences.ContainsKey(pi.Name) && !FuzzySets.ContainsKey(name))
            {
                FuzzySets.Add(name, VariableReferences[pi.Name].Variable.AddFuzzySet(name, set));
            }

            return this;
        }

        public void AddFuzzySetByName(string setName, string variableName, IFuzzySet set)
        {
            if (VariableReferences.ContainsKey(variableName))
            {
                VariableReferences[variableName].Variable.AddFuzzySet(setName, set);
            }
        }

        public FuzzySetTermProxy this[string name]
        {
            get { return FuzzyTerm(name); }
        }

        public FuzzySetTermProxy FuzzyTerm(string name)
        {
            return FuzzySets.ContainsKey(name) ? FuzzySets[name] : null;
        }

        /// <summary>
        /// Defines a variable for the property specified by the <paramref name="name"/> parameter
        /// </summary>
        /// <param name="name"></param>
        /// <returns>FuzzyVariable</returns>
        public FuzzyVariable DefineVariable(string name)
        {
            // defines a variable for the property named in 'name'

            var type = typeof (T);
            var prop = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).FirstOrDefault(p => p.Name == name);
            if(prop == null) throw new ArgumentException("Property does not exist.", "name");

            return VariableReferences.ContainsKey(name) ? VariableReferences[name].Variable : AddVariable(name, prop);
        }

        public FuzzyVariable DefineVariable<TProp>(Expression<Func<T, TProp>> func)
        {
            var propertyInfo = func.GetPropertyInfo();
            var name = propertyInfo.Name;

            return VariableReferences.ContainsKey(name) ? VariableReferences[name].Variable : AddVariable(name, propertyInfo);
        }

        private FuzzyVariable AddVariable(string name, PropertyInfo prop)
        {
            var variable = Module.CreateFLV(name);
            VariableReferences.Add(name, new FuzzyVariableReference {PropertyInfo = prop, Variable = variable});
            return variable;
        }

        public void Fuzzify<TProp>(Expression<Func<T, TProp>> func)
        {
            var pi = func.GetPropertyInfo();

            if (VariableReferences.ContainsKey(pi.Name))
            {
                double value;
                if (double.TryParse(pi.GetValue(WrappedObject, null).ToString(), out value))
                {
                    Module.Fuzzify(pi.Name, value);
                }
                
            }
        }

        public void Fuzzify<TProp>(Expression<Func<T, TProp>> func, TProp value)
        {
            var pi = func.GetPropertyInfo();

            if (VariableReferences.ContainsKey(pi.Name))
            {
                pi.SetValue(WrappedObject, value, null);
            }
        }

        public void DeFuzzify<TProp>(Expression<Func<T, TProp>> func, Expression<Func<FuzzyVariable, double>> method)
        {
            var pi = func.GetPropertyInfo();

            var defuzzy = Module.DeFuzzify(pi.Name, method);
            pi.SetValue(WrappedObject, defuzzy, null);
        }

        public dynamic GetDynamic()
        {
            return new DynamicWrapper<FuzzySetTermProxy>(FuzzySets);
        }

        #region Compile Overrides
        public FuzzyObject<T> Compile<T1>(Expression<Func<T, T1>> func)
        {
            // fuzzfy
            Fuzzify(func);

            return this;
        }

        public FuzzyObject<T> Compile<T1, T2>(Expression<Func<T, T1>> func, Expression<Func<T, T2>> func2)
        {
            // fuzzfy
            Fuzzify(func);
            Fuzzify(func2);

            return this;
        }

        public FuzzyObject<T> Compile<T1, T2, T3>(Expression<Func<T, T1>> func, Expression<Func<T, T2>> func2, Expression<Func<T, T3>> func3)
        {
            // fuzzfy
            Fuzzify(func);
            Fuzzify(func2);
            Fuzzify(func3);

            return this;
        }

        public FuzzyObject<T> Compile<T1, T2, T3, T4>(Expression<Func<T, T1>> func, Expression<Func<T, T2>> func2, Expression<Func<T, T3>> func3, Expression<Func<T, T4>> func4)
        {
            // fuzzfy
            Fuzzify(func);
            Fuzzify(func2);
            Fuzzify(func3);
            Fuzzify(func4);

            return this;
        }

        public FuzzyObject<T> Compile<T1, T2, T3, T4, T5>(Expression<Func<T, T1>> func, Expression<Func<T, T2>> func2, Expression<Func<T, T3>> func3, Expression<Func<T, T4>> func4, Expression<Func<T, T5>> func5)
        {
            // fuzzfy
            Fuzzify(func);
            Fuzzify(func2);
            Fuzzify(func3);
            Fuzzify(func4);
            Fuzzify(func5);

            return this;
        }

        public FuzzyObject<T> Compile<T1, T2, T3, T4, T5, T6>(Expression<Func<T, T1>> func, Expression<Func<T, T2>> func2, Expression<Func<T, T3>> func3, Expression<Func<T, T4>> func4, Expression<Func<T, T5>> func5, Expression<Func<T, T6>> func6)
        {
            // fuzzfy
            Fuzzify(func);
            Fuzzify(func2);
            Fuzzify(func3);
            Fuzzify(func4);
            Fuzzify(func5);
            Fuzzify(func6);

            return this;
        }

        public FuzzyObject<T> Compile<T1, T2, T3, T4, T5, T6, T7>(Expression<Func<T, T1>> func, Expression<Func<T, T2>> func2, Expression<Func<T, T3>> func3, Expression<Func<T, T4>> func4, Expression<Func<T, T5>> func5, Expression<Func<T, T6>> func6, Expression<Func<T, T7>> func7)
        {
            // fuzzfy
            Fuzzify(func);
            Fuzzify(func2);
            Fuzzify(func3);
            Fuzzify(func4);
            Fuzzify(func5);
            Fuzzify(func6);
            Fuzzify(func7);

            return this;
        }

        public FuzzyObject<T> Compile<T1, T2, T3, T4, T5, T6, T7, T8>(Expression<Func<T, T1>> func, Expression<Func<T, T2>> func2, Expression<Func<T, T3>> func3, Expression<Func<T, T4>> func4, Expression<Func<T, T5>> func5, Expression<Func<T, T6>> func6, Expression<Func<T, T7>> func7, Expression<Func<T, T8>> func8)
        {
            // fuzzfy
            Fuzzify(func);
            Fuzzify(func2);
            Fuzzify(func3);
            Fuzzify(func4);
            Fuzzify(func5);
            Fuzzify(func6);
            Fuzzify(func7);
            Fuzzify(func8);

            return this;
        }

        public FuzzyObject<T> Compile<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Expression<Func<T, T1>> func, Expression<Func<T, T2>> func2, Expression<Func<T, T3>> func3, Expression<Func<T, T4>> func4, Expression<Func<T, T5>> func5, Expression<Func<T, T6>> func6, Expression<Func<T, T7>> func7, Expression<Func<T, T8>> func8, Expression<Func<T, T9>> func9)
        {
            // fuzzfy
            Fuzzify(func);
            Fuzzify(func2);
            Fuzzify(func3);
            Fuzzify(func4);
            Fuzzify(func5);
            Fuzzify(func6);
            Fuzzify(func7);
            Fuzzify(func8);
            Fuzzify(func9);

            return this;
        }
        #endregion

    }
}