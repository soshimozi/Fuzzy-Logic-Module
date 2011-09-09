using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuzzyLib;

namespace TestFuzzyLib
{
    class Program
    {
        static FuzzyModule m_FuzzyModule = new FuzzyModule();
        static void Main(string[] args)
        {
            FuzzyModule fm = new FuzzyModule();

            FuzzyVariable DistToTarget = m_FuzzyModule.CreateFLV("DistToTarget");
            FuzzySetProxy Target_Close = DistToTarget.AddFuzzySet("Target_Close", FuzzySet.CreateLeftShoulderSet(0, 25, 150));
            FuzzySetProxy Target_Medium = DistToTarget.AddFuzzySet("Target_Medium", FuzzySet.CreateTriangularSet(25, 150, 300));
            FuzzySetProxy Target_Far = DistToTarget.AddFuzzySet( "Target_Far", FuzzySet.CreateRightShoulderSet(150, 300, 1000));

            FuzzyVariable Desirability = m_FuzzyModule.CreateFLV("Desirability");
            FuzzySetProxy Undesirable = Desirability.AddFuzzySet("Undesirable", FuzzySet.CreateLeftShoulderSet(0, 25, 50));
            FuzzySetProxy Desirable = Desirability.AddFuzzySet("Desirable", FuzzySet.CreateTriangularSet(25, 50, 75));
            FuzzySetProxy VeryDesirable = Desirability.AddFuzzySet("VeryDesirable", FuzzySet.CreateRightShoulderSet(50, 75, 100));

            FuzzyVariable AmmoStatus = m_FuzzyModule.CreateFLV("AmmoStatus");
            FuzzySetProxy Ammo_Loads = AmmoStatus.AddFuzzySet("Ammo_Loads", FuzzySet.CreateRightShoulderSet(10, 30, 100));
            FuzzySetProxy Ammo_Okay = AmmoStatus.AddFuzzySet("Ammo_Okay", FuzzySet.CreateTriangularSet(0, 10, 30));
            FuzzySetProxy Ammo_Low = AmmoStatus.AddFuzzySet("Ammo_Low", FuzzySet.CreateTriangularSet(0, 0, 10));

            m_FuzzyModule.AddRule(new FuzzyOperatorAND(Target_Close, Ammo_Loads), Undesirable);
            m_FuzzyModule.AddRule(new FuzzyOperatorAND(Target_Close, Ammo_Okay), Undesirable);
            m_FuzzyModule.AddRule(new FuzzyOperatorAND(Target_Close, Ammo_Low), Undesirable);

            m_FuzzyModule.AddRule(new FuzzyOperatorAND(Target_Medium, Ammo_Loads), VeryDesirable);
            m_FuzzyModule.AddRule(new FuzzyOperatorAND(Target_Medium, Ammo_Okay), VeryDesirable);
            m_FuzzyModule.AddRule(new FuzzyOperatorAND(Target_Medium, Ammo_Low), Desirable);

            m_FuzzyModule.AddRule(new FuzzyOperatorAND(Target_Far, Ammo_Loads), Desirable);
            m_FuzzyModule.AddRule(new FuzzyOperatorAND(Target_Far, Ammo_Okay), Undesirable);
            m_FuzzyModule.AddRule(new FuzzyOperatorAND(Target_Far, Ammo_Low), Undesirable);

            Console.Write("Enter distance: ");
            string distanceString = Console.ReadLine();

            double distance = 0;
            double.TryParse(distanceString, out distance);

            Console.Write("Enter rounds: ");
            string roundsString = Console.ReadLine();

            int numRoundsLeft = 0;
            int.TryParse(roundsString, out numRoundsLeft);

            //fuzzify distance and amount of ammo
            m_FuzzyModule.Fuzzify("DistToTarget", distance);
            m_FuzzyModule.Fuzzify("AmmoStatus", (double)numRoundsLeft);
            double desirabilityScore = m_FuzzyModule.DeFuzzify("Desirability", DefuzzifyMethod.MAX_AV);

            Console.WriteLine("Desirability Score: {0}", desirabilityScore);
            Console.ReadLine();
        }
    }
}
