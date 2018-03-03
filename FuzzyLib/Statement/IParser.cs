using System;

namespace FuzzyLib.Statement
{
    public interface IParser
    {
        Tuple<IFuzzyTerm, IFuzzyTerm> ParseStatement(string statement);
    }
}
