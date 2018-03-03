using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuzzyLib
{
    static class MathExtensions
    {
        public static double Range(this double val, double inclusiveMin, double inclusiveMax)
        {
            if (val < inclusiveMin) { return inclusiveMin; }
            if (val > inclusiveMax) { return inclusiveMax; }

            return val;
        }
    }
}
