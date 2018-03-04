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

            var parser = new FuzzyLogicStatementParser(module, map);

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

            var fm = new FuzzyModule();
            var fob = new ObservableFuzzyObject<Enemy>(fm);


            var ps = new FuzzyLogicStatementParser(fm, map);

            // TODO: use reflection to call DefineVariable based on type and decorated properties
            fob.DefineVariable(p => p.DistanceToTarget);
            fob.DefineVariable(p => p.AmmoStatus);
            fob.DefineVariable(p => p.Desirability);
            fob.DefineVariable(p => p.Skill);

            fob.DefineFuzzyTerm("Very_Skilled", p => p.Skill, CreateRightShoulderSet(20, 100, 80))
                .DefineFuzzyTerm("Skilled", p => p.Skill, CreateTriangularSet(10, 30, 20))
                .DefineFuzzyTerm("Low_Skilled", p => p.Skill, CreateLeftShoulderSet(0, 20, 5));

            fob.DefineFuzzyTerm("Ammo_Loads", p => p.AmmoStatus, CreateRightShoulderSet(10, 100, 20))
                .DefineFuzzyTerm("Ammo_Okay", p => p.AmmoStatus, CreateTriangularSet(0, 30, 10))
                .DefineFuzzyTerm("Ammo_Low", p => p.AmmoStatus, CreateTriangularSet(0, 10, 0));

            fob.DefineFuzzyTerm("Undesirable", p => p.Desirability, CreateLeftShoulderSet(0, 25, 50))
                .DefineFuzzyTerm("Desirable", p => p.Desirability, CreateTriangularSet(25, 50, 75))
                .DefineFuzzyTerm("VeryDesirable", p => p.Desirability, CreateRightShoulderSet(50, 75, 100));

            fob.DefineFuzzyTerm("Target_Close", p => p.DistanceToTarget, CreateLeftShoulderSet(0, 150, 25))
                .DefineFuzzyTerm("Target_Medium", p => p.DistanceToTarget, CreateTriangularSet(25, 300, 150))
                .DefineFuzzyTerm("Target_Far", p => p.DistanceToTarget, CreateRightShoulderSet( 150, 1000, 300));

            fob.DefineFuzzyTerm("Undesirable", p => p.Desirability, CreateLeftShoulderSet( 0, 50, 25));
            fob.DefineFuzzyTerm("Desirable", p => p.Desirability, CreateTriangularSet( 25, 75, 50));
            fob.DefineFuzzyTerm("VeryDesirable", p => p.Desirability, CreateRightShoulderSet(50, 100, 75));

            // add a new rule via parsing
            ps.ParseStatement("IF VERY(DistanceToTarget:Target_Far) THEN Desirability:VeryDesirable");

            dynamic modwrapper = fob.GetDynamic();
            fob.AddRule(
                FuzzyOperator.Or(FuzzyOperator.And(modwrapper.Target_Close, modwrapper.Ammo_Low),
                       FuzzyOperator.Or(FuzzyOperator.And(modwrapper.Target_Close, modwrapper.Ammo_Loads),
                       FuzzyOperator.And(modwrapper.Target_Close, modwrapper.Ammo_Okay))
                ),
                modwrapper.Undesirable);


            fob.AddRule(
                FuzzyOperator.And(fob["Target_Medium"], fob["Ammo_Low"]),
                fob["Undesirable"]);


            fob.AddRule(fob.WrapSet("Target_Medium").And(fob["Ammo_Loads"]), fob.WrapSet("Desirable"));
            fob.AddRule(fob.WrapSet("Target_Medium").And(fob["Ammo_Okay"]), fob["VeryDesirable"]);
            fob.AddRule(fob.WrapSet("Target_Medium").And(fob["Ammo_Low"]), fob["Desirable"]);

            fob.AddRule(fob.WrapSet("Target_Far").And(fob["Ammo_Loads"]), fob["Desirable"]);
            fob.AddRule(fob.WrapSet("Target_Far").And(fob["Ammo_Okay"]), fob["Undesirable"]);
            fob.AddRule(fob.WrapSet("Target_Far").And(fob["Ammo_Low"]), fob["Undesirable"]);

            var enemy = fob.Proxy;
            enemy.DistanceToTarget = 12;
            enemy.AmmoStatus = 12;

            // get result
            fob.DeFuzzify(p => p.Desirability, m => m.DeFuzzifyMaxAv());
            Console.WriteLine("First result: {0}", enemy.Desirability);

            enemy.DistanceToTarget = 175;
            enemy.AmmoStatus = 43;

            fob.DeFuzzify(p => p.Desirability, m => m.DeFuzzifyMaxAv());
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
