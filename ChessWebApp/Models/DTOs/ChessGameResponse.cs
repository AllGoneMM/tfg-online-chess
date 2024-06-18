using ChessLibrary.Engine;
using ChessLibrary.Engine.Movement;
using ChessLibrary.Models;
using ChessLibrary.Models.Pieces;

namespace ChessWebApp.Models.DTOs
{
    public class ChessGameResponse
    {
        public string? Fen { get; set; }
        public MoveResult? MoveResult { get; set; }
        public List<string>? MoveHistory { get; set; }
        public PieceTeam? Turn { get; set; }
        public State? State { get; set; }
        public Square? Promotion { get; set; }
        public List<Move>? LegalMoves { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
