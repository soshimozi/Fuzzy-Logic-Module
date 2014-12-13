namespace FuzzyLib
{
    public class FuzzyTermWrapper<TFuzzyTerm> : FuzzyTerm where TFuzzyTerm : FuzzyTerm
    {
        private readonly TFuzzyTerm _wrapped;

        public FuzzyTermWrapper(TFuzzyTerm wrapped)
        {
            _wrapped = wrapped;
        }

        public TFuzzyTerm Wrapped { get { return _wrapped;  } }

        public static FuzzyTermWrapper<TFuzzyTerm> Wrap(TFuzzyTerm towrap)
        {
            return new FuzzyTermWrapper<TFuzzyTerm>(towrap);
        }

        public FuzzyTermWrapper<FuzzyOperatorAnd> And(FuzzyTerm term)
        {
            return new FuzzyTermWrapper<FuzzyOperatorAnd>(FuzzyOperator.And(_wrapped, term));
        }

        public FuzzyTermWrapper<FuzzyOperatorOr> Or(FuzzyTerm term)
        {
            return new FuzzyTermWrapper<FuzzyOperatorOr>(FuzzyOperator.Or(_wrapped, term));
        }

        public FuzzyTermWrapper<FairlyFuzzyOperator> Fairly()
        {
            return new FuzzyTermWrapper<FairlyFuzzyOperator>(FuzzyOperator.Fairly(this));
        }

        public FuzzyTermWrapper<VeryFuzzyOperator> Very()
        {
            return new FuzzyTermWrapper<VeryFuzzyOperator>(FuzzyOperator.Very(this));
        }

        public override double DegreeOfMembership
        {
            get { return _wrapped.DegreeOfMembership; }
        }

        public override void ClearDegreeOfMembership()
        {
            _wrapped.ClearDegreeOfMembership();
        }

        public override void MergeWithDOM(double value)
        {
            _wrapped.MergeWithDOM(value);
        }

        public override object Clone()
        {
            return _wrapped.Clone();
        }
    }
}
