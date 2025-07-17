namespace MiniCompiler.Exceptions;
    public class SyntaxErrorException : Exception
    {
        public SyntaxErrorException(string message) : base(message) { }
    }

    public class LexicalErrorException : Exception
    {
        public LexicalErrorException(string message) : base(message) { }
    }
