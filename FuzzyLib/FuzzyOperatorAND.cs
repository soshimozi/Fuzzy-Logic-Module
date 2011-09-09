using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuzzyLib
{
    public class FuzzyOperatorAND : FuzzyTerm
    {
        //an instance of this class may AND together up to 4 terms
        private List<FuzzyTerm> _terms = new List<FuzzyTerm>();

        public FuzzyOperatorAND(params FuzzyTerm[] terms)
        {
            foreach (FuzzyTerm term in terms)
            {
                _terms.Add(term.Clone() as FuzzyTerm);
            }
        }

        public override double DegreeOfMembership
        {
            get
            {
                return (System.Nullable<double>)
                    (from term in _terms
                     select term.DegreeOfMembership).Min() ?? double.MaxValue;
            }
        }

        public override void ClearDegreOfMembership()
        {
            foreach (FuzzyTerm term in _terms)
            {
                term.ClearDegreOfMembership();
            }
        }

        public override void MergeWithDOM(double value)
        {
            foreach (FuzzyTerm term in _terms)
            {
                term.MergeWithDOM(value);
            }
        }

        public override object Clone()
        {

            List<FuzzyTerm> terms = new List<FuzzyTerm>();
            foreach (FuzzyTerm term in _terms)
            {
                terms.Add(term.Clone() as FuzzyTerm);
            }

            return new FuzzyOperatorAND(terms.ToArray());
        }
    }
}
