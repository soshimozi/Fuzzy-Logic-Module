using System.Text.RegularExpressions;

namespace Parser
{
    public class CharacterTypeExpression
    {
        public CharacterType Type { get; set; }
        public Regex Expression { get; set; }
    }
}