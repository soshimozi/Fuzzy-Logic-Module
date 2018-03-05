using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FuzzyLib.Decorator;
using FuzzyLib.Infrastructure;
using FuzzyLib.Interfaces;

namespace FuzzyLib.Object.Generic
{
    public class FuzzyObject<T>
    {
        private readonly FuzzyModule _fuzzyModule;
        public T WrappedObject { get; set; }

        protected readonly Dictionary<string, FuzzyVariableReference> VariableReferences = new Dictionary<string, FuzzyVariableReference>();

        protected readonly Dictionary<string, FuzzyTermProxy> FuzzySets = new Dictionary<string, FuzzyTermProxy>();

        public FuzzyObject(T obj, FuzzyModule module)
        {
            WrappedObject = obj;
            _fuzzyModule = module;
        }

        public FuzzyObject(FuzzyModule module)
        {
            _fuzzyModule = module;
        }

        public void AddRule(IFuzzyTerm antecedent, IFuzzyTerm consequence)
        {
            Module.AddRule(antecedent, consequence);
        }

        public void AddRule<TAntecendent, TConsequence>(FuzzyTermDecorator<TAntecendent> antecedent, FuzzyTermDecorator<TConsequence> consequence) where TAntecendent : IFuzzyTerm where TConsequence : IFuzzyTerm
        {
            Module.AddRule(antecedent.Wrapped, consequence.Wrapped);
        }

        public FuzzyTermDecorator<FuzzyTermProxy> If(string name)
        {
            return FuzzySets.ContainsKey(name) ? new FuzzyTermDecorator<FuzzyTermProxy>(FuzzySets[name]) : null;
        }

        public FuzzyModule Module
        {
            get
            {
                return _fuzzyModule;
            }
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

        public void DefineFuzzyTerm<TProp>(string name, Expression<Func<T, TProp>> expr, IFuzzySetManifold set)
        {
            var pi = expr.GetPropertyInfo();

            var variable = DefineVariable(expr);
            if (variable == null) return;
            if (FuzzySets.ContainsKey(name)) return;

            FuzzySets.Add(name, variable.AddFuzzyTerm(name, set));
        }

        public void DefineFuzzyTermByName(string setName, string variableName, IFuzzySetManifold set)
        {
            if (VariableReferences.ContainsKey(variableName))
            {
                VariableReferences[variableName].Variable.AddFuzzyTerm(setName, set);
            }
        }

        public FuzzyTermProxy this[string name]
        {
            get { return GetFuzzyTerm(name); }
        }

        protected FuzzyTermProxy GetFuzzyTerm(string name)
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

        private FuzzyVariable DefineVariable<TProp>(Expression<Func<T, TProp>> func)
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
            return new DynamicWrapper<FuzzyTermProxy>(FuzzySets);
        }
    }
}