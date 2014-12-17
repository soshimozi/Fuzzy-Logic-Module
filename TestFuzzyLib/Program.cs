using System;
using System.ComponentModel;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Text;
using System.Xml;
using AspectOrientedProgramming;
using FuzzyLib;
using FuzzyLib.Object;
using FuzzyLib.Sets;
using FuzzyLib.Statement;
using Observables;
using Observables.Annotations;

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

            enemy2.DistanceToTarget = 10;
            enemy2.AmmoStatus = 20;

            fo.DeFuzzify(e => e.Desirability, m => m.DeFuzzifyMaxAv() );

            var desirablity = enemy2.Desirability;
            //do
            //{
            //    currentToken = scanner.Get();

            //} while (currentToken.Code != TokenCode.End);

            //scanner.MoveFirst();

            //Console.Write(scanner.CurrentChar());

            //while (scanner.MoveNext() != 0)
            //{
            //    Console.Write(scanner.CurrentChar());
            //}

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


            var fuzzyModule = new FuzzyModule();
            var mod = new ObservableFuzzyObject<Enemy>(enemy, fuzzyModule);

            mod.DefineVariable(p => p.DistanceToTarget);
            mod.DefineVariable(p => p.AmmoStatus);
            mod.DefineVariable(p => p.Desirability);
            mod.DefineVariable(p => p.Skill);

            //_min = min;
            //_max = max;

            //_peakPoint = peak;
            //_leftOffset = (peak - min);
            //_rightOffset = (max - peak);

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

            //FuzzyVariable Desirability = m_FuzzyModule.CreateFLV("Desirability");
            mod.AddFuzzySet("Undesirable", p => p.Desirability, FuzzySet.CreateLeftShoulderSet, 0, 25, 50);
            mod.AddFuzzySet("Desirable", p => p.Desirability, FuzzySet.CreateTriangularSet, 25, 50, 75);
            mod.AddFuzzySet("VeryDesirable", p => p.Desirability, FuzzySet.CreateRightShoulderSet, 50, 75, 100);

            //dynamic modwrapper = mod;

            dynamic modwrapper = mod.GetDynamic();

            mod.AddRule(
                mod.Or(mod.And(modwrapper.Target_Close,modwrapper.Ammo_Low),
                       mod.Or(mod.And(modwrapper.Target_Close, modwrapper.Ammo_Loads),
                       mod.And(modwrapper.Target_Close, modwrapper.Ammo_Okay))
                ),
                modwrapper.Undesirable);

            mod.AddRule(mod.WrapSet("Target_Medium").And(mod["Ammo_Loads"]), mod.WrapSet("Desirable"));
            mod.AddRule(mod.WrapSet("Target_Medium").And(mod["Ammo_Okay"]), mod["VeryDesirable"]);
            mod.AddRule(mod.WrapSet("Target_Medium").And(mod["Ammo_Low"]), mod["Desirable"]);

            mod.AddRule(mod.WrapSet("Target_Far").And(mod["Ammo_Loads"]), mod["Desirable"]);
            mod.AddRule(mod.WrapSet("Target_Far").And(mod["Ammo_Okay"]), mod["Undesirable"]);
            mod.AddRule(mod.WrapSet("Target_Far").And(mod["Ammo_Low"]), mod["Undesirable"]);

            mod.AddRule(mod.WrapSet("Target_Far").Very(), mod["VeryDesirable"]);

            enemy.DistanceToTarget = 12;
            enemy.AmmoStatus = 12;

            //mod.Compile(
            //    p => p.DistanceToTarget,
            //    p => p.AmmoStatus);

            // get result
            mod.DeFuzzify(p => p.Desirability, m => m.DeFuzzifyMaxAv());
            Console.WriteLine("First result: {0}", enemy.Desirability);

            enemy.DistanceToTarget = 175;
            enemy.AmmoStatus = 43;

            //mod.Compile(
            //    p => p.DistanceToTarget,
            //    p => p.AmmoStatus);

            mod.DeFuzzify(p => p.Desirability, m => m.DeFuzzifyMaxAv());
            Console.WriteLine("Second result: {0}", enemy.Desirability);


            Console.ReadKey(true);

            //var buffer = new TextBuffer("IF (Target_Distance:Target_Close AND Ammo:Low_Ammo) OR VERY(Target_Distance:Target_Close AND FAIRLY Ammo:Low_Ammo) OR Ammo:Ammo_Loads THEN Desirability:Desirable");

            //scanner = new TextScanner(buffer, map);

            //var manager = new FuzzyManager(fuzzyModule);

            //manager.DefineVariable("Target_Distance");
            //manager.DefineVariable("Ammo");
            //manager.DefineVariable("Desirability");

            //manager.AddFuzzySet("Low_Ammo", "Ammo", FuzzySet.CreateTriangularSet, 0, 0, 10);
            //manager.AddFuzzySet("Target_Close", "Target_Distance", FuzzySet.CreateLeftShoulderSet, 0, 25, 150);
            //manager.AddFuzzySet("Desirable", "Desirability", FuzzySet.CreateTriangularSet, 25, 50, 75);
            //manager.AddFuzzySet("Ammo_Loads", "Ammo", FuzzySet.CreateRightShoulderSet, 10, 20, 100);

            //var rule = Parse(manager);
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
