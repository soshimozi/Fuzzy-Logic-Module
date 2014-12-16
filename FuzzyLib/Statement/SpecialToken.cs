namespace FuzzyLib.Statement
{
    public class SpecialToken : TokenType
    {
        public override void Parse(TextBuffer buffer)
        {
            switch (buffer.CurrentChar())
            {
                case '(':
                    Code = TokenCode.LParen;
                    break;

                case ')':
                    Code = TokenCode.RParen;
                    break;

                default:
                    Code = TokenCode.Error;
                    break;
            }

            buffer.MoveNext();
        }

        public override bool IsDelimiter
        {
            get { return true; }
        }

        public override bool IsReservedWord
        {
            get { return false; }
        }
    }
}