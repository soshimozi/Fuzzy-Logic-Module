using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuzzyLib
{
    public abstract class FuzzySet
    {
        protected double _degreeOfMembership;

        public double MinBound
        {
            get;
            private set;
        }

        public double MaxBound
        {
            get;
            private set;
        }

        // this is the maximum of the set's membership membership function.  For instance,
        // if the set is triangular then this will be the peak point of the triangle.
        // It is calculated at the constructor to avoid run-time calculation of midpoint values.
        protected double _representativeValue;

        public FuzzySet(double representativeValue, double minBound, double maxBound)
        {
            _degreeOfMembership = 0;
            _representativeValue = representativeValue;

            MinBound = minBound;
            MaxBound = maxBound;
        }

        abstract public double CalculateDegreeOfMovement(double value);

        //if this fuzzy set is part of a consequent FLV, and it is fired by a rule 
        //then this method sets the DOM (in this context, the DOM represents a
        //confidence level)to the maximum of the parameter value or the set's 
        //existing m_dDOM value
        public void MergeWithDegreeOfMovement(double value)
        {
            if (value > _degreeOfMembership)
                _degreeOfMembership = value;
        }

        public double RepresentativeValue
        {
            get { return _representativeValue; }
        }

        public void Clear()
        {
            _degreeOfMembership = 0;
        }

        public double DegreeOfMembership
        {
            get { return _degreeOfMembership; }
            set
            {
                if (value > 1.0d)
                {
                    _degreeOfMembership = 1.0d;
                }
                else if (value < 0)
                {
                    _degreeOfMembership = 0;
                }
                else
                {
                    _degreeOfMembership = value;
                }

            }

        }

        public static FuzzySet CreateTriangularSet(double min, double peak, double max)
        {
            return new TriangleFuzzySet(min, peak, max);
        }

        public static FuzzySet CreateLeftShoulderSet(double min, double peak, double max)
        {
            return new LeftShoulderFuzzySet (min, peak, max);
        }

        public static FuzzySet CreateRightShoulderSet(double min, double peak, double max)
        {
            return new RightShoulderFuzzySet(min, peak, max);
        }

        public static FuzzySet CreateSingletonShoulderSet(double min, double peak, double max)
        {
            return new SingletonFuzzySet(min, peak, max);
        }
    }
}
