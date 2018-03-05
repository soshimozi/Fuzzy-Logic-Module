namespace FuzzyLib.Parser
{
    class EndToken : TokenType
    {
        public EndToken()
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
