using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace Parser
{
    public class CharCodeMap
    {
        private readonly Dictionary<char, CharacterType> _map = new Dictionary<char, CharacterType>();

        public void LoadXml(string xml)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml);

            Load(doc);
        }

        public void Load(XmlDocument doc)
        {
            var nodes = doc.SelectNodes("map/add");

            var expressions = new List<CharacterTypeExpression>();

            if (nodes == null) return;

            foreach (var node in nodes.Cast<XmlNode>())
            {
                if (node == null || node.Attributes == null) continue;

                var expressionAttribute =
                    node.Attributes.Cast<XmlAttribute>().FirstOrDefault(t => t.Name == "expression");

                var typeAttribute = 
                    node.Attributes.Cast<XmlAttribute>().FirstOrDefault(t => t.Name == "type");

                if (expressionAttribute == null || typeAttribute == null) continue;

                Regex rex;
                try
                {
                    rex = new Regex(expressionAttribute.Value);
                }
                catch (Exception)
                {
                    continue;
                }

                CharacterType characterType;
                if(!Enum.TryParse(typeAttribute.Value, out characterType))
                    characterType = CharacterType.Error;

                expressions.Add(new CharacterTypeExpression {Expression = rex, Type = characterType});
            }

            for (var c = (char) 0; c < 127; c++)
            {
                foreach (var expression in expressions)
                {
                    var matches = expression.Expression.Matches(c.ToString(CultureInfo.InvariantCulture));
                    if (matches.Count == 0) continue;

                    // found a match
                    _map.Add(c, expression.Type);
                    break;
                }

                if(!_map.ContainsKey(c))
                    _map.Add(c, CharacterType.Error);
            }

            _map[(char)0] = CharacterType.EndOfLine;
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
