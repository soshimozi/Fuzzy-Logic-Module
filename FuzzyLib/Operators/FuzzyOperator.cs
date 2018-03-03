namespace FuzzyLib.Operators
{
    public static class FuzzyOperator
    {
        /* binary operations */
        public static FuzzyOperatorAnd And(IFuzzyTerm lhs, IFuzzyTerm rhs)
        {
            return new FuzzyOperatorAnd(lhs, rhs);
        }

        public static FuzzyOperatorOr Or(IFuzzyTerm lhs, IFuzzyTerm rhs)
        {
            return new FuzzyOperatorOr(lhs, rhs);
        }

        /* unary operations */
        public static FairlyFuzzyOperator Fairly(IFuzzyTerm term)
        {
            return new FairlyFuzzyOperator(term);
        }

        public static VeryFuzzyOperator Very(IFuzzyTerm term)
        {
            return new VeryFuzzyOperator(term);
        }

    }
}
