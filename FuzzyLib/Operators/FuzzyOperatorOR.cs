using System.Collections.Generic;
using System.Linq;

namespace FuzzyLib.Operators
{
    public class FuzzyOperatorOr : IFuzzyTerm
    {
        //an instance of this class may AND together up to 4 terms
        readonly List<IFuzzyTerm> _terms = new List<IFuzzyTerm>();

        public FuzzyOperatorOr(params IFuzzyTerm[] terms)
        {
            foreach (var term in terms)
            {
                _terms.Add(term.Clone() as IFuzzyTerm);
            }
        }

        public override double DegreeOfMembership
        {
            get
            {
                return
                    (from term in _terms
                        select term.DegreeOfMembership).Max();
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
            return new FuzzyOperatorOr(_terms.Select(term => term.Clone() as IFuzzyTerm).ToArray());
        }
    }
}
