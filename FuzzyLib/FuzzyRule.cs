using FuzzyLib.Variables;

namespace FuzzyLib
{
    public class FuzzyRule
    {
        //antecedent (usually a composite of several fuzzy sets and operators)
        private readonly FuzzyTerm _antecedent;

        //consequence (usually a single fuzzy set, but can be several ANDed together)
        private readonly FuzzyTerm _consequence;

        public FuzzyRule(FuzzyTerm antecedent,
                        FuzzyTerm consequence)
        {
            _antecedent = antecedent.Clone() as FuzzyTerm;
            _consequence = consequence.Clone() as FuzzyTerm;
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
