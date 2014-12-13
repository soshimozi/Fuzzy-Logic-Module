namespace TestFuzzyLib
{
    public class Enemy : ObservableObject<Enemy>
    {
        public double Health { get; set; }

        public int Age { get; set; }
        
        public double Hunger { get; set; }

        [Observable]
        public double DistanceToTarget { get; set; }

        [Observable]
        public int AmmoStatus { get; set; }

        public double Desireability { get; set; }

    }
}