using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Configuration;
using System.Runtime.Serialization.Formatters;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;

namespace FuzzyLibUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private EditorProperties _editorProperties;
        private CompletionWindow completionWindow;
        private CompletionDataProvider _completionDataProvider;

        public MainWindow()
        {
            // Load our custom highlighting definition
            IHighlightingDefinition customHighlighting;
            using (Stream s = typeof(MainWindow).Assembly.GetManifestResourceStream("FuzzyLibUI.CustomHighlighting.xshd"))
            {
                if (s == null)
                    throw new InvalidOperationException("Could not find embedded resource");
                using (XmlReader reader = new XmlTextReader(s))
                {
                    customHighlighting = ICSharpCode.AvalonEdit.Highlighting.Xshd.
                        HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }

            // and register it in the HighlightingManager
            HighlightingManager.Instance.RegisterHighlighting("Custom Highlighting", new [] { ".cool" }, customHighlighting);

            InitializeComponent();

            _editorProperties = new EditorProperties();
            _editorProperties.PropertyChanged += EditorPropertiesOnPropertyChanged;

            propertyGrid.SelectedObject = _editorProperties;

            _editorProperties.FontFamily = "Consolas";

            Modules = new ObservableCollection<FuzzyModuleModel>();
            Shapes = new ObservableCollection<FuzzyShapeModel>();

            LoadData();

            DataContext = this;

            textEditor.TextArea.TextEntered += TextAreaOnTextEntered;

            _completionDataProvider = new CompletionDataProvider();
        }

        private void TextAreaOnTextEntered(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == ".")
            {
                
            }

            if (e.Text == " ")
            {
                // open code completion after the user has pressed dot:
                completionWindow = new CompletionWindow(textEditor.TextArea);

                var module = Modules.FirstOrDefault();
                if (module != null)
                {
                    _completionDataProvider.SetModel(module);

                    var completionData = _completionDataProvider.GetCompletionData("");
                    foreach (var cp in completionData)
                    {
                        completionWindow.CompletionList.CompletionData.Add(cp);
                    }
                }

                //// provide AvalonEdit with the data:
                //IList<ICompletionData> data = new List<ICompletionData>(); 
                
                //// keywords first
                //data.Add(new CompletionData("IF"));
                //data.Add(new CompletionData("THEN"));
                //data.Add(new CompletionData("AND"));
                //data.Add(new CompletionData("NOT"));
                //data.Add(new CompletionData("OR"));
                //data.Add(new CompletionData("VERY"));
                //data.Add(new CompletionData("FAIRLY"));

                //// get all terms
                //var module = Modules.FirstOrDefault();

                //if (module != null)
                //{
                //    foreach (var variable in module.Variables)
                //    {
                //        foreach (var term in variable.Terms)
                //        {
                //            data.Add(new CompletionData(term.Name));
                //        }
                //    }
                //}

                //var comparison = new Comparison<ICompletionData>((x, y) => String.Compare(x.Text, y.Text, StringComparison.Ordinal));

                //var sortedData = data.ToArray();
                
                //Array.Sort(sortedData, comparison);

                //foreach (var cp in sortedData)
                //{
                //    completionWindow.CompletionList.CompletionData.Add(cp);
                //}

                completionWindow.Show();
                completionWindow.Closed += delegate
                {
                    completionWindow = null;
                };
            }

        }

        //private DataService _dataService = new DataService();

        private void LoadData()
        {
            //var modules = _dataService.LoadModules();
            //foreach (var module in modules)
            //{
            //    Modules.Add(module);
            //}

            //var shapes = _dataService.GetShapes();
            //foreach (var shape in shapes)
            //{
            //    Shapes.Add(shape);
            //}


            //using (var db = new FuzzyLogicEntities())
            //{
            //    var dbshapes = db.FuzzyShapes;

            //    var dbmodules = db.FuzzyModules;

            //    foreach (var dbmodule in dbmodules)
            //    {
            //        var moduleModel = new FuzzyModuleModel {Name = dbmodule.Name, Type = dbmodule.DataType};

            //        foreach (var dbvariable in dbmodule.FuzzyVariables)
            //        {
            //            var variableModel = new FuzzyVariableModel {Name = dbvariable.Name};

            //            foreach (var dbterm in dbvariable.FuzzyTerms)
            //            {
            //                var termModel = new FuzzyTermModel {Name = dbterm.Name, Shape = MapShape(dbterm.FuzzyShape)};
            //                variableModel.Terms.Add(termModel);

            //                foreach (var dbparameter in dbterm.FuzzyTermParameters)
            //                {
            //                    var parameterModel = new FuzzyTermParameterModel {Name = dbparameter.Name, Value = dbparameter.Value};
            //                    termModel.Parameters.Add(parameterModel);
            //                }
            //            }

            //            moduleModel.Variables.Add(variableModel);
            //        }

            //        Modules.Add(moduleModel);
            //    }

            //    foreach (var dbshape in dbshapes)
            //    {
            //        var shape = MapShape(dbshape);
            //        Shapes.Add(shape);
            //    }

            //}
        }

        //private FuzzyShapeModel MapShape(FuzzyShape fuzzyShape)
        //{
        //    var model = new FuzzyShapeModel {Name = fuzzyShape.Name, Type = fuzzyShape.DataType};
        //    return model;
        //}

        public ObservableCollection<FuzzyModuleModel> Modules { get; private set; }
        public ObservableCollection<FuzzyShapeModel> Shapes { get; private set; }

        public IList Children
        {
            get
            {
                return new CompositeCollection
                {
                    new CollectionContainer {Collection = Modules},
                    new CollectionContainer {Collection = Shapes}
                };
            }
        }  

        private void EditorPropertiesOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            switch (propertyChangedEventArgs.PropertyName)
            {
                case "FontFamily":
                    textEditor.FontFamily = new FontFamily(_editorProperties.FontFamily);
                    break;
            }
        }

        private void SaveFileClick(object sender, RoutedEventArgs e)
        {
        }

        private void OpenFileClick(object sender, RoutedEventArgs e)
        {
        }
    }
}
