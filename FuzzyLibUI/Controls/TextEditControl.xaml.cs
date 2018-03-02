using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FuzzyLibUI.Controls
{
    /// <summary>
    /// Interaction logic for TextEditControl.xaml
    /// </summary>
    public partial class TextEditControl : UserControl
    {
        public TextEditControl()
        {
            InitializeComponent();

            textEditor.TextChanged += TextEditorOnTextChanged;
        }

        private void TextEditorOnTextChanged(object sender, EventArgs eventArgs)
        {
            SetValue(TextProperty, textEditor.Text);
        }

        public string Text
        {
            get
            {
                return (String) this.GetValue(TextProperty);
            }
            set
            {
                SetValue(TextProperty, value);
                textEditor.Text = value;
            }
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof (string),
            typeof (TextEditControl), new PropertyMetadata(""));
    }
}
