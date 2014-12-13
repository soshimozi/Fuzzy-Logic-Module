using System;

namespace FuzzyLib
{
    public class LeftShoulderFuzzySet : FuzzySet
    {
        //the values that define the shape of this FLV
        readonly double _peakPoint;
        readonly double _rightOffset;
        readonly double _leftOffset;

        public LeftShoulderFuzzySet(double min,
                              double peak,
                              double max)
            : base(((peak - (peak - min)) + peak) / 2, min, max)
        {
            _peakPoint = peak;
            _leftOffset = (peak - min);
            _rightOffset = (max - peak);
        }

        public override double CalculateDegreeOfMembership(double value)
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
    }
}
