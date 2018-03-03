using FuzzyLib.Interfaces;
using System;

namespace FuzzyLib.Operators
{
    public class FairlyFuzzyOperator : IFuzzyTerm
    {
        private readonly IFuzzyTerm _term;

        public FairlyFuzzyOperator(IFuzzyTerm term)
        {
            _term = term;
        }

        public override double DegreeOfMembership
        {
            get { return Math.Sqrt(_term.DegreeOfMembership); }
        }

        public override void ClearDegreeOfMembership()
        {
            _term.ClearDegreeOfMembership();
        }

        public override void MergeWithDOM(double value)
        {
            _term.MergeWithDOM(Math.Sqrt(value));
        }

        public override object Clone()
        {
            return new FairlyFuzzyOperator(_term.Clone() as IFuzzyTerm);
        }
    }
}
