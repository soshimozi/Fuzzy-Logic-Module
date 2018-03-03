using System;

namespace FuzzyLib.Interfaces
{
    public abstract class IFuzzyTerm : ICloneable
    {
        //retrieves the degree of membership of the term
        public abstract double DegreeOfMembership
        {
            get;
        }

        //clears the degree of membership of the term
        public abstract void ClearDegreeOfMembership();

        //method for updating the DOM of a consequent when a rule fires
        public abstract void MergeWithDOM(double value);

        public abstract object Clone();
    }
}
