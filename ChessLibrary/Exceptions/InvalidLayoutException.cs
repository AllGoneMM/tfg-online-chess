namespace ChessLibrary.Exceptions
{
    public class InvalidLayoutException : Exception
    {
        public InvalidLayoutException() : base() { }
        public InvalidLayoutException(string message) : base(message) { }
        public InvalidLayoutException(string message, Exception inner) : base(message, inner) { }
    }
}
