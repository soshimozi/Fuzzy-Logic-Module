using System;
using System.Dynamic;
using System.Text;
using FuzzyLib;

namespace TestFuzzyLib
{
    class Program
    {
        static FuzzyModule m_FuzzyModule = new FuzzyModule();
        static void Main(string[] args)
        {
            //FuzzyModule fm = new FuzzyModule();

            //FuzzyVariable DistToTarget = m_FuzzyModule.CreateFLV("DistToTarget");
            //FuzzySetTerm Target_Close = DistToTarget.AddFuzzySet("Target_Close", FuzzySet.CreateLeftShoulderSet(0, 25, 150));
            //FuzzySetTerm Target_Medium = DistToTarget.AddFuzzySet("Target_Medium", FuzzySet.CreateTriangularSet(25, 150, 300));
            //FuzzySetTerm Target_Far = DistToTarget.AddFuzzySet("Target_Far", FuzzySet.CreateRightShoulderSet(150, 300, 1000));

            //FuzzyVariable Desirability = m_FuzzyModule.CreateFLV("Desirability");
            //FuzzySetTerm Undesirable = Desirability.AddFuzzySet("Undesirable", FuzzySet.CreateLeftShoulderSet(0, 25, 50));
            //FuzzySetTerm Desirable = Desirability.AddFuzzySet("Desirable", FuzzySet.CreateTriangularSet(25, 50, 75));
            //FuzzySetTerm VeryDesirable = Desirability.AddFuzzySet("VeryDesirable", FuzzySet.CreateRightShoulderSet(50, 75, 100));

            //FuzzyVariable AmmoStatus = m_FuzzyModule.CreateFLV("AmmoStatus");
            //FuzzySetTerm Ammo_Loads = AmmoStatus.AddFuzzySet("Ammo_Loads", FuzzySet.CreateRightShoulderSet(10, 30, 100));
            //FuzzySetTerm Ammo_Okay = AmmoStatus.AddFuzzySet("Ammo_Okay", FuzzySet.CreateTriangularSet(0, 10, 30));
            //FuzzySetTerm Ammo_Low = AmmoStatus.AddFuzzySet("Ammo_Low", FuzzySet.CreateTriangularSet(0, 0, 10));

            ////m_FuzzyModule.AddRule(new FuzzyOperatorOr(new FuzzyOperatorAnd(Target_Close, Ammo_Low), new FuzzyOperatorOr(new FuzzyOperatorAnd(Target_Close, Ammo_Loads), new FuzzyOperatorAnd(Target_Close, Ammo_Okay))), Undesirable);
            ////m_FuzzyModule.AddRule(new FuzzyOperatorAND(Target_Close, Ammo_Okay), Undesirable);
            ////m_FuzzyModule.AddRule(new FuzzyOperatorAND(Target_Close, Ammo_Low), Undesirable);

            //m_FuzzyModule.AddRule(new FuzzyOperatorAnd(Target_Medium, Ammo_Loads), VeryDesirable);
            //m_FuzzyModule.AddRule(new FuzzyOperatorAnd(Target_Medium, Ammo_Okay), VeryDesirable);
            //m_FuzzyModule.AddRule(new FuzzyOperatorAnd(Target_Medium, Ammo_Low), Desirable);

            //m_FuzzyModule.AddRule(new FuzzyOperatorAnd(Target_Far, Ammo_Loads), Desirable);
            //m_FuzzyModule.AddRule(new FuzzyOperatorAnd(Target_Far, Ammo_Okay), Undesirable);
            //m_FuzzyModule.AddRule(new FuzzyOperatorAnd(Target_Far, Ammo_Low), Undesirable);

            //Console.Write("Enter distance: ");
            //string distanceString = Console.ReadLine();

            //double distance = 0;
            //double.TryParse(distanceString, out distance);

            //Console.Write("Enter rounds: ");
            //string roundsString = Console.ReadLine();

            //int numRoundsLeft = 0;
            //int.TryParse(roundsString, out numRoundsLeft);

            ////fuzzify distance and amount of ammo
            //m_FuzzyModule.Fuzzify("DistToTarget", distance);
            //m_FuzzyModule.Fuzzify("AmmoStatus", (double)numRoundsLeft);
            //double desirabilityScore = m_FuzzyModule.DeFuzzify("Desirability", p => p.DeFuzzifyMaxAv());

            //Console.WriteLine("Desirability Score: {0}", desirabilityScore);
            //Console.ReadLine();

            //var dynamicProxy = new DynamicProxy<Enemy>(new Enemy());
            //var enemy = dynamicProxy.GetTransparentProxy() as Enemy;

            var enemy = ObservableDynamicProxy<Enemy>.Marshal(new Enemy(), false);

            var mod = new ObservableFuzzyObject<Enemy>(enemy);

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
                .AddFuzzySet("Target_Far", p => p.DistanceToTarget, FuzzySet.CreateRightShoulderSet, 150, 300, 1000);

            //FuzzyVariable Desirability = m_FuzzyModule.CreateFLV("Desirability");
            mod.AddFuzzySet("Undesirable", p => p.Desireability, FuzzySet.CreateLeftShoulderSet, 0, 25, 50);
            mod.AddFuzzySet("Desirable", p => p.Desireability, FuzzySet.CreateTriangularSet, 25, 50, 75);
            mod.AddFuzzySet("VeryDesirable", p => p.Desireability, FuzzySet.CreateRightShoulderSet, 50, 75, 100);

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

            enemy.DistanceToTarget = 20;
            enemy.AmmoStatus = 43;

            mod.Compile(
                p => p.DistanceToTarget,
                p => p.AmmoStatus);

            // get result
            mod.DeFuzzify(p => p.Desireability, m => m.DeFuzzifyMaxAv());
            Console.WriteLine("First result: {0}", enemy.Desireability);

            enemy.DistanceToTarget = 175;
            enemy.AmmoStatus = 43;

            mod.Compile(
                p => p.DistanceToTarget,
                p => p.AmmoStatus);

            mod.DeFuzzify(p => p.Desireability, m => m.DeFuzzifyMaxAv());
            Console.WriteLine("Second result: {0}", enemy.Desireability);


            Console.ReadKey(true);

        }
    }
}
