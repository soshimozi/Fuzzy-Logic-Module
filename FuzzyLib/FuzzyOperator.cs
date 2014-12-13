using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace FuzzyLib
{
    public static class FuzzyOperator
    {
        public static FuzzyOperatorAND And(FuzzyTerm lhs, FuzzyTerm rhs)
        {
            return new FuzzyOperatorAND(lhs, rhs);
        }

        public static FuzzyOperatorOR Or(FuzzyTerm lhs, FuzzyTerm rhs)
        {
            return new FuzzyOperatorOR(lhs, rhs);
        }
    }
}
