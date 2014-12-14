using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FuzzyLib.Variables;

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
        public void AddRule(FuzzyTerm antecedent, FuzzyTerm consequence)
        {
            _rules.Add(new FuzzyRule(antecedent, consequence));
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
    }

}
