using FuzzyLib.Interfaces;
using FuzzyLib.Sets;

namespace FuzzyLib
{
    public class FuzzyTermProxy : IFuzzyTerm
    {
        private readonly IFuzzySetManifold _set;
        private FuzzyTermProxy(IFuzzySetManifold set)
        {
            _set = set;
        }

        public static FuzzyTermProxy CreateProxyForSet(IFuzzySetManifold set)
        {
            return new FuzzyTermProxy(set);
        }

        public IFuzzySetManifold Set { get { return _set; } }

        public override double DegreeOfMembership
        {
            get { return _set.DegreeOfMembership; }
        }

        public override void ClearDegreeOfMembership()
        {
            _set.Clear();
        }

        public override void MergeWithDOM(double value)
        {
            _set.MergeWithDegreeOfMembership(value);
        }

        public override object Clone()
        {
            return new FuzzyTermProxy(_set);
        }
    }
}
