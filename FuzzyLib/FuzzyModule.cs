using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FuzzyLib.Decorator;
using FuzzyLib.Operators;

namespace FuzzyLib
{
    public class FuzzyModule
    {
        //when calculating the centroid of the fuzzy manifold this value is used
        //to determine how many cross-sections should be sampled
        public const int NumSamples = 15;

        private readonly Dictionary<string, FuzzyVariable> _variables
            = new Dictionary<string, FuzzyVariable>();

        private readonly List<FuzzyRule> _rules
            = new List<FuzzyRule>();

        //zeros the DOMs of the consequents of each rule. Used by Defuzzify()
        private void SetConfidencesOfConsequentsToZero()
        {
            foreach (var rule in _rules)
            {
                rule.SetConfidenceOfConsequentToZero();
            }
        }

        //creates a new 'empty' fuzzy variable and returns a reference to it.
        public FuzzyVariable CreateFLV(string varName)
        {
            _variables.Add(varName, new FuzzyVariable());
            return _variables[varName];
        }

        //adds a rule to the module
        public FuzzyRule AddRule(IFuzzyTerm antecedent, IFuzzyTerm consequence)
        {
            var rule = new FuzzyRule(antecedent, consequence);
            _rules.Add(rule);

            return rule;
        }

        //this method calls the Fuzzify method of the named FLV 
        public void Fuzzify(string nameOfFLV, double value)
        {
            _variables[nameOfFLV].Fuzzify(value);
        }

        public double DeFuzzify(string key, Expression<Func<FuzzyVariable, double>> method)
        {
            //clear the DOMs of all the consequents of all the rules
            SetConfidencesOfConsequentsToZero();

            //process the rules
            foreach (var curRule in _rules)
            {
                curRule.Calculate();
            }

            var gen = method.Compile();
            return gen.Invoke(_variables[key]);
        }

        public static FuzzyTermDecorator<FuzzyOperatorAnd> And(IFuzzyTerm lhs, IFuzzyTerm rhs)
        {
            return new FuzzyTermDecorator<FuzzyOperatorAnd>(FuzzyOperator.And(lhs, rhs));
        }

        public static FuzzyTermDecorator<FuzzyOperatorOr> Or(IFuzzyTerm lhs, IFuzzyTerm rhs)
        {
            return new FuzzyTermDecorator<FuzzyOperatorOr>(FuzzyOperator.Or(lhs, rhs));
        }

        public static FuzzyTermDecorator<FairlyFuzzyOperator> Fairly(IFuzzyTerm term)
        {
            return new FuzzyTermDecorator<FairlyFuzzyOperator>(FuzzyOperator.Fairly(term));
        }

        public static FuzzyTermDecorator<VeryFuzzyOperator> Very(IFuzzyTerm term)
        {
            return new FuzzyTermDecorator<VeryFuzzyOperator>(FuzzyOperator.Very(term));
        }

        public FuzzyVariable this[string name]
        {
            get { return _variables[name]; }
        }

        public IFuzzyTerm GetFuzzySet(string setName)
        {
            var parts = setName.Split(':');
            if (parts.Length == 2)
            {
                var variable = this[parts[0]];
                return variable.GetFuzzySet(parts[1]);
            }

            return null;
        }
    }

}
