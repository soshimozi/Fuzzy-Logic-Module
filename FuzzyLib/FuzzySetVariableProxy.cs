using FuzzyLib.Sets;

namespace FuzzyLib
{
    public class FuzzySetTermProxy : FuzzyTerm
    {
        private readonly FuzzySet _set;
        private FuzzySetTermProxy(FuzzySet set)
        {
            _set = set;
        }

        public static FuzzySetTermProxy CreateProxyForSet(FuzzySet set)
        {
            return new FuzzySetTermProxy(set);
        }

        public FuzzySet Set { get { return _set; } }

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
