using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuzzyLib
{
    public class FuzzySetProxy : FuzzyTerm
    {
        private FuzzySetProxy()
        {
        }

        private FuzzySet _set;
        private FuzzySetProxy(FuzzySet set)
        {
            _set = set;
        }

        public static FuzzySetProxy CreateProxyForSet(FuzzySet set)
        {
            return new FuzzySetProxy(set);
        }

        public FuzzySet Set { get { return _set; } }

        public override double DegreeOfMembership
        {
            get { return _set.DegreeOfMembership; }
        }

        public override void ClearDegreOfMembership()
        {
            _set.Clear();
        }

        public override void MergeWithDOM(double value)
        {
            _set.MergeWithDegreeOfMovement(value);
        }

        public override object Clone()
        {
            return new FuzzySetProxy(_set);
        }
    }
}
