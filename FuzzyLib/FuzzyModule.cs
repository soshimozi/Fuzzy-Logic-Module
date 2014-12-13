using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace FuzzyLib
{
    public class FuzzyModule
    {
        //when calculating the centroid of the fuzzy manifold this value is used
        //to determine how many cross-sections should be sampled
        private const int NumSamples = 15;

        private Dictionary<string, FuzzyVariable> m_Variables
            = new Dictionary<string, FuzzyVariable>();

        private List<FuzzyRule> m_Rules
            = new List<FuzzyRule>();

        //zeros the DOMs of the consequents of each rule. Used by Defuzzify()
        private void SetConfidencesOfConsequentsToZero()
        {
            foreach (FuzzyRule rule in m_Rules)
            {
                rule.SetConfidenceOfConsequentToZero();
            }
        }

        //creates a new 'empty' fuzzy variable and returns a reference to it.
        public FuzzyVariable CreateFLV(string varName)
        {
            m_Variables.Add(varName, new FuzzyVariable());
            return m_Variables[varName];
        }

        //adds a rule to the module
        public void AddRule(FuzzyTerm antecedent, FuzzyTerm consequence)
        {
            m_Rules.Add(new FuzzyRule(antecedent, consequence));
        }

        //this method calls the Fuzzify method of the named FLV 
        public void Fuzzify(string nameOfFLV, double value)
        {
            m_Variables[nameOfFLV].Fuzzify(value);
        }

        //given a fuzzy variable and a deffuzification method this returns a 
        //crisp value
        //public double DeFuzzify(string key, 
        //                          DefuzzifyMethod method = DefuzzifyMethod.MAX_AV)
        //{
        //    //clear the DOMs of all the consequents of all the rules
        //    SetConfidencesOfConsequentsToZero();

        //    //process the rules
        //    foreach (FuzzyRule curRule in m_Rules)
        //    {
        //        curRule.Calculate();
        //    }

        //    //now defuzzify the resultant conclusion using the specified method
        //    switch (method)
        //    {
        //        case DefuzzifyMethod.CENTROID:
        //            return m_Variables[key].DeFuzzifyCentroid(NumSamples);

        //        case DefuzzifyMethod.MAX_AV:
        //            return m_Variables[key].DeFuzzifyMaxAv();
        //    }

        //    return 0;
        //}

        public double DeFuzzify(string key, Expression<Func<FuzzyVariable, double>> method)
        {
            //clear the DOMs of all the consequents of all the rules
            SetConfidencesOfConsequentsToZero();

            //process the rules
            foreach (FuzzyRule curRule in m_Rules)
            {
                curRule.Calculate();
            }

            var gen = method.Compile();
            return gen.Invoke(m_Variables[key]);
        }
    }




//you must pass one of these values to the defuzzify method. This module
    //only supports the MaxAv and centroid methods.
    //public enum DefuzzifyMethod 
    //{ 
    //    MAX_AV, 
    //    CENTROID
    //}
}
