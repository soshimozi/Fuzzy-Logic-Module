using System.ComponentModel;
using System.Runtime.CompilerServices;
using FuzzyLibUI.Annotations;

namespace FuzzyLibUI
{
    [Description("Editor Options")]
    public class EditorProperties : INotifyPropertyChanged
    {
        private string _fontFamily;

        // ReSharper disable once LocalizableElement
        [DisplayName("Font Family")]
        [DefaultValue("")]
        [Description("The font family of the editor.")]
        [TypeConverter(typeof(FontFamilyConverter))]
        public string FontFamily { get { return _fontFamily;  } set { _fontFamily = value; OnPropertyChanged(); } }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}