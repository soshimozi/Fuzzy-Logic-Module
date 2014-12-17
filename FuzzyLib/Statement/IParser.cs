using System;

namespace FuzzyLib.Statement
{
    public interface IParser
    {
        Tuple<FuzzyTerm, FuzzyTerm> ParseStatement(string statement);
    }
}
