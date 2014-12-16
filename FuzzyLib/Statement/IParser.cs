using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuzzyLib.Variables;

namespace FuzzyLib.Statement
{
    public interface IParser
    {
        Tuple<FuzzyTerm, FuzzyTerm> ParseStatement(string statement);
    }
}
