using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace FuzzyLib.Parser
{
    internal class CharCodeMap : ICharCodeMap
    {
        private readonly Dictionary<char, CharacterType> _map = new Dictionary<char, CharacterType>();
        private readonly Dictionary<string, List<string>> _expressionMap = new Dictionary<string, List<string>>()
        {
            {"Letter", new List<string> { "[a-zA-Z]" } },
            {"Special", new List<string> {@"[_]",  @"[\(\)\:]"} },
            {"Digit", new List<string> { "[0-9]" } }
        };

        public CharCodeMap()
        {
            var expressions = LoadExpressions();
            MapExpressions(expressions);
        }

        private void MapExpressions(List<CharacterTypeExpression> expressions)
        {
            for (var c = (char)0; c < 127; c++)
            {
                foreach (var expression in expressions)
                {
                    var matches = expression.Expression.Matches(c.ToString(CultureInfo.InvariantCulture));
                    if (matches.Count == 0) continue;

                    // found a match
                    _map.Add(c, expression.Type);
                    break;
                }

                if (!_map.ContainsKey(c))
                    _map.Add(c, CharacterType.Error);
            }

            _map[(char)0] = CharacterType.EndOfLine;
        }

        private List<CharacterTypeExpression> LoadExpressions()
        {
            var expressions = new List<CharacterTypeExpression>();

            foreach (var type in _expressionMap.Keys)
            {
                foreach (var expression in _expressionMap[type])
                {
                    if (!TryGetExpression(expression, out Regex regex)) continue;
                    if (!Enum.TryParse(type, out CharacterType characterType)) characterType = CharacterType.Error;
                    expressions.Add(new CharacterTypeExpression { Expression = regex, Type = characterType });
                }
            }

            return expressions;
        }

        private bool TryGetExpression(string expression, out Regex regex)
        {
            try
            {
                regex = new Regex(expression);
                return true;
            }
            catch (Exception ex)
            {
                regex = null;
                return false;
            }
        }

        public CharacterType this[char character]
        {
            get
            {
                return _map.ContainsKey(character) ? _map[character] : CharacterType.Error;
            }
        }
    }
}
