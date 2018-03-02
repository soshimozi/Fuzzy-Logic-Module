using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace FuzzyLibUI
{
    public class CompletionData : ICompletionData
    {
        public CompletionData(string text)
        {
            Text = text;
        }

        public ImageSource Image
        {
            get { return null; }
        }

        public string Text { get; private set; }

        // Use this property if you want to show a fancy UIElement in the drop down list.
        public object Content
        {
            get { return Text; }
        }

        public object Description
        {
            get { return "Description for " + Text; }
        }

        public double Priority { get { return 0; } }

        public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
        {
            textArea.Document.Replace(completionSegment, Text);
        }
    }
}
