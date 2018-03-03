using System.Reflection;

namespace FuzzyLib.Object
{
    public class FuzzyVariableReference
    {
        public FuzzyVariable Variable { get; set; }   
        public PropertyInfo PropertyInfo { get; set; }
    }
}