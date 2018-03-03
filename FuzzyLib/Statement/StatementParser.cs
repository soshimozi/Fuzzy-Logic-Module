using FuzzyLib.Interfaces;
using System;
using System.Linq;

namespace FuzzyLib.Statement
{
    public class StatementParser : IParser
    {
        private readonly FuzzyModule _module;
        private readonly CharCodeMap _characterMap;

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

        public StatementParser(FuzzyModule fuzzyModule, CharCodeMap map)
        {
            _characterMap = map;
            _module = fuzzyModule;
        }

        protected void GetToken()
        {
            _tokenType = _scanner.Get();
            _token = _tokenType.Code;
        }

        public Tuple<IFuzzyTerm, IFuzzyTerm> ParseStatement(string statement)
        {
            var textbuffer =
                new TextBuffer(statement);

            // initialize scanner with our new buffer
            // NOTE: this makes this class non-thread safe
            _scanner = new TextScanner(textbuffer, _characterMap);

            GetToken();

            if (_token != TokenCode.If) throw new Exception("Missing If");

            GetToken();
            var antecedent = ParseExpression();

            if (_token != TokenCode.Then) throw new Exception("Missing Then");

            GetToken();
            var consequence = ParseExpression();

            return new Tuple<IFuzzyTerm, IFuzzyTerm>(antecedent, consequence);
        }

        private IFuzzyTerm ParseExpression()
        {
            var resultTerm = ParseSimpleExpression();
            return resultTerm;
        }

        private IFuzzyTerm ParseSimpleExpression()
        {
            var resultTerm = ParseTerm();

            while (TcAddOps.Any(tc => tc == _token))
            {
                var op = _token;
                GetToken();

                var operandTerm = ParseTerm();
                if (op == TokenCode.Or)
                {
                    resultTerm = FuzzyModule.Or(resultTerm, operandTerm);
                }
            }

            return resultTerm;
        }

        private IFuzzyTerm ParseTerm()
        {
            var resultTerm = ParseFactor();

            while (TcMulOps.Any(tc => tc == _token))
            {
                var op = _token;
                GetToken();
                var operandTerm = ParseFactor();

                if (op == TokenCode.And)
                {
                    resultTerm = FuzzyModule.And(resultTerm, operandTerm);
                }
            }

            return resultTerm;
        }

        private IFuzzyTerm ParseFactor()
        {
            IFuzzyTerm resultTerm;

            switch (_token)
            {
                case TokenCode.Identifier:
                    resultTerm = ParseVariable();
                    break;

                case TokenCode.Fairly:
                    GetToken();
                    resultTerm = FuzzyModule.Fairly(ParseFactor());
                    break;

                case TokenCode.Very:
                    GetToken();
                    resultTerm = FuzzyModule.Very(ParseFactor());
                    break;

                case TokenCode.LParen:
                    GetToken();
                    resultTerm = ParseExpression();

                    if (_token == TokenCode.RParen)
                        GetToken();
                    else
                        throw new Exception("Missing Right Parenthesis");
                    break;

                default:
                    throw new Exception("Invalid Expression");
            }

            return resultTerm;
        }

        private IFuzzyTerm ParseVariable()
        {
            var setName = _tokenType.TokenString;

            GetToken();

            var returnTerm = _module.GetFuzzySet(setName);
            if (returnTerm == null) throw new Exception("Invalid variable scope or name.");

            return returnTerm;
        }


    }
}
