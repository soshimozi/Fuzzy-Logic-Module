using System;

namespace FuzzyLib
{
    public class RightShoulderFuzzySet : FuzzySet
    {
        //the values that define the shape of this FLV
        private readonly double _peakPoint;
        private readonly double _leftOffset;
        private readonly double _rightOffset;

        public override double CalculateDegreeOfMembership(double value)
        {
            //test for the case where the left or right offsets are zero
            //(to prevent divide by zero errors below)
            if ((Math.Abs(_rightOffset) < double.Epsilon && Math.Abs(_peakPoint - value) < double.Epsilon) ||
                 (Math.Abs(_leftOffset) < double.Epsilon && Math.Abs(_peakPoint - value) < double.Epsilon))
            {
                return 1.0;
            }

            //find DOM if left of center
            if ((value <= _peakPoint) && (value > (_peakPoint - _leftOffset)))
            {
                double grad = 1.0 / _leftOffset;

                return grad * (value - (_peakPoint - _leftOffset));
            }
                //find DOM if right of center and less than center + right offset
            if ((value > _peakPoint) && (value <= _peakPoint + _rightOffset))
            {
                return 1.0;
            }

            return 0;
        }

        public RightShoulderFuzzySet(double min,
                               double peak,
                               double max)
            : base(((peak + (max - peak)) + peak) / 2, min, max)
        {
            _peakPoint = peak;
            _leftOffset = peak - min;
            _rightOffset = max - peak;
        }

    }
}
