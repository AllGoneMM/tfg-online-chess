using ChessLibrary.Models.Pieces;

namespace ChessWebApp.Models.DTOs
{
    public class ChessGameInfoResponse
    {
        public ChessPlayer OpponentInformation { get; set; }
        public ChessPlayer PlayerInformation { get; set; }
    }
}
