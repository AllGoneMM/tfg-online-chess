using ChessLibrary.Models.Pieces;

namespace ChessWebApp.Models.DTOs
{
    public class ChessGameInfoResponse
    {
        public PlayerInfo OpponentInformation { get; set; }
        public PlayerInfo PlayerInformation { get; set; }
    }
}
