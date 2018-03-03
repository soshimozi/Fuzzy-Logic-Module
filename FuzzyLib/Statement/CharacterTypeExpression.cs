using System.Text.RegularExpressions;

namespace FuzzyLib.Statement
{
    public class CharacterTypeExpression
    {
        public CharacterType Type { get; set; }
        public Regex Expression { get; set; }
    }
}