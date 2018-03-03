using FuzzyLib.Interfaces;
using FuzzyLib.Sets;

namespace FuzzyLib
{
    public class FuzzySetTermProxy : IFuzzyTerm
    {
        private readonly IFuzzySet _set;
        private FuzzySetTermProxy(IFuzzySet set)
        {
            _set = set;
        }

        public static FuzzySetTermProxy CreateProxyForSet(IFuzzySet set)
        {
            return new FuzzySetTermProxy(set);
        }

        public IFuzzySet Set { get { return _set; } }

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
            return new FuzzySetTermProxy(_set);
        }
    }
}
