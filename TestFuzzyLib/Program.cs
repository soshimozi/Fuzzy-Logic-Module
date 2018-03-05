using System;
using System.IO;
using System.Xml;
using FuzzyLib;
using FuzzyLib.Object;
using FuzzyLib.Operators;
using FuzzyLib.Sets;
using FuzzyLib.Parser.Xml;
using FuzzyLib.Parser;

namespace TestFuzzyLib
{
    class Program
    {
        static void Main(string[] args)
        {
            var module = new FuzzyModule();

            var enemyWrapped = new Enemy();
            var fo = new ObservableFuzzyObject<Enemy>(enemyWrapped, module);

            //var parser = new FuzzyLogicStatementParser(module);

            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(GetResourceTextFile("foo.xml"));

            FuzzyLogicXMLLoader<Enemy>.LoadObjectFromXml(xmlDocument, fo);

            enemyWrapped.DistanceToTarget = 23;

            Console.WriteLine($"fo.distanceToTarget: {fo.Proxy.DistanceToTarget}");

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


            fob.DefineFuzzyTerm("Very_Skilled", p => p.Skill, new RightShoulderFuzzySet(20, 100, 80));
            fob.DefineFuzzyTerm("Skilled", p => p.Skill, new TriangleFuzzySet(10, 30, 20));
            fob.DefineFuzzyTerm("Low_Skilled", p => p.Skill, new TriangleFuzzySet(0, 20, 5));

            fob.DefineFuzzyTerm("Ammo_Loads", p => p.AmmoStatus, new RightShoulderFuzzySet(10, 100, 20));
            fob.DefineFuzzyTerm("Ammo_Okay", p => p.AmmoStatus, new TriangleFuzzySet(0, 30, 10));
            fob.DefineFuzzyTerm("Ammo_Low", p => p.AmmoStatus, new TriangleFuzzySet(0, 10, 0));

            fob.DefineFuzzyTerm("Undesirable", p => p.Desirability, new LeftShoulderFuzzySet(0, 50, 25));
            fob.DefineFuzzyTerm("Desirable", p => p.Desirability, new TriangleFuzzySet(25, 75, 50));
            fob.DefineFuzzyTerm("VeryDesirable", p => p.Desirability, new RightShoulderFuzzySet(50, 100, 75));

            fob.DefineFuzzyTerm("Target_Close", p => p.DistanceToTarget, new LeftShoulderFuzzySet(0, 150, 25));
            fob.DefineFuzzyTerm("Target_Medium", p => p.DistanceToTarget, new TriangleFuzzySet(25, 300, 150));
            fob.DefineFuzzyTerm("Target_Far", p => p.DistanceToTarget, new RightShoulderFuzzySet( 150, 1000, 300));

            fob.DefineFuzzyTerm("Undesirable", p => p.Desirability, new LeftShoulderFuzzySet(0, 50, 25));
            fob.DefineFuzzyTerm("Desirable", p => p.Desirability, new TriangleFuzzySet(25, 75, 50));
            fob.DefineFuzzyTerm("VeryDesirable", p => p.Desirability, new RightShoulderFuzzySet(50, 100, 75));

            // add a new rule via parsing
            //var ps = new FuzzyLogicStatementParser(fm);

            //ps.ParseRule("IF VERY(DistanceToTarget:Target_Far) THEN Desirability:VeryDesirable");

            fm.AddRule("IF VERY(DistanceToTarget:Target_Far) THEN Desirability:VeryDesirable");

            // add a new rule using the building statements
            fm.AddRule(
                FuzzyOperator.Or(FuzzyOperator.And(fob["Target_Close"], fob["Ammo_Low"]),
                       FuzzyOperator.Or(FuzzyOperator.And(fob["Target_Close"], fob["Ammo_Loads"]),
                       FuzzyOperator.And(fob["Target_Close"], fob["Ammo_Okay"]))
                ),
                fob["Undesirable"]);

            // add a new rule using the expression wrappers
            fm.AddRule(fob.If("Target_Medium").And(fob["Ammo_Loads"]), fob["Desirable"]);
            fm.AddRule(fob.If("Target_Medium").And(fob["Ammo_Okay"]), fob["VeryDesirable"]);
            fm.AddRule(fob.If("Target_Medium").And(fob["Ammo_Low"]), fob["Desirable"]);

            fm.AddRule(fob.If("Target_Far").And(fob["Ammo_Loads"]), fob["Desirable"]);
            fm.AddRule(fob.If("Target_Far").And(fob["Ammo_Okay"]), fob["Undesirable"]);
            fm.AddRule(fob.If("Target_Far").And(fob["Ammo_Low"]), fob["Undesirable"]);

            var enemy = fob.Proxy;
            enemy.DistanceToTarget = 12;
            enemy.AmmoStatus = 12;

            // get result, first parameter is property to update, and second parameter is the method to use (average vs centroid)
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

        //public static IFuzzySetManifold CreateTriangularSet(double min, double peak, double max)
        //{
        //    return new TriangleFuzzySet(min, max, peak);
        //}

        //public static IFuzzySetManifold CreateLeftShoulderSet(double min, double peak, double max)
        //{
        //    return new LeftShoulderFuzzySet(min, max, peak);
        //}

        //public static IFuzzySetManifold CreateRightShoulderSet(double min, double peak, double max)
        //{
        //    return new RightShoulderFuzzySet(min, max, peak);
        //}

        //public static IFuzzySetManifold CreateSingletonShoulderSet(double min, double peak, double max)
        //{
        //    return new SingletonFuzzySet(min, max, peak);
        //}
    }
}
