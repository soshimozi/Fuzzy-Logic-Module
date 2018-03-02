using System;

namespace FuzzyLib.Sets
{
    public class LeftShoulderFuzzySet : IFuzzySet
    {
        //the values that define the shape of this FLV
        private readonly double _peakPoint;
        private readonly double _rightOffset;
        private readonly double _leftOffset;

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

        public double DegreeOfMembership { get => _degreeOfMembership; set => _degreeOfMembership = value.Range(0, 1.0d); }

        public double RepresentativeValue
        {
            get;
            private set;
        }
        

        public LeftShoulderFuzzySet(double min,
                              double max,
                              double peak)
        {
            RepresentativeValue = ((peak - (peak - min)) + peak) / 2;
            MinBound = min;
            MaxBound = max;

            _peakPoint = peak;
            _leftOffset = (peak - min);
            _rightOffset = (max - peak);
        }

        public double CalculateDegreeOfMembership(double value)
        {
            //test for the case where the left or right offsets are zero
            //(to prevent divide by zero errors below)
            if ((Math.Abs(_rightOffset) < double.Epsilon && Math.Abs(_peakPoint - value) < double.Epsilon) ||
                 (Math.Abs(_leftOffset) < double.Epsilon && Math.Abs(_peakPoint - value) < double.Epsilon))
            {
                return 1.0;
            }

            //find DOM if right of center
            if ((value >= _peakPoint) && (value < (_peakPoint + _rightOffset)))
            {
                var grad = 1.0 / -_rightOffset;

                return grad * (value - _peakPoint) + 1.0;
            }

                //find DOM if left of center
            if ((value < _peakPoint) && (value >= _peakPoint - _leftOffset))
            {
                return 1.0;
            }

                //out of range of this FLV, return zero
            return 0.0;
        }

        public void Clear()
        {
            _degreeOfMembership = 0d;
        }

        public void MergeWithDegreeOfMembership(double value)
        {
            if (value > _degreeOfMembership)
                _degreeOfMembership = value;
        }
    }
}
