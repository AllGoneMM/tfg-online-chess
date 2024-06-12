namespace ChessLibrary.Exceptions
{
    public class IllegalStatusException : Exception
    {
        public IllegalStatusException() : base() { }
        public IllegalStatusException(string message) : base(message) { }
        public IllegalStatusException(string message, Exception inner) : base(message, inner) { }
    }
}
