namespace TestFuzzyLib
{
    public interface IEnemy
    {
        double Health { get; set; }
        
        int Age { get; set; }
        
        double Hunger { get; set; }
        
        double DistanceToTarget { get; set; }
        
        int AmmoStatus { get; set; }

        double Desireability { get; set; }    
    }
}
