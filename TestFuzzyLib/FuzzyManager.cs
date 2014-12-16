using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuzzyLib;
using FuzzyLib.Decorator;
using FuzzyLib.Object;
using FuzzyLib.Operators;
using FuzzyLib.Variables;

namespace TestFuzzyLib
{
    public class FuzzyManager
    {
        protected readonly FuzzyModule Module;

        protected readonly Dictionary<string, FuzzyVariable> VariableReferences = new Dictionary<string, FuzzyVariable>();

        protected readonly Dictionary<string, FuzzySetTerm> FuzzySets = new Dictionary<string, FuzzySetTerm>();

        public FuzzyManager(FuzzyModule module)
        {
            Module = module;
        }

        public FuzzyVariable DefineVariable(string name)
        {
            return VariableReferences.ContainsKey(name) ? VariableReferences[name] : AddVariable(name);
        }

        public FuzzySet AddFuzzySet(string name, string variable, Func<double, double, double, FuzzySet> setfunc, int min, int peak, int max)
        {
            var fullname = string.Format("{0}:{1}", variable, name);

            if (!VariableReferences.ContainsKey(variable) || FuzzySets.ContainsKey(fullname)) return null;

            var set = setfunc.Invoke(min, peak, max);
            FuzzySets.Add(fullname, VariableReferences[variable].AddFuzzySet(name, set));
            return set;
        }

        public FuzzyTerm GetFuzzySet(string name, string variable)
        {
            var fullname = string.Format("{0}:{1}", variable, name);
            return FuzzySets.ContainsKey(fullname) ? FuzzySets[fullname] : null;
        }

        public FuzzyRule AddRule(FuzzyTerm antecedent, FuzzyTerm consequence)
        {
            return Module.AddRule(antecedent, consequence);
        }


        public FuzzyTermDecorator<FuzzyOperatorAnd> And(FuzzyTerm lhs, FuzzyTerm rhs)
        {
            return new FuzzyTermDecorator<FuzzyOperatorAnd>(FuzzyOperator.And(lhs, rhs));
        }

        public FuzzyTermDecorator<FuzzyOperatorOr> Or(FuzzyTerm lhs, FuzzyTerm rhs)
        {
            return new FuzzyTermDecorator<FuzzyOperatorOr>(FuzzyOperator.Or(lhs, rhs));
        }

        public FuzzyTermDecorator<FairlyFuzzyOperator> Fairly(FuzzyTerm term)
        {
            return new FuzzyTermDecorator<FairlyFuzzyOperator>(FuzzyOperator.Fairly(term));
        }

        public FuzzyTermDecorator<VeryFuzzyOperator> Very(FuzzyTerm term)
        {
            return new FuzzyTermDecorator<VeryFuzzyOperator>(FuzzyOperator.Very(term));
        }


        private FuzzyVariable AddVariable(string name)
        {
            var variable = Module.CreateFLV(name);
            VariableReferences.Add(name, variable);

            return variable;
        }
    }
}
