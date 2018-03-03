using System;

namespace Parser
{
    public class TextScanner
    {
        private readonly TextBuffer _buffer;
        private readonly CharCodeMap _map;

        public TextScanner(TextBuffer buffer, CharCodeMap map)
        {
            _buffer = buffer;
            _map = map;
        }

        public TokenType Get()
        {
            while (Char.IsWhiteSpace(_buffer.CurrentChar()) && _buffer.CurrentChar() != 0)
                _buffer.MoveNext();

            TokenType token;
            switch (_map[_buffer.CurrentChar()])
            {
                case CharacterType.Special:
                    token = new SpecialToken();
                    break;

                case CharacterType.Error:
                    token = new ErrorToken();
                    break;

                case CharacterType.Letter:
                    token = new WordToken(_map);
                    break;

                case CharacterType.EndOfLine:
                    token = new EndToken();
                    break;

                default:
                    token = new ErrorToken();
                    break;
            }


            token.Parse(_buffer);
            return token;
        }
    }
}
