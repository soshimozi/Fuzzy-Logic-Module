using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestFuzzyLib
{
    public class TextScanner
    {
        private TextBuffer _buffer;
        private CharCodeMap _map;

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
