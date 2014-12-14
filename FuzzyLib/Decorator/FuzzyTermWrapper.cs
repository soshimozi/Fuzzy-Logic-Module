using FuzzyLib.Operators;
using FuzzyLib.Variables;

namespace FuzzyLib.Decorator
{
    public class FuzzyTermDecorator<TFuzzyTerm> : FuzzyTerm where TFuzzyTerm : FuzzyTerm
    {
        private readonly TFuzzyTerm _wrapped;

        public FuzzyTermDecorator(TFuzzyTerm wrapped)
        {
            _wrapped = wrapped;
        }

        public TFuzzyTerm Wrapped { get { return _wrapped;  } }

        public static FuzzyTermDecorator<TFuzzyTerm> Wrap(TFuzzyTerm towrap)
        {
            return new FuzzyTermDecorator<TFuzzyTerm>(towrap);
        }

        public FuzzyTermDecorator<FuzzyOperatorAnd> And(FuzzyTerm term)
        {
            return new FuzzyTermDecorator<FuzzyOperatorAnd>(FuzzyOperator.And(_wrapped, term));
        }

        public FuzzyTermDecorator<FuzzyOperatorOr> Or(FuzzyTerm term)
        {
            return new FuzzyTermDecorator<FuzzyOperatorOr>(FuzzyOperator.Or(_wrapped, term));
        }

        public FuzzyTermDecorator<FairlyFuzzyOperator> Fairly()
        {
            return new FuzzyTermDecorator<FairlyFuzzyOperator>(FuzzyOperator.Fairly(this));
        }

        public FuzzyTermDecorator<VeryFuzzyOperator> Very()
        {
            return new FuzzyTermDecorator<VeryFuzzyOperator>(FuzzyOperator.Very(this));
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
