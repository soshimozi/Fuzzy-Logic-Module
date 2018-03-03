using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestFuzzyLib
{
    class EndToken : TokenType
    {
        public EndToken() : base()
        {
            Code = TokenCode.End;
        }

        public override void Parse(TextBuffer buffer)
        {
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
