using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuzzyLib.Variables;

namespace FuzzyLib.Sets
{
    public class GradientSet : FuzzySet
    {
        public GradientSet(double min, double peak, double max)
            : base(peak)
        {
        }

        public override double GetMinBound()
        {
            throw new NotImplementedException();
        }

        public override double GetMaxBound()
        {
            throw new NotImplementedException();
        }

        public override double CalculateDegreeOfMembership(double value)
        {
            throw new NotImplementedException();
        }
    }
}
