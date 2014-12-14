namespace FuzzyLib.Variables
{
    public class FuzzySetTerm : FuzzyTerm
    {
        private readonly FuzzySet _set;
        private FuzzySetTerm(FuzzySet set)
        {
            _set = set;
        }

        public static FuzzySetTerm CreateProxyForSet(FuzzySet set)
        {
            return new FuzzySetTerm(set);
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
            return new FuzzySetTerm(_set);
        }
    }
}
