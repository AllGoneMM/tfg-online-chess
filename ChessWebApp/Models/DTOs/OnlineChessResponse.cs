using ChessLibrary.Engine.Movement;
using ChessLibrary.Engine;
using ChessLibrary.Models.Pieces;
using ChessLibrary.Models;

namespace ChessWebApp.Models.DTOs
{
    public class OnlineChessResponse
    {
        public string? OpponentUsername { get; set; }
        public string? Username { get; set; }
        public PieceTeam? Team { get; set; }
        public string? Fen { get; set; }
        public MoveResult? MoveResult { get; set; }
        public PieceTeam? Turn { get; set; }
        public State? State { get; set; }
        public Square? Promotion { get; set; }
        public List<Move>? LegalMoves { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
