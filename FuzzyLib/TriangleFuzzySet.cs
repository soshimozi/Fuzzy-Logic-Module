using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuzzyLib
{

    public class TriangleFuzzySet : FuzzySet
    {
        //the values that define the shape of this FLV
        public double _peakPoint;
        public double _leftOffset;
        public double _rightOffset;

        public TriangleFuzzySet(double min,
                            double peak,
                            double max)
            : base(peak, min, max)
        {
            _peakPoint = peak;
            _leftOffset = peak - min;
            _rightOffset = max - peak;
        }

        public override double CalculateDegreeOfMovement(double value)
        {
            //test for the case where the triangle's left or right offsets are zero
            //(to prevent divide by zero errors below)
            if ((_rightOffset == 0 && _peakPoint == value) ||
                 (_leftOffset == 0 && _peakPoint == value))
            {
                return 1.0;
            }

            //find DOM if left of center
            if ((value <= _peakPoint) && (value >= (_peakPoint - _leftOffset)))
            {
                double grad = 1.0 / _leftOffset;

                return grad * (value - (_peakPoint - _leftOffset));
            }
            //find DOM if right of center
            else if ((value > _peakPoint) && (value < (_peakPoint + _rightOffset)))
            {
                double grad = 1.0 / -_rightOffset;

                return grad * (value - _peakPoint) + 1.0;
            }
            //out of range of this FLV, return zero
            else
            {
                return 0.0;
            }
          }
    }
}
