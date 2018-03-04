using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuzzyLibUI
{
    public class DataService
    {
        public IList<FuzzyModuleModel> LoadModules()
        {
            var modules = new List<FuzzyModuleModel>();

            using (var db = new FuzzyLogicEntities())
            {
                var modles = db.FuzzyModules.ToList();

                modules.AddRange(db.FuzzyModules.ToList().Select(LoadModule));

                //var dbmodules = db.FuzzyModules;

                //foreach (var dbmodule in dbmodules)
                //{
                //    var moduleModel = LoadModule(dbmodule); // new FuzzyModuleModel {Name = dbmodule.Name, Type = dbmodule.DataType};

                //    foreach (var dbvariable in dbmodule.FuzzyVariables)
                //    {
                //        var variableModel = new FuzzyVariableModel {Name = dbvariable.Name};

                //        foreach (var dbterm in dbvariable.FuzzyTerms)
                //        {
                //            var termModel = new FuzzyTermModel {Name = dbterm.Name, Shape = MapShape(dbterm.FuzzyShape)};
                //            variableModel.Terms.Add(termModel);

                //            foreach (var dbparameter in dbterm.FuzzyTermParameters)
                //            {
                //                var parameterModel = new FuzzyTermParameterModel
                //                {
                //                    Name = dbparameter.Name,
                //                    Value = dbparameter.Value
                //                };
                //                termModel.Parameters.Add(parameterModel);
                //            }
                //        }

                //        moduleModel.Variables.Add(variableModel);
                //    }

                //modules.Add(moduleModel);
            }

            return modules;
        }

        private FuzzyModuleModel LoadModule(FuzzyModule module)
        {
            var model = new FuzzyModuleModel { Name = module.Name, Type = module.DataType };

            LoadVariables(model, module);
            return model;
        }

        private void LoadVariables(FuzzyModuleModel model, FuzzyModule module)
        {
            foreach (var dbvariable in module.FuzzyVariables)
            {
                var variableModel = new FuzzyVariableModel { Name = dbvariable.Name };
                model.Variables.Add(variableModel);

                LoadTerms(variableModel, dbvariable);
            }
        }

        private void LoadTerms(FuzzyVariableModel variableModel, FuzzyVariable dbvariable)
        {
            foreach (var dbterm in dbvariable.FuzzyTerms)
            {
                var termModel = new FuzzyTermModel { Name = dbterm.Name, Shape = MapShape(dbterm.FuzzyShape) };
                variableModel.Terms.Add(termModel);

                LoadTermParameters(termModel, dbterm);
            }
        }

        private void LoadTermParameters(FuzzyTermModel termModel, FuzzyTerm dbterm)
        {
            foreach (var dbparameter in dbterm.FuzzyTermParameters)
            {
                var parameterModel = new FuzzyTermParameterModel
                {
                    Name = dbparameter.Name,
                    Value = dbparameter.Value
                };

                termModel.Parameters.Add(parameterModel);
            }

        }

        public IList<FuzzyShapeModel> GetShapes()
        {
            var shapes = new List<FuzzyShapeModel>();

            using (var db = new FuzzyLogicEntities())
            {
                shapes.AddRange(db.FuzzyShapes.ToList().Select(MapShape));
            }

            return shapes;
        }

        private FuzzyShapeModel MapShape(FuzzyShape fuzzyShape)
        {
            var model = new FuzzyShapeModel {Name = fuzzyShape.Name, Type = fuzzyShape.DataType};
            return model;
        }


    }
}
