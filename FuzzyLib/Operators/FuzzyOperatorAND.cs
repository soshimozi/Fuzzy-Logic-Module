using System.Collections.Generic;
using System.Linq;

namespace FuzzyLib.Operators
{
    public class FuzzyOperatorAnd : FuzzyTerm
    {
        //an instance of this class may AND together up to 4 terms
        private readonly List<FuzzyTerm> _terms = new List<FuzzyTerm>();

        public FuzzyOperatorAnd(params FuzzyTerm[] terms)
        {
            foreach (var term in terms)
            {
                _terms.Add(term.Clone() as FuzzyTerm);
            }
        }

        public override double DegreeOfMembership
        {
            get
            {
                return (from term in _terms
                    select term.DegreeOfMembership).Min();
            }
        }

        public override void ClearDegreeOfMembership()
        {
            foreach (var term in _terms)
            {
                term.ClearDegreeOfMembership();
            }
        }

        public override void MergeWithDOM(double value)
        {
            foreach (var term in _terms)
            {
                term.MergeWithDOM(value);
            }
        }

        public override object Clone()
        {
            return new FuzzyOperatorAnd(_terms.Select(term => term.Clone() as FuzzyTerm).ToArray());
        }
    }
}
