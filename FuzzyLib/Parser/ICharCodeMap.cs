using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuzzyLib.Parser
{
    interface ICharCodeMap
    {
        CharacterType this[char character]
        {
            get;
        }
    }
}
