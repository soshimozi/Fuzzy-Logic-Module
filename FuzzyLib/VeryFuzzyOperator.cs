using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuzzyLib
{
    public class VeryFuzzyOperator : FuzzyTerm
    {
        private FuzzySet m_Set;

        public VeryFuzzyOperator(FuzzySetProxy proxy)
        {
            m_Set = proxy.Set;
        }

        public override double DegreeOfMembership
        {
            get { return m_Set.DegreeOfMembership * m_Set.DegreeOfMembership; }
        }

        public override void ClearDegreOfMembership()
        {
            m_Set.Clear();
        }

        public override void MergeWithDOM(double value)
        {
            m_Set.MergeWithDegreeOfMovement(value * value);
        }

        public override object Clone()
        {
            return new VeryFuzzyOperator(FuzzySetProxy.CreateProxyForSet(m_Set));
        }
    }
}
