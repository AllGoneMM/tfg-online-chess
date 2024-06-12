using ChessLibrary.Models.Pieces;

namespace ChessWebApp.Models.DTOs
{
    public class PlayerInfo
    {
        public string ConnectionId { get; set; }
        public string Username { get; set; }
        public string? GroupId { get; set; }
        public PieceTeam Team { get; set; }
    }

}
