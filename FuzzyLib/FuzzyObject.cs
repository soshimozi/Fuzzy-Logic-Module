using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace FuzzyLib
{
    public class FuzzyObject<T> //: DynamicObject
    {
        private readonly FuzzyModule _module;
        private readonly T _wrappedObject;

        private readonly Dictionary<string, FuzzyVariableReference> _variableReferences = new Dictionary<string, FuzzyVariableReference>();
        private readonly Dictionary<string, FuzzySetProxy> _fuzySets = new Dictionary<string, FuzzySetProxy>(); 

        public FuzzyObject(T obj)
        {
            _wrappedObject = obj;
            _module = new FuzzyModule();
        }

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


        public FuzzyObject<T> AddRule(FuzzyTerm antecedent, FuzzyTerm consequence)
        {
            _module.AddRule(antecedent, consequence);
            return this;
        }

        public FuzzyObject<T> AddRule<TAntecendent, TConsequence>(FuzzyTermWrapper<TAntecendent> antecedent, FuzzyTermWrapper<TConsequence> consequence) where TAntecendent : FuzzyTerm where TConsequence : FuzzyTerm
        {
            _module.AddRule(antecedent.Wrapped, consequence.Wrapped);
            return this;
        }

        public FuzzyTermWrapper<FuzzySetProxy> WrapSet(FuzzySetProxy set)
        {
            return new FuzzyTermWrapper<FuzzySetProxy>(set);
        }

        public FuzzyTermWrapper<FuzzySetProxy> WrapSet(string name)
        {
            return _fuzySets.ContainsKey(name) ? new FuzzyTermWrapper<FuzzySetProxy>(_fuzySets[name]) : null;
        }

        public FuzzyTermWrapper<FuzzyOperatorAND> And(FuzzyTerm lhs, FuzzyTerm rhs)
        {
            return new FuzzyTermWrapper<FuzzyOperatorAND>(FuzzyOperator.And(lhs, rhs));
        }

        public FuzzyTermWrapper<FuzzyOperatorOR> Or(FuzzyTerm lhs, FuzzyTerm rhs)
        {
            return new FuzzyTermWrapper<FuzzyOperatorOR>(FuzzyOperator.Or(lhs, rhs));
        }

        //public FuzzyModule Module { get { return _module; } }



        //public override bool TryGetMember(GetMemberBinder binder, out object result)
        //{
        //    var name = binder.Name;

        //    FuzzySetProxy proxy;
        //    var retval = _fuzySets.TryGetValue(name, out proxy);

        //    result = proxy;

        //    return retval;
        //}

        //public override bool TrySetMember(
        //     SetMemberBinder binder, object value)
        //{
        //    return false;
        //}

        public FuzzyObject<T> AddFuzzySet<TProp, TFuzzy>(string name, Expression<Func<T, TProp>> expr, Func<double, double, double, TFuzzy> setfunc, int min, int peak, int max) where TFuzzy : FuzzySet
        {
            var pi = expr.GetPropertyInfo();
            if (_variableReferences.ContainsKey(pi.Name) && !_fuzySets.ContainsKey(name))
            {
                _fuzySets.Add(name, _variableReferences[pi.Name].Variable.AddFuzzySet(name, setfunc.Invoke(min, peak, max)));

            }
                
            return this;
        }

        public FuzzyTerm this[string name]
        {
            get { return FuzzyTerm(name); }
        }

        public FuzzyTerm FuzzyTerm(string name)
        {
            return _fuzySets.ContainsKey(name) ? (FuzzyTerm)_fuzySets[name] : null;
        }

        /// <summary>
        /// Defines a variable for the property specified by the <paramref name="name"/> parameter
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns>FuzzyVariable</returns>
        public FuzzyVariable DefineVariable(string name)
        {
            // defines a variable for the property named in 'name'

            var type = typeof (T);
            var prop = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).FirstOrDefault(p => p.Name == name);
            if(prop == null) throw new ArgumentException("Property does not exist.", "name");

            return _variableReferences.ContainsKey(name) ? _variableReferences[name].Variable : AddVariable(name, prop);
        }

        private FuzzyVariable AddVariable(string name, PropertyInfo prop)
        {
            var variable = _module.CreateFLV(name);
            _variableReferences.Add(name, new FuzzyVariableReference {PropertyInfo = prop, Variable = variable});
            return variable;
        }

        public void Fuzzify<TProp>(Expression<Func<T, TProp>> func)
        {
            var pi = func.GetPropertyInfo();

            if (_variableReferences.ContainsKey(pi.Name))
            {
                double value;
                if (double.TryParse(pi.GetValue(_wrappedObject, null).ToString(), out value))
                {
                    _module.Fuzzify(pi.Name, value);
                }
                
            }
        }

        public void DeFuzzify<TProp>(Expression<Func<T, TProp>> func, Expression<Func<FuzzyVariable, double>> method)
        {
            var pi = func.GetPropertyInfo();

            var defuzzy = _module.DeFuzzify(pi.Name, method);
            pi.SetValue(_wrappedObject, defuzzy, null);
            //var pi = func.GetPropertyInfo();
            //double? fuzzy = null;

            //try
            //{
            //    fuzzy = (double) pi.GetValue(_wrappedObject, null);
            //}
            //catch (Exception)
            //{
            //}

            //var propertyValue = (TProp)pi.GetValue(_wrappedObject, null);
        }

        public void Fuzzify<TProp>(Expression<Func<T, TProp>> func, TProp value)
        {
            var pi = func.GetPropertyInfo();

            if (_variableReferences.ContainsKey(pi.Name))
            {
                pi.SetValue(_wrappedObject, value, null);
            }
        }

        public FuzzyVariable DefineVariable<TProp>(Expression<Func<T, TProp>> func)
        {
            var propertyInfo = func.GetPropertyInfo();
            var name = propertyInfo.Name;

            return _variableReferences.ContainsKey(name) ? _variableReferences[name].Variable : AddVariable(name, propertyInfo);
        }

        public dynamic GetDynamic()
        {
            return new DynamicWrapper<FuzzySetProxy>(_fuzySets);
        }
    }
}