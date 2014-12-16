namespace TestFuzzyLib
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

                case ':':
                    Code = TokenCode.Scope;
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