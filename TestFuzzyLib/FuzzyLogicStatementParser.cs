using FuzzyLib;
using FuzzyLib.Interfaces;
using Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestFuzzyLib
{
    public class FuzzyLogicStatementParser : StatementParser
    {
        private readonly FuzzyModule _module;
        public FuzzyLogicStatementParser(FuzzyModule module, CharCodeMap map) : base(map) => _module = module;

        protected override void Parse()
        {
            GetToken();
            if (TokenCode != TokenCode.If) throw new Exception("Missing If");

            GetToken();
            var antecedent = ParseExpression();

            if (TokenCode != TokenCode.Then) throw new Exception("Missing Then");

            GetToken();
            var consequence = ParseExpression();

            _module.AddRule(antecedent, consequence);
        }

        private IFuzzyTerm ParseExpression()
        {
            var resultTerm = ParseSimpleExpression();
            return resultTerm;
        }

        private IFuzzyTerm ParseSimpleExpression()
        {
            var resultTerm = ParseTerm();

            while (TcAddOps.Any(tc => tc == TokenCode))
            {
                var op = TokenCode;
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

            while (TcMulOps.Any(tc => tc == TokenCode))
            {
                var op = TokenCode;
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

            switch (TokenCode)
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

                    if (TokenCode == TokenCode.RParen)
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
            var setName = TokenType.TokenString;

            GetToken();

            var returnTerm = GetFuzzyTerm(setName);
            if (returnTerm == null) throw new Exception("Invalid variable scope or name.");

            return returnTerm;
        }

        private IFuzzyTerm GetFuzzyTerm(string setName)
        {
            var parts = setName.Split(':');
            if (parts.Length == 2)
            {
                return _module[parts[0]][parts[1]];
            }

            return null;
        }

    }
}
