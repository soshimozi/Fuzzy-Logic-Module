using System.Globalization;

namespace FuzzyLib.Parser
{
    class ErrorToken : TokenType
    {
        public ErrorToken()
        {
            Code = TokenCode.Error;
        }

        public override void Parse(TextBuffer buffer)
        {
            TokenString = buffer.CurrentChar().ToString(CultureInfo.InvariantCulture);
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
