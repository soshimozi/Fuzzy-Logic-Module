using System.Collections.Generic;
using System.Text;

namespace TestFuzzyLib
{
    public class WordToken : TokenType
    {
        private readonly Dictionary<string, TokenCode> _reservedWords = new Dictionary<string, TokenCode>
        {
            {"AND", TokenCode.And},
            {"OR", TokenCode.Or},
            {"NOT", TokenCode.Not},
            {"IF", TokenCode.If},
            {"THEN", TokenCode.Then},
            {"VERY", TokenCode.Very},
            {"FAIRLY", TokenCode.Fairly}
        };

        private bool _isReservedWord;

        private CharCodeMap _characterMap;
        public WordToken(CharCodeMap characterMap)
        {
            _characterMap = characterMap;
        }

        public override void Parse(TextBuffer buffer)
        {
            StringBuilder tokenBuilder = new StringBuilder();

            char ch = buffer.CurrentChar();

            do
            {
                tokenBuilder.Append(ch);

                ch = buffer.MoveNext();
            } while (_characterMap[ch] == CharacterType.Letter || _characterMap[ch] == CharacterType.Digit || ch == '_' );

            TokenString = tokenBuilder.ToString();

            CheckForReservedWord();
        }

        private void CheckForReservedWord()
        {
            _isReservedWord = _reservedWords.ContainsKey(TokenString);
            Code = _isReservedWord ? _reservedWords[TokenString] : TokenCode.Identifier;
        }

        public override bool IsDelimiter
        {
            get { return false; }
        }

        public override bool IsReservedWord
        {
            get { return _isReservedWord; }
        }
    }
}