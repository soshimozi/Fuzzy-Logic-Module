using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuzzyLib
{
    public abstract class FuzzyTerm : ICloneable
    {
        //retrieves the degree of membership of the term
        public abstract double DegreeOfMembership
        {
            get;
        }

        //clears the degree of membership of the term
        public abstract void ClearDegreOfMembership();

        //method for updating the DOM of a consequent when a rule fires
        public abstract void MergeWithDOM(double value);

        public abstract object Clone();
    }
}
