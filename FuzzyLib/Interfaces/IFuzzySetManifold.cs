using FuzzyLib.Sets;

namespace FuzzyLib.Interfaces
{
    public interface IFuzzySetManifold
    {
        double MinBound { get; }

        double MaxBound { get; }

        // this is the maximum of the set's membership membership function.  For instance,
        // if the set is triangular then this will be the peak point of the triangle.
        // It is calculated at the constructor to avoid run-time calculation of midpoint values.
        double RepresentativeValue { get; }

        double CalculateDegreeOfMembership(double value);

        //if this fuzzy set is part of a consequent FLV, and it is fired by a rule 
        //then this method sets the DOM (in this context, the DOM represents a
        //confidence level)to the maximum of the parameter value or the set's 
        //existing m_dDOM value
        void MergeWithDegreeOfMembership(double value);

        void Clear();

        double DegreeOfMembership { get; set; }
    }
}
