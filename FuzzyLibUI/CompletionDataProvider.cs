using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.AvalonEdit.CodeCompletion;

namespace FuzzyLibUI
{
    public class CompletionDataProvider
    {
        private readonly List<CompletionData> _keywords = new List<CompletionData>();
        private FuzzyModuleModel _model;

        readonly Comparison<ICompletionData> _comparison = ((x, y) => String.Compare(x.Text, y.Text, StringComparison.Ordinal));

        public CompletionDataProvider()
        {
            _keywords.Add(new CompletionData("IF"));
            _keywords.Add(new CompletionData("THEN"));
            _keywords.Add(new CompletionData("AND"));
            _keywords.Add(new CompletionData("NOT"));
            _keywords.Add(new CompletionData("OR"));
            _keywords.Add(new CompletionData("VERY"));
            _keywords.Add(new CompletionData("FAIRLY"));

        }

        public void SetModel(FuzzyModuleModel model)
        {
            _model = model;
        }

        public IList<ICompletionData> GetCompletionData(string scope)
        {
            var data = new List<ICompletionData>();

            // keywords first
            data.AddRange(_keywords);

            if (_model == null) return SortCollection(data.ToArray());

            if (scope == "")
            {
                // add module terms
                foreach (var variable in _model.Variables)
                {
                    data.AddRange(variable.Terms.Select(term => new CompletionData(term.Name)));
                }
            }
            else
            {
                // word should be a variable name

                // then we show terms
            }

            return SortCollection(data.ToArray());
        }

        private IList<ICompletionData> SortCollection(ICompletionData [] data)
        {
            Array.Sort(data, _comparison);

            var sortedList = new List<ICompletionData>();

            sortedList.AddRange(data.ToList());

            return sortedList;
        }
    }
}
