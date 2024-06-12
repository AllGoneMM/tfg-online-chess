namespace ChessLibrary.Exceptions
{
    public class IllegalTileException : Exception
    {
        public IllegalTileException() : base() { }
        public IllegalTileException(string message) : base(message) { }
        public IllegalTileException(string message, Exception inner) : base(message, inner) { }
    }
}
