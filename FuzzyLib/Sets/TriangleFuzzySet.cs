using FuzzyLib.Interfaces;
using System;

namespace FuzzyLib.Sets
{

    public class TriangleFuzzySet : IFuzzySetManifold
    {
        //the values that define the shape of this FLV
        private readonly double _peakPoint;
        private readonly double _leftOffset;
        private readonly double _rightOffset;

        private double _degreeOfMembership;

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

        public double RepresentativeValue
        {
            get;
            private set;
        }

        public double DegreeOfMembership { get => _degreeOfMembership; set => _degreeOfMembership = value.Range(0, 1.0d); }

        public TriangleFuzzySet(double min,
                            double max,
                            double peak)
        {

            MinBound = min;
            MaxBound = max;
            RepresentativeValue = peak;

            _peakPoint = peak;
            _leftOffset = peak - min;
            _rightOffset = max - peak;
        }

        public double CalculateDegreeOfMembership(double value)
        {
            //test for the case where the triangle's left or right offsets are zero
            //(to prevent divide by zero errors below)
            if ((Math.Abs(_rightOffset) < double.Epsilon && Math.Abs(_peakPoint - value) < double.Epsilon) ||
                 (Math.Abs(_leftOffset) < double.Epsilon && Math.Abs(_peakPoint - value) < double.Epsilon))
            {
                return 1.0;
            }

            double grad;

            //find DOM if left of center
            if ((value <= _peakPoint) && (value >= (_peakPoint - _leftOffset)))
            {
                grad = 1.0 / _leftOffset;

                return grad * (value - (_peakPoint - _leftOffset));
            }

            //out of range of this FLV, return zero
            if ((!(value > _peakPoint)) || (!(value < (_peakPoint + _rightOffset)))) return 0.0;

            //find DOM if right of center
            grad = 1.0 / -_rightOffset;
            return grad * (value - _peakPoint) + 1.0;
        }

        public void Clear()
        {
            _degreeOfMembership = 0.0;
        }

        public void MergeWithDegreeOfMembership(double value)
        {
            if (value > _degreeOfMembership)
                _degreeOfMembership = value;
        }
    }
}
