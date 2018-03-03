namespace FuzzyLib
{
    public class FuzzyRule
    {
        //antecedent (usually a composite of several fuzzy sets and operators)
        private readonly IFuzzyTerm _antecedent;

        //consequence (usually a single fuzzy set, but can be several ANDed together)
        private readonly IFuzzyTerm _consequence;

        public FuzzyRule(IFuzzyTerm antecedent,
                        IFuzzyTerm consequence)
        {
            _antecedent = antecedent.Clone() as IFuzzyTerm;
            _consequence = consequence.Clone() as IFuzzyTerm;
        }

        public void SetConfidenceOfConsequentToZero()
        {
            _consequence.ClearDegreeOfMembership();
        }

        //this method updates the DOM (the confidence) of the consequent term with
        //the DOM of the antecedent term. 
        public void Calculate()
        {
            _consequence.MergeWithDOM(_antecedent.DegreeOfMembership);
        }
    }
}
