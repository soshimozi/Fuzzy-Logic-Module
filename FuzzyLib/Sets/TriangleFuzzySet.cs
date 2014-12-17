using System;

namespace FuzzyLib.Sets
{

    public class TriangleFuzzySet : FuzzySet
    {
        //the values that define the shape of this FLV
        private readonly double _peakPoint;
        private readonly double _leftOffset;
        private readonly double _rightOffset;
        private readonly double _min;
        private readonly double _max;

        public override double GetMinBound()
        {
            return _min;
        }

        public override double GetMaxBound()
        {
            return _max;
        }

        public TriangleFuzzySet(double min,
                            double peak,
                            double max)
            : base(peak)
        {
            _min = min;
            _max = max;

            _peakPoint = peak;
            _leftOffset = peak - min;
            _rightOffset = max - peak;
        }

        public override double CalculateDegreeOfMembership(double value)
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
    }
}
