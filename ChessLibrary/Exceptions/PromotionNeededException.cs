namespace ChessLibrary.Exceptions
{
    public class PromotionNeededException : Exception
    {
        public PromotionNeededException() : base() { }
        public PromotionNeededException(string message) : base(message) { }
        public PromotionNeededException(string message, Exception inner) : base(message, inner) { }
    }
}
