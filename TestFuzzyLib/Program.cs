using System;
using System.IO;
using FuzzyLib;
using FuzzyLib.Object;
using FuzzyLib.Operators;
using FuzzyLib.Sets;
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


            mod.AddFuzzySet("Very_Skilled", (p) => p.Skill, CreateRightShoulderSet(20, 100, 80))
                .AddFuzzySet("Skilled", p => p.Skill, CreateTriangularSet(10, 30, 20))
                .AddFuzzySet("Low_Skilled", p => p.Skill, CreateLeftShoulderSet(0, 20, 5));

            mod.AddFuzzySet("Ammo_Loads", p => p.AmmoStatus, CreateRightShoulderSet(10, 100, 20))
                .AddFuzzySet("Ammo_Okay", p => p.AmmoStatus, CreateTriangularSet(0, 30, 10))
                .AddFuzzySet("Ammo_Low", p => p.AmmoStatus, CreateTriangularSet(0, 10, 0));

            mod.AddFuzzySet("Undesirable", p => p.Desirability, CreateLeftShoulderSet(0, 25, 50))
                .AddFuzzySet("Desirable", p => p.Desirability, CreateTriangularSet(25, 50, 75))
                .AddFuzzySet("VeryDesirable", p => p.Desirability, CreateRightShoulderSet(50, 75, 100));

            mod.AddFuzzySet("Target_Close", p => p.DistanceToTarget, CreateLeftShoulderSet(0, 150, 25))
                .AddFuzzySet("Target_Medium", p => p.DistanceToTarget, CreateTriangularSet(25, 300, 150))
                .AddFuzzySet("Target_Far", p => p.DistanceToTarget, CreateRightShoulderSet( 150, 1000, 300));

            mod.AddFuzzySet("Undesirable", p => p.Desirability, CreateLeftShoulderSet( 0, 50, 25));
            mod.AddFuzzySet("Desirable", p => p.Desirability, CreateTriangularSet( 25, 75, 50));
            mod.AddFuzzySet("VeryDesirable", p => p.Desirability, CreateRightShoulderSet(50, 100, 75));

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

        public static IFuzzySet CreateTriangularSet(double min, double peak, double max)
        {
            return new TriangleFuzzySet(min, max, peak);
        }

        public static IFuzzySet CreateLeftShoulderSet(double min, double peak, double max)
        {
            return new LeftShoulderFuzzySet(min, max, peak);
        }

        public static IFuzzySet CreateRightShoulderSet(double min, double peak, double max)
        {
            return new RightShoulderFuzzySet(min, max, peak);
        }

        public static IFuzzySet CreateSingletonShoulderSet(double min, double peak, double max)
        {
            return new SingletonFuzzySet(min, max, peak);
        }
    }
}
