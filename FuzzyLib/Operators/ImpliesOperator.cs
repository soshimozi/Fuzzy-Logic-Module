using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuzzyLib.Variables;

namespace FuzzyLib.Operators
{
    public class ImpliesOperator : FuzzyTerm
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
