using System.Reflection;
using FuzzyLib.Variables;

namespace FuzzyLib.Object
{
    public class FuzzyVariableReference
    {
        public FuzzyVariable Variable { get; set; }   
        public PropertyInfo PropertyInfo { get; set; }
    }
}