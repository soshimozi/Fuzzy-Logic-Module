namespace FuzzyLib.Statement
{
    public abstract class TokenType
    {
        public TokenCode Code { get; set; }
        public string TokenString { get; set; }

        public abstract void Parse(TextBuffer buffer);

        public abstract bool IsDelimiter { get; }
        public abstract bool IsReservedWord { get; }
    }
}
