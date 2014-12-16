using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuzzyLib.Statement;

namespace TestFuzzyLib
{
    public class ErrorToken : TokenType
    {
        public ErrorToken()
        {
            Code = TokenCode.Error;
        }

        public override void Parse(TextBuffer buffer)
        {
            TokenString = buffer.CurrentChar().ToString();
            buffer.MoveNext();
        }

        public override bool IsDelimiter
        {
            get { return false; }
        }

        public override bool IsReservedWord
        {
            get { return false; }
        }
    }
}
