using FuzzyLib.AOP;
using FuzzyLib.Observables;


namespace TestFuzzyLib
{

    public class Enemy : DynamicPOCO<Enemy>
    {
        public double Health { get; set; }

        public int Age { get; set; }
        
        public double Hunger { get; set; }

        [Observable]
        public double DistanceToTarget { get; set; }

        [Observable]
        public int AmmoStatus { get; set; }

        [Observable]
        public double Skill { get; set; }

        public double Desirability { get; set; }
    }
}