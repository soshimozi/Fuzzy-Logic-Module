using System;

namespace FuzzyLib.Operators
{
    public class FairlyFuzzyOperator : FuzzyTerm
    {
        private readonly FuzzyTerm _term;

        public FairlyFuzzyOperator(FuzzyTerm term)
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
            return new FairlyFuzzyOperator(_term.Clone() as FuzzyTerm);
        }
    }
}
