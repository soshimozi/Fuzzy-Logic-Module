using System;
using System.Dynamic;
using System.Text;
using FuzzyLib;
using System.ComponentModel;

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
            FuzzySetProxy Target_Far = DistToTarget.AddFuzzySet("Target_Far", FuzzySet.CreateRightShoulderSet(150, 300, 1000));

            FuzzyVariable Desirability = m_FuzzyModule.CreateFLV("Desirability");
            FuzzySetProxy Undesirable = Desirability.AddFuzzySet("Undesirable", FuzzySet.CreateLeftShoulderSet(0, 25, 50));
            FuzzySetProxy Desirable = Desirability.AddFuzzySet("Desirable", FuzzySet.CreateTriangularSet(25, 50, 75));
            FuzzySetProxy VeryDesirable = Desirability.AddFuzzySet("VeryDesirable", FuzzySet.CreateRightShoulderSet(50, 75, 100));

            FuzzyVariable AmmoStatus = m_FuzzyModule.CreateFLV("AmmoStatus");
            FuzzySetProxy Ammo_Loads = AmmoStatus.AddFuzzySet("Ammo_Loads", FuzzySet.CreateRightShoulderSet(10, 30, 100));
            FuzzySetProxy Ammo_Okay = AmmoStatus.AddFuzzySet("Ammo_Okay", FuzzySet.CreateTriangularSet(0, 10, 30));
            FuzzySetProxy Ammo_Low = AmmoStatus.AddFuzzySet("Ammo_Low", FuzzySet.CreateTriangularSet(0, 0, 10));

            //m_FuzzyModule.AddRule(new FuzzyOperatorOr(new FuzzyOperatorAnd(Target_Close, Ammo_Low), new FuzzyOperatorOr(new FuzzyOperatorAnd(Target_Close, Ammo_Loads), new FuzzyOperatorAnd(Target_Close, Ammo_Okay))), Undesirable);
            //m_FuzzyModule.AddRule(new FuzzyOperatorAND(Target_Close, Ammo_Okay), Undesirable);
            //m_FuzzyModule.AddRule(new FuzzyOperatorAND(Target_Close, Ammo_Low), Undesirable);

            m_FuzzyModule.AddRule(new FuzzyOperatorAnd(Target_Medium, Ammo_Loads), VeryDesirable);
            m_FuzzyModule.AddRule(new FuzzyOperatorAnd(Target_Medium, Ammo_Okay), VeryDesirable);
            m_FuzzyModule.AddRule(new FuzzyOperatorAnd(Target_Medium, Ammo_Low), Desirable);

            m_FuzzyModule.AddRule(new FuzzyOperatorAnd(Target_Far, Ammo_Loads), Desirable);
            m_FuzzyModule.AddRule(new FuzzyOperatorAnd(Target_Far, Ammo_Okay), Undesirable);
            m_FuzzyModule.AddRule(new FuzzyOperatorAnd(Target_Far, Ammo_Low), Undesirable);

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
            double desirabilityScore = m_FuzzyModule.DeFuzzify("Desirability", p => p.DeFuzzifyMaxAv());

            Console.WriteLine("Desirability Score: {0}", desirabilityScore);
            Console.ReadLine();

            var enemy = new Enemy();
            var mod = new FuzzyObject<Enemy>(enemy);


            mod.DefineVariable(p => p.DistanceToTarget);
            mod.DefineVariable(p => p.AmmoStatus);
            mod.DefineVariable(p => p.Desireability);

            mod.AddFuzzySet("Ammo_Loads", p => p.AmmoStatus, FuzzySet.CreateRightShoulderSet, 10, 20, 100)
                .AddFuzzySet("Ammo_Okay", p => p.AmmoStatus, FuzzySet.CreateTriangularSet, 0, 10, 30)
                .AddFuzzySet("Ammo_Low", p => p.AmmoStatus, FuzzySet.CreateTriangularSet, 0, 0, 10);

            mod.AddFuzzySet("Undesirable", p => p.Desireability, FuzzySet.CreateLeftShoulderSet, 0, 25, 50)
                .AddFuzzySet("Desirable", p => p.Desireability, FuzzySet.CreateTriangularSet, 25, 50, 75)
                .AddFuzzySet("VeryDesirable", p => p.Desireability, FuzzySet.CreateRightShoulderSet, 50, 75, 100);

            mod.AddFuzzySet("Target_Close", p => p.DistanceToTarget, FuzzySet.CreateLeftShoulderSet, 0, 25, 150)
                .AddFuzzySet("Target_Medium", p => p.DistanceToTarget, FuzzySet.CreateTriangularSet, 25, 150, 300)
                .AddFuzzySet( "Target_Far", p => p.DistanceToTarget, FuzzySet.CreateRightShoulderSet, 150, 300, 1000);

            //FuzzyVariable Desirability = m_FuzzyModule.CreateFLV("Desirability");
            //FuzzySetProxy Undesirable = Desirability.AddFuzzySet("Undesirable", FuzzySet.CreateLeftShoulderSet(0, 25, 50));
            //FuzzySetProxy Desirable = Desirability.AddFuzzySet("Desirable", FuzzySet.CreateTriangularSet(25, 50, 75));
            //FuzzySetProxy VeryDesirable = Desirability.AddFuzzySet("VeryDesirable", FuzzySet.CreateRightShoulderSet(50, 75, 100));

            //dynamic modwrapper = mod;

            dynamic modwrapper = mod.GetDynamic();

            mod.AddRule(
                mod.Or(mod.WrapSet("Target_Close")
                        .And(mod["Ammo_Low"]),
                       mod.Or(mod.WrapSet("Target_Close")
                            .And(mod["Ammo_Loads"]), 
                       mod.WrapSet("Target_Close")
                            .And(mod["Ammo_Okay"]))
                ), 
                mod["Undesirable"]);

            mod.AddRule(mod.WrapSet("Target_Close").And(mod["Ammo_Okay"]), mod["Undesirable"]);
            mod.AddRule(mod.WrapSet("Target_Close").And(mod["Ammo_Low"]), mod["Undesirable"]);

            mod.AddRule(mod.WrapSet("Target_Medium").And(mod["Ammo_Loads"]), mod["VeryDesirable"]);
            mod.AddRule(mod.WrapSet("Target_Medium").And(mod["Ammo_Okay"]), mod["VeryDesirable"]);
            mod.AddRule(mod.WrapSet("Target_Medium").And(mod["Ammo_Low"]), mod["Desirable"]);

            mod.AddRule(mod.WrapSet("Target_Far").And(mod["Ammo_Loads"]), mod["Desirable"]);
            mod.AddRule(mod.WrapSet("Target_Far").And(mod["Ammo_Okay"]), mod["Undesirable"]);
            mod.AddRule(mod.WrapSet("Target_Far").And(mod["Ammo_Low"]), mod["Undesirable"]);

            //m_FuzzyModule.AddRule(new FuzzyOperatorAND(Target_Far, Ammo_Loads), Desirable);
            //m_FuzzyModule.AddRule(new FuzzyOperatorAND(Target_Far, Ammo_Okay), Undesirable);
            //m_FuzzyModule.AddRule(new FuzzyOperatorAND(Target_Far, Ammo_Low), Undesirable);

            mod.AddRule(mod.WrapSet("Target_Far").Very().And(mod["Ammo_Low"]), mod["Desireable"]);

            //    new FuzzyOperatorOR(new FuzzyOperatorAND(Target_Close, Ammo_Low), new FuzzyOperatorOR(new FuzzyOperatorAND(Target_Close, Ammo_Loads), new FuzzyOperatorAND(Target_Close, Ammo_Okay))), Undesirable);

            //FuzzySetProxy Ammo_Okay = AmmoStatus.AddFuzzySet("Ammo_Okay", FuzzySet.CreateTriangularSet(0, 10, 30));
            //FuzzySetProxy Ammo_Low = AmmoStatus.AddFuzzySet("Ammo_Low", FuzzySet.CreateTriangularSet(0, 0, 10));


            //var ammoLoads = distanceVariable.AddFuzzySet("dont", FuzzySet.CreateTriangularSet(0, 50, 4000));
            //var desirable = desireability.AddFuzzySet("dont", FuzzySet.CreateTriangularSet(0, 50, 4000));

            enemy.DistanceToTarget = 45;
            enemy.AmmoStatus = 5;
            enemy.Desireability = 23;

            // fuzzify some variables to be used
            //mod.Fuzzify(p => p.DistanceToTarget);
            //mod.Fuzzify(p => p.AmmoStatus);

            mod.Compile(
                p => p.DistanceToTarget,
                p => p.AmmoStatus);


            // get result
            mod.DeFuzzify(p => p.Desireability, m => m.DeFuzzifyCentroid(15));
            //mod.DeFuzzify(p => p.Desireability, m => m.DeFuzzifyMaxAv());

            //mod.Module.AddRule(new FuzzyOperatorAND(targetFar, ammoLoads), desirable);
            //mod.DeFuzzify(p => p.Desireability, DefuzzifyMethod.CENTROID);
        }
    }

    class Enemy
    {
        public double Health { get; set; }
        public int Age { get; set; }
        public double Hunger { get; set; }
        public double DistanceToTarget { get; set; }
        public int AmmoStatus { get; set; }
        public double Desireability { get; set; }
    }
}
