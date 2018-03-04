
using System;
using System.Linq;

namespace Parser
{
    public abstract class StatementParser : IParser
    {
        private readonly ICharCodeMap _characterMap;

        private TokenCode _token;
        private TokenType _tokenType;
        private TextScanner _scanner;

        protected static readonly TokenCode[] TcAddOps
            =
        {
            TokenCode.Or
        };

        protected static readonly TokenCode[] TcMulOps
            =
        {
            TokenCode.And,
            TokenCode.Very,
            TokenCode.Fairly
        };

        public StatementParser(ICharCodeMap map)
        {
            _characterMap = map;
        }

        protected void GetToken()
        {
            _tokenType = _scanner.Get();
            _token = _tokenType.Code;
        }

        public TokenCode TokenCode
        {
            get
            {
                if(_tokenType == null)
                {
                    return TokenCode.Error;
                }

                return _tokenType.Code;
            }
        }

        public TokenType TokenType => _tokenType;


        public void ParseRule(string ruleStatement)
        {
            var textbuffer =
                new TextBuffer(ruleStatement);

            // initialize scanner with our new buffer
            // NOTE: this makes this class non-thread safe
            _scanner = new TextScanner(textbuffer, _characterMap);

            Parse();
        }

        protected abstract void Parse();

    }
}
