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
using FuzzyLib.Variables;
using Observables;
using Observables.Annotations;

namespace TestFuzzyLib
{
    class Program
    {
        static FuzzyModule m_FuzzyModule = new FuzzyModule();

        private static TokenCode token;

        private static TokenType _tokenType;

        private static TextScanner scanner;

        private static readonly TokenCode[] tcUnaryOps
            = {
                TokenCode.Fairly,
                TokenCode.Very
            };

        private static readonly TokenCode[] tcAddOps
            = {
                TokenCode.Or,
            TokenCode.Very,
            TokenCode.Fairly
            };

        private static readonly TokenCode[] tcMulOps
            =
        {
            TokenCode.And
        };

        private static readonly TokenCode[] tcExpressionStart
            =
        {
            TokenCode.LParen,
            TokenCode.Identifier
        };

        private static void GetToken()
        {
            _tokenType = scanner.Get();
            token = _tokenType.Code;
        }
        static void Main(string[] args)
        {

            CharCodeMap map = new CharCodeMap();

            var doc = new XmlDocument();


            map.LoadXml(GetResourceTextFile("CharacterMap.xml"));

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
            mod.DefineVariable(p => p.Desireability);
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
            mod.DeFuzzify(p => p.Desireability, m => m.DeFuzzifyMaxAv());
            Console.WriteLine("First result: {0}", enemy.Desireability);

            enemy.DistanceToTarget = 175;
            enemy.AmmoStatus = 43;

            //mod.Compile(
            //    p => p.DistanceToTarget,
            //    p => p.AmmoStatus);

            mod.DeFuzzify(p => p.Desireability, m => m.DeFuzzifyMaxAv());
            Console.WriteLine("Second result: {0}", enemy.Desireability);


            Console.ReadKey(true);

            TextBuffer buffer = new TextBuffer("IF (Target_Distance:Target_Close AND Ammo:Low_Ammo) OR VERY(Target_Distance:Target_Close AND FAIRLY Ammo:Low_Ammo) OR Ammo:Ammo_Loads THEN Desirability:Desirable");

            scanner = new TextScanner(buffer, map);

            TokenType currentToken = scanner.Get();

            FuzzyManager manager = new FuzzyManager(fuzzyModule);
            manager.DefineVariable("Target_Distance");
            manager.DefineVariable("Ammo");
            manager.DefineVariable("Desirability");

            manager.AddFuzzySet("Low_Ammo", "Ammo", FuzzySet.CreateTriangularSet, 0, 0, 10);
            manager.AddFuzzySet("Target_Close", "Target_Distance", FuzzySet.CreateLeftShoulderSet, 0, 25, 150);
            manager.AddFuzzySet("Desirable", "Desirability", FuzzySet.CreateTriangularSet, 25, 50, 75);
            manager.AddFuzzySet("Ammo_Loads", "Ammo", FuzzySet.CreateRightShoulderSet, 10, 20, 100);

            FuzzyRule rule = Parse(manager);
        }

        private static FuzzyRule Parse(FuzzyManager manager)
        {
            FuzzyTerm antecedent;
            FuzzyTerm consequence;

            if (token == TokenCode.If)
            {
                GetToken();
                antecedent = ParseExpression(manager);

                if (token == TokenCode.Then)
                {
                    GetToken();
                    consequence = ParseExpression(manager);

                    return manager.AddRule(antecedent, consequence);
                }
                else
                    throw new Exception("Missing Then");
            }

            throw new Exception("Missing If");
        }

        private static FuzzyTerm ParseExpression(FuzzyManager manager)
        {
            var resultTerm = ParseSimpleExpression(manager);
            return resultTerm;
        }

        private static FuzzyTerm ParseSimpleExpression(FuzzyManager manager)
        {
            FuzzyTerm resultTerm;
            TokenCode op;

            resultTerm = ParseTerm(manager);

            while (tcAddOps.Any(tc => tc == token))
            {
                op = token;
                GetToken();

                var operandTerm = ParseTerm(manager);
                if (op == TokenCode.Or)
                {
                    resultTerm = manager.Or(resultTerm, operandTerm);
                }
            }

            return resultTerm;
        }

        private static FuzzyTerm ParseTerm(FuzzyManager manager)
        {
            var resultTerm = ParseFactor(manager);

            while (tcMulOps.Any(tc => tc == token))
            {
                var op = token;
                GetToken();
                var operandTerm = ParseFactor(manager);

                if (op == TokenCode.And)
                {
                    resultTerm = manager.And(resultTerm, operandTerm);
                }
            }

            return resultTerm;
        }

        private static FuzzyTerm ParseFactor(FuzzyManager manager)
        {
            FuzzyTerm resultTerm = null;

            switch (token)
            {
                case TokenCode.Identifier:
                    resultTerm = ParseVariable(manager);
                    break;

                case TokenCode.Fairly:
                    GetToken();
                    resultTerm = manager.Fairly(ParseFactor(manager));
                    break;

                case TokenCode.Very:
                    GetToken();
                    resultTerm = manager.Very(ParseFactor(manager));
                    break;

                case TokenCode.LParen:
                    GetToken();
                    resultTerm = ParseExpression(manager);

                    if (token == TokenCode.RParen)
                        GetToken();
                    else
                        throw new Exception("Missing Right Parenthesis");
                    break;

                default:
                    throw new Exception("Invalid Expression");
            }

            return resultTerm;
        }

        private static FuzzyTerm ParseVariable(FuzzyManager manager)
        {
            var variableScope = _tokenType.TokenString;
            
            GetToken();

            if (token != TokenCode.Scope)
            {
                throw new Exception("Invalid Identifer - missing scope operator.");
            }

            GetToken();
            if (token != TokenCode.Identifier)
            {
                throw new Exception("Invalid Identifier - missing set name.");
            }

            var setName = _tokenType.TokenString;

            GetToken();

            var returnTerm = manager.GetFuzzySet(setName, variableScope);
            if (returnTerm == null) throw new Exception("Invalid variable scope or name.");

            return returnTerm;
        }

        public static string GetResourceTextFile(string filename)
        {
            string result = string.Empty;

            using (Stream stream = typeof(Program).Assembly.
                       GetManifestResourceStream("TestFuzzyLib." + filename))
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    result = sr.ReadToEnd();
                }
            }
            return result;
        }
    }
}
