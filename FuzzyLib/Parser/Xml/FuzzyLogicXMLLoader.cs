using FuzzyLib.Interfaces;
using FuzzyLib.Object.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;

namespace FuzzyLib.Parser.Xml
{
    public class FuzzyLogicXMLLoader<T>
    {
        //private static IParser _parser;
        //private static FuzzyObject<T> _objectToRead;
        //private static Dictionary<string, Type> _shapeTypeMap = new Dictionary<string, Type>();

        public static void LoadObjectFromXml(XmlDocument document, FuzzyObject<T> fuzzyObject)
        {
            var shapeTypeMap = new Dictionary<string, Type>();
            var moduleNode = document.SelectSingleNode("FuzzyModule");
            if (moduleNode == null) return;

            var parser = new FuzzyLogicStatementParser(fuzzyObject.Module);
            ReadShapeReferences(moduleNode, shapeTypeMap);
            ReadVariables(moduleNode, shapeTypeMap, fuzzyObject);
            ParseRules(moduleNode, parser);
        }


        protected FuzzyLogicXMLLoader()
        {

        }
        //protected FuzzyLogicXMLLoader(XmlDocument document, FuzzyObject<T> fuzzyObject)
        //{
        //    _parser = new FuzzyLogicStatementParser(objectToRead.Module);
        //    _objectToRead = objectToRead;
        //    Load(document);
        //}

        //private static void Load(XmlDocument doc)
        //{
        //    var moduleNode = doc.SelectSingleNode("FuzzyModule");

        //    if (moduleNode != null)
        //    {
        //        ReadShapeReferences(moduleNode);

        //        // read variable section first
        //        ReadVariables(moduleNode);

        //        ReadRules(moduleNode);
        //    }
        //}

        private static void ReadShapeReferences(XmlNode moduleNode, Dictionary<string, Type> shapeTypeMap)
        {

            ProcessNodeList<XmlNode>(moduleNode, "FuzzyShapeRefs/ShapeRef", n =>
            {
                var name = GetAttributeValue("name", n);
                var typeName = GetAttributeValue("type", n);
                var shapeType = Type.GetType(typeName);

                shapeTypeMap.Add(name, shapeType);
            });
        }

        private static void ParseRules(XmlNode moduleNode, IParser parser)
        {
            ProcessNodeList<XmlNode>(moduleNode, "FuzzyRules/Rule", n =>
            {
                var ruleText = n.FirstChild.InnerText;
                parser.ParseRule(ruleText);
            });
        }

        private static void ReadVariables(XmlNode doc, Dictionary<string, Type> shapeTypeMap, FuzzyObject<T> fuzzyObject)
        {
            var variables = new List<string>();

            ProcessNodeList<XmlNode>(doc, "FuzzyVariables/FuzzyVariable", n =>
            {
                var name = GetAttributeValue("name", n);
                fuzzyObject.DefineVariable(name);
                variables.Add(name);
            });

            // add sets
            foreach (var variableName in variables)
            {
                var name = variableName;
                ProcessNodeList<XmlNode>(
                    doc,
                    string.Format("FuzzyVariables/FuzzyVariable[@name='{0}']/Terms/Term", variableName),
                    n => ParseTerm(n, name, shapeTypeMap, fuzzyObject));
            }
        }

        private static string GetAttributeValue(string name, XmlNode node)
        {
            if (node.Attributes != null)
                return node.Attributes.Cast<XmlAttribute>()
                    .Where(a => a != null)
                    .Where(a => a.Name == name)
                    .Select(a => a.Value)
                    .FirstOrDefault();

            return null;
        }

        private static void ProcessNodeList<TNodeType>(XmlNode from, string path, Action<TNodeType> action) where TNodeType : XmlNode
        {
            var xmlNodeList = @from.SelectNodes(path);
            if (xmlNodeList == null) return;

            foreach (var node in xmlNodeList.Cast<TNodeType>().Where(n => n != null))
            {
                action(node);
            }
        }

        private static void ParseTerm(XmlNode termNode, string variable, Dictionary<string, Type> shapeTypeMap, FuzzyObject<T> fuzzyObject)
        {
            if (termNode.Attributes == null) return;

            var setName = GetAttributeValue("name", termNode);
            if (setName == null) return;

            var shapeNode = termNode.SelectSingleNode("Shape");
            if (shapeNode == null) return;

            var shapeRef = GetAttributeValue("ref", shapeNode);

            if (!shapeTypeMap.TryGetValue(shapeRef, out Type shapeType))
                return;

            if (shapeType == null) return;

            var shape = MakeShape(shapeNode, shapeType);
            fuzzyObject.DefineFuzzyTermByName(setName, variable, shape);
        }

        private static IFuzzySetManifold MakeShape(XmlNode shapeNode, Type shapeType)
        {
            // make collection of parameters
            var parameters = new List<ParameterInfo>();

            ProcessNodeList<XmlNode>(shapeNode, "parameters/add", n =>
            {
                var name = GetAttributeValue("name", n);
                var value = GetAttributeValue("value", n);
                parameters.Add(new ParameterInfo { Name = name, Value = value });
            });

            if (parameters.Count == 0)
            {
                // no parameters so get default constructor
                var constructor = shapeType.GetConstructor(Type.EmptyTypes);
                if (constructor != null)
                    constructor.Invoke(null);
            }
            else
            {
                var constructor =
                    shapeType.GetConstructors()
                    .FirstOrDefault(c => c.GetParameters().Count() == parameters.Count && c.GetParameters().All(c2 => parameters.Any(p => p.Name == c2.Name)));


                if (constructor == null) return null;

                var constructorParameters = BuildConstructorParameters(constructor, parameters);
                if (typeof(IFuzzySetManifold).IsAssignableFrom(shapeType))
                    return constructor.Invoke(constructorParameters) as IFuzzySetManifold;
            }

            return null;
        }

        private static object[] BuildConstructorParameters(ConstructorInfo constructor, List<ParameterInfo> parameters)
        {
            var retval = new object[parameters.Count];

            var constructorParameters = constructor.GetParameters();
            for (var i = 0; i < constructorParameters.Length; i++)
            {
                var parameterInfo = parameters.FirstOrDefault(p => p.Name == constructorParameters[i].Name);

                retval[i] = Convert.ChangeType(parameterInfo.Value, constructorParameters[i].ParameterType);
            }

            return retval;
        }

        struct ParameterInfo
        {
            public string Name;
            public string Value;
        }
    }
}
