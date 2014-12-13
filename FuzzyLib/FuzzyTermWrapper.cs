namespace FuzzyLib
{
    public class FuzzyTermWrapper<TFuzzyTerm> where TFuzzyTerm : FuzzyTerm
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

        public FuzzyTermWrapper<FuzzyOperatorAND> And(FuzzyTerm term)
        {
            return new FuzzyTermWrapper<FuzzyOperatorAND>(FuzzyOperator.And(_wrapped, term));
        }

        public FuzzyTermWrapper<FuzzyOperatorAND> And<TTerm>(FuzzyTermWrapper<TTerm> term) where TTerm : FuzzyTerm
        {
            return new FuzzyTermWrapper<FuzzyOperatorAND>(FuzzyOperator.And(_wrapped, term.Wrapped));
        }

        public FuzzyTermWrapper<FuzzyOperatorOR> Or(FuzzyTerm term)
        {
            return new FuzzyTermWrapper<FuzzyOperatorOR>(FuzzyOperator.Or(_wrapped, term));
        }

        public FuzzyTermWrapper<FuzzyOperatorOR> Or<TTerm>(FuzzyTermWrapper<TTerm> term) where TTerm : FuzzyTerm
        {
            return new FuzzyTermWrapper<FuzzyOperatorOR>(FuzzyOperator.Or(_wrapped, term.Wrapped));
        }
    }
}
