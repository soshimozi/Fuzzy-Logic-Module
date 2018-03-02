using System;
using System.IO;
using FuzzyLib;
using FuzzyLib.Object;
using FuzzyLib.Operators;
using FuzzyLib.Statement;
using Observables;

namespace TestFuzzyLib
{
    class Program
    {
        static void Main(string[] args)
        {

            var map = new CharCodeMap();
            map.LoadXml(GetResourceTextFile("CharacterMap.xml"));

            var module = new FuzzyModule();
            var parser = new StatementParser(module, map);

            var enemy2 = ObservableDynamicProxy<Enemy>.Marshal(new Enemy(), false);
            var fo = new ObservableFuzzyObject<Enemy>(enemy2, module);
            var xmlLoader = new FuzzyXmlLoader<Enemy>(parser, fo);
            
            xmlLoader.LoadXml(GetResourceTextFile("foo.xml"));

            enemy2.DistanceToTarget = 12;
            enemy2.AmmoStatus = 12;

            // this should be one of the variables specified
            // otherwise an error could result
            fo.DeFuzzify(e => e.Desirability, m => m.DeFuzzifyMaxAv() );
            Console.WriteLine("Desirability: {0}", enemy2.Desirability);

            enemy2.DistanceToTarget = 175;
            enemy2.AmmoStatus = 43;
            fo.DeFuzzify(e => e.Desirability, m => m.DeFuzzifyMaxAv());

            Console.WriteLine("Desirability: {0}", enemy2.Desirability);

            var enemy = ObservableDynamicProxy<Enemy>.Marshal(new Enemy(), false);
            var fuzzyModule = new FuzzyModule();
            var mod = new ObservableFuzzyObject<Enemy>(enemy, fuzzyModule);

            mod.DefineVariable(p => p.DistanceToTarget);
            mod.DefineVariable(p => p.AmmoStatus);
            mod.DefineVariable(p => p.Desirability);
            mod.DefineVariable(p => p.Skill);

            mod.AddFuzzySet("Very_Skilled", p => p.Skill, FuzzySet.CreateRightShoulderSet, 20, 80, 100)
                .AddFuzzySet("Skilled", p => p.Skill, FuzzySet.CreateTriangularSet, 10, 20, 30)
                .AddFuzzySet("Low_Skilled", p => p.Skill, FuzzySet.CreateLeftShoulderSet, 0, 5, 20);

            mod.AddFuzzySet("Ammo_Loads", p => p.AmmoStatus, FuzzySet.CreateRightShoulderSet, 10, 20, 100)
                .AddFuzzySet("Ammo_Okay", p => p.AmmoStatus, FuzzySet.CreateTriangularSet, 0, 10, 30)
                .AddFuzzySet("Ammo_Low", p => p.AmmoStatus, FuzzySet.CreateTriangularSet, 0, 0, 10);

            mod.AddFuzzySet("Undesirable", p => p.Desirability, FuzzySet.CreateLeftShoulderSet, 0, 25, 50)
                .AddFuzzySet("Desirable", p => p.Desirability, FuzzySet.CreateTriangularSet, 25, 50, 75)
                .AddFuzzySet("VeryDesirable", p => p.Desirability, FuzzySet.CreateRightShoulderSet, 50, 75, 100);

            mod.AddFuzzySet("Target_Close", p => p.DistanceToTarget, FuzzySet.CreateLeftShoulderSet, 0, 25, 150)
                .AddFuzzySet("Target_Medium", p => p.DistanceToTarget, FuzzySet.CreateTriangularSet, 25, 150, 300)
                .AddFuzzySet("Target_Far", p => p.DistanceToTarget, FuzzySet.CreateRightShoulderSet, 150, 300, 1000);

            mod.AddFuzzySet("Undesirable", p => p.Desirability, FuzzySet.CreateLeftShoulderSet, 0, 25, 50);
            mod.AddFuzzySet("Desirable", p => p.Desirability, FuzzySet.CreateTriangularSet, 25, 50, 75);
            mod.AddFuzzySet("VeryDesirable", p => p.Desirability, FuzzySet.CreateRightShoulderSet, 50, 75, 100);

            dynamic modwrapper = mod.GetDynamic();
            mod.AddRule(
                FuzzyOperator.Or(FuzzyOperator.And(modwrapper.Target_Close, modwrapper.Ammo_Low),
                       FuzzyOperator.Or(FuzzyOperator.And(modwrapper.Target_Close, modwrapper.Ammo_Loads),
                       FuzzyOperator.And(modwrapper.Target_Close, modwrapper.Ammo_Okay))
                ),
                modwrapper.Undesirable);

            mod.AddRule(mod.WrapSet("Target_Medium").And(mod["Ammo_Loads"]), mod.WrapSet("Desirable"));
            mod.AddRule(mod.WrapSet("Target_Medium").And(mod["Ammo_Okay"]), mod["VeryDesirable"]);
            mod.AddRule(mod.WrapSet("Target_Medium").And(mod["Ammo_Low"]), mod["Desirable"]);

            mod.AddRule(mod.WrapSet("Target_Far").And(mod["Ammo_Loads"]), mod["Desirable"]);
            mod.AddRule(mod.WrapSet("Target_Far").And(mod["Ammo_Okay"]), mod["Undesirable"]);
            mod.AddRule(mod.WrapSet("Target_Far").And(mod["Ammo_Low"]), mod["Undesirable"]);

            enemy.DistanceToTarget = 12;
            enemy.AmmoStatus = 12;

            // get result
            mod.DeFuzzify(p => p.Desirability, m => m.DeFuzzifyMaxAv());
            Console.WriteLine("First result: {0}", enemy.Desirability);

            enemy.DistanceToTarget = 175;
            enemy.AmmoStatus = 43;

            mod.DeFuzzify(p => p.Desirability, m => m.DeFuzzifyMaxAv());
            Console.WriteLine("Second result: {0}", enemy.Desirability);

            Console.ReadKey(true);
        }


        private static string GetResourceTextFile(string filename)
        {
            using (var stream = typeof(Program).Assembly.
                       GetManifestResourceStream("TestFuzzyLib." + filename))
            {
                if (stream == null) return string.Empty;

                using (var sr = new StreamReader(stream))
                {
                    return sr.ReadToEnd();
                }
            }
        }
    }
}
