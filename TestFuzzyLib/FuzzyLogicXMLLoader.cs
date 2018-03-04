﻿using FuzzyLib.Interfaces;
using FuzzyLib.Object.Generic;
using Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TestFuzzyLib
{
    public class FuzzyLogicXMLLoader<T>
    {
        private readonly IParser _parser;
        private readonly FuzzyObject<T> _objectToRead;
        private readonly Dictionary<string, Type> _typeMap = new Dictionary<string, Type>();

        public FuzzyLogicXMLLoader(XmlDocument document, IParser parser, FuzzyObject<T> objectToRead)
        {
            _parser = parser;
            _objectToRead = objectToRead;
            Load(document);
        }

        private void Load(XmlDocument doc)
        {
            var moduleNode = doc.SelectSingleNode("FuzzyModule");

            if (moduleNode != null)
            {
                ReadShapeReferences(moduleNode);

                // read variable section first
                ReadVariables(moduleNode);

                ReadRules(moduleNode);
            }
        }

        private void ReadShapeReferences(XmlNode moduleNode)
        {

            ProcessNodeList<XmlNode>(moduleNode, "FuzzyShapeRefs/ShapeRef", n =>
            {
                var name = GetAttributeValue("name", n);
                var typeName = GetAttributeValue("type", n);
                var shapeType = Type.GetType(typeName);

                _typeMap.Add(name, shapeType);
            });
        }

        private void ReadRules(XmlNode moduleNode)
        {
            ProcessNodeList<XmlNode>(moduleNode, "FuzzyRules/Rule", n =>
            {
                var ruleText = n.FirstChild.InnerText;
                _parser.ParseRule(ruleText);
            });
        }

        private void ReadVariables(XmlNode doc)
        {
            var variables = new List<string>();

            ProcessNodeList<XmlNode>(doc, "FuzzyVariables/FuzzyVariable", n =>
            {
                var name = GetAttributeValue("name", n);
                _objectToRead.DefineVariable(name);
                variables.Add(name);
            });

            // add sets
            foreach (var variableName in variables)
            {
                var name = variableName;
                ProcessNodeList<XmlNode>(
                    doc,
                    string.Format("FuzzyVariables/FuzzyVariable[@name='{0}']/Terms/Term", variableName),
                    n => ParseTerm(name, n));
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

        private void ParseTerm(string variable, XmlNode termNode)
        {
            if (termNode.Attributes == null) return;

            var setName = GetAttributeValue("name", termNode);
            if (setName == null) return;

            var shapeNode = termNode.SelectSingleNode("Shape");
            if (shapeNode == null) return;

            var shapeRef = GetAttributeValue("ref", shapeNode);

            if (!_typeMap.TryGetValue(shapeRef, out Type shapeType))
                return;

            if (shapeType == null) return;

            //var typeName = GetAttributeValue("type", shapeNode);
            //if (typeName == null) return;

            //var shapeType = Type.GetType(typeName);
            //if (shapeType == null) return;

            var shape = MakeShape(shapeNode, shapeType);
            _objectToRead.DefineFuzzyTermByName(setName, variable, shape);
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
