namespace ChessLibrary.Exceptions
{
    public class FenFormatException : Exception
    {
        public FenFormatException() : base() { }
        public FenFormatException(string message) : base(message) { }
        public FenFormatException(string message, Exception inner) : base(message, inner) { }
    }
}
