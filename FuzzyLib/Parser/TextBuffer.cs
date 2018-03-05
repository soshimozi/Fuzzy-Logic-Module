namespace FuzzyLib.Parser
{
    class TextBuffer
    {
        private readonly string _line;
        private int _index;

        public TextBuffer(string line)
        {
            _line = line;
            _index = 0;
        }

        public char CurrentChar()
        {
            if (_index >= (_line.Length - 1)) return (char) 0;
            return _line[_index];
        }

        public char MoveNext()
        {
            if (_index + 1 > (_line.Length - 1)) return (char)0;
            return _line[++_index];
        }

        public void PutBack()
        {
            if (_index > 0) _index--;
        }
    }
}
