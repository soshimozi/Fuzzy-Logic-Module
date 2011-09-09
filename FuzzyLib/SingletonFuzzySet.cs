using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuzzyLib
{
    public class SingletonFuzzySet : FuzzySet
    {
        //the values that define the shape of this FLV
        private double m_dMidPoint;
        private double m_dLeftOffset;
        private double m_dRightOffset;

        public override double CalculateDegreeOfMovement(double value)
        {
            if ((value >= m_dMidPoint - m_dLeftOffset) &&
                 (value <= m_dMidPoint + m_dRightOffset))
            {
                return 1.0;
            }

            //out of range of this FLV, return zero
            else
            {
                return 0.0;
            }
        }


        public SingletonFuzzySet(double min,
                             double peak,
                             double max)
            : base(peak, min, max)
        {
            m_dMidPoint = peak;
            m_dLeftOffset = peak - min;
            m_dRightOffset = max - peak;
        }
    }
}
