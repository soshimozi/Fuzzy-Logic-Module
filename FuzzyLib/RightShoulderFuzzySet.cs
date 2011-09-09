using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuzzyLib
{
    public class RightShoulderFuzzySet : FuzzySet
    {
        //the values that define the shape of this FLV
        public double m_dPeakPoint;
        public double m_dLeftOffset;
        public double m_dRightOffset;

        public override double CalculateDegreeOfMovement(double value)
        {
            //test for the case where the left or right offsets are zero
            //(to prevent divide by zero errors below)
            if ((m_dRightOffset == 0 && m_dPeakPoint == value) ||
                 (m_dLeftOffset == 0 && m_dPeakPoint == value))
            {
                return 1.0;
            }

            //find DOM if left of center
            else if ((value <= m_dPeakPoint) && (value > (m_dPeakPoint - m_dLeftOffset)))
            {
                double grad = 1.0 / m_dLeftOffset;

                return grad * (value - (m_dPeakPoint - m_dLeftOffset));
            }
            //find DOM if right of center and less than center + right offset
            else if ((value > m_dPeakPoint) && (value <= m_dPeakPoint + m_dRightOffset))
            {
                return 1.0;
            }

            else
            {
                return 0;
            }
        }

        public RightShoulderFuzzySet(double min,
                               double peak,
                               double max)
            : base(((peak + (max - peak)) + peak) / 2, min, max)
        {
            m_dPeakPoint = peak;
            m_dLeftOffset = peak - min;
            m_dRightOffset = max - peak;
        }

    }
}
