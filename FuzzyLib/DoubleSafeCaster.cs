using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuzzyLib
{
    static class DoubleSafeCaster
    {
        public static double Convert(object value)
        {
            double tryvalue;
            if (double.TryParse(value.ToString(), out tryvalue))
            {
                return tryvalue;
            }

            return 0.0;
        }
    }
}
