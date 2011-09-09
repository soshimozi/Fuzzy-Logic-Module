using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuzzyLib
{
    public class FairlyFuzzyOperator : FuzzyTerm
    {
        private FuzzySet m_Set;

        public FairlyFuzzyOperator(FuzzySetProxy proxy)
        {
            m_Set = proxy.Set;
        }

        public override double DegreeOfMembership
        {
            get { return Math.Sqrt(m_Set.DegreeOfMembership); }
        }

        public override void ClearDegreOfMembership()
        {
            m_Set.Clear();
        }

        public override void MergeWithDOM(double value)
        {
            m_Set.MergeWithDegreeOfMovement(Math.Sqrt(value));
        }

        public override object Clone()
        {
            return new FairlyFuzzyOperator(FuzzySetProxy.CreateProxyForSet(m_Set));
        }
    }
}
