using FuzzyLib.Variables;

namespace FuzzyLib.Operators
{
    public static class FuzzyOperator
    {
        /* binary operations */
        public static FuzzyOperatorAnd And(FuzzyTerm lhs, FuzzyTerm rhs)
        {
            return new FuzzyOperatorAnd(lhs, rhs);
        }

        public static FuzzyOperatorOr Or(FuzzyTerm lhs, FuzzyTerm rhs)
        {
            return new FuzzyOperatorOr(lhs, rhs);
        }

        /* unary operations */
        public static FairlyFuzzyOperator Fairly(FuzzyTerm term)
        {
            return new FairlyFuzzyOperator(term);
        }

        public static VeryFuzzyOperator Very(FuzzyTerm term)
        {
            return new VeryFuzzyOperator(term);
        }

    }
}
