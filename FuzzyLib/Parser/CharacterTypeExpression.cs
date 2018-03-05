using System.Text.RegularExpressions;

namespace FuzzyLib.Parser
{
    internal class CharacterTypeExpression
    {
        public CharacterType Type { get; set; }
        public Regex Expression { get; set; }
    }
}