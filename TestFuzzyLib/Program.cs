using System;
using System.IO;
using System.Xml;
using FuzzyLib;
using FuzzyLib.Interfaces;
using FuzzyLib.Object;
using FuzzyLib.Observables;
using FuzzyLib.Operators;
using FuzzyLib.Sets;
using Parser;

namespace TestFuzzyLib
{
    class Program
    {
        static void Main(string[] args)
        {

            var module = new FuzzyModule();
            var fo = new ObservableFuzzyObject<Enemy>(module);

            var map = new CharCodeMap();
            map.LoadXml(GetResourceTextFile("CharacterMap.xml"));

            var parser = new FuzzyLogicXMLParser(module, map);

            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(GetResourceTextFile("foo.xml"));

            var xmlLoader = new FuzzyLogicXMLLoader<Enemy>(xmlDocument, parser, fo);

            // set some variables
            var enemyProxy = fo.Proxy;
            enemyProxy.DistanceToTarget = 12;
            enemyProxy.AmmoStatus = 12;
            fo.DeFuzzify(e => e.Desirability, m => m.DeFuzzifyMaxAv() );
            Console.WriteLine("Desirability: {0}", enemyProxy.Desirability);

            enemyProxy.DistanceToTarget = 175;
            enemyProxy.AmmoStatus = 43;
            fo.DeFuzzify(e => e.Desirability, m => m.DeFuzzifyMaxAv());
            Console.WriteLine("Desirability: {0}", enemyProxy.Desirability);


            var mod = new ObservableFuzzyObject<Enemy>(new FuzzyModule());

            // TODO: use reflection to call DefineVariable based on type and decorated properties
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

            var enemy = mod.Proxy;
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
