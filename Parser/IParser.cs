using System;

namespace Parser
{
    public interface IParser
    {
        void ParseStatement(string statement);
    }
}
