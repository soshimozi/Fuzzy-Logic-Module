namespace FuzzyLib.Operators
{
    public class VeryFuzzyOperator : IFuzzyTerm
    {
        private readonly IFuzzyTerm _term;

        public VeryFuzzyOperator(IFuzzyTerm term)
        {
            _term = term;
        }

        public override double DegreeOfMembership
        {
            get { return _term.DegreeOfMembership * _term.DegreeOfMembership; }
        }

        public override void ClearDegreeOfMembership()
        {
            _term.ClearDegreeOfMembership();
        }

        public override void MergeWithDOM(double value)
        {
            _term.MergeWithDOM(value * value);
        }

        public override object Clone()
        {
            return new VeryFuzzyOperator(_term.Clone() as IFuzzyTerm);
        }
    }
}
