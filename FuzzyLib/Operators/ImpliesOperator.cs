using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuzzyLib.Operators
{
    public class ImpliesOperator : IFuzzyTerm
    {
        public override double DegreeOfMembership
        {
            get { throw new NotImplementedException(); }
        }

        public override void ClearDegreeOfMembership()
        {
            throw new NotImplementedException();
        }

        public override void MergeWithDOM(double value)
        {
            throw new NotImplementedException();
        }

        public override object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
