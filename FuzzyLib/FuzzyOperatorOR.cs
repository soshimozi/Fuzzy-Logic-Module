using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuzzyLib
{
    public class FuzzyOperatorOR : FuzzyTerm
    {
        //an instance of this class may AND together up to 4 terms
        List<FuzzyTerm> m_Terms = new List<FuzzyTerm>();

        public FuzzyOperatorOR(params FuzzyTerm[] terms)
        {
            // TODO: Complete member initialization
            foreach (FuzzyTerm term in terms)
            {
                m_Terms.Add(term.Clone() as FuzzyTerm);
            }
        }

        public override double DegreeOfMembership
        {
            get 
            {
                return (System.Nullable<double>)
                    (from term in m_Terms
                     select term.DegreeOfMembership).Max() ?? double.MinValue;
            }
        }

        public override void ClearDegreOfMembership()
        {
            throw new NotImplementedException();
        }

        public override void MergeWithDOM(double value)
        {
            throw new NotImplementedException();
        }

        public override object Clone()
        {
            return new FuzzyOperatorOR(m_Terms.ToArray());
        }
    }
}
