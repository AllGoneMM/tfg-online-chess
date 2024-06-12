namespace ChessLibrary.Exceptions
{
    public class NoLegalMovesException : Exception
    {
        public NoLegalMovesException()
        {
        }

        public NoLegalMovesException(string message) : base(message)
        {
        }

        public NoLegalMovesException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
