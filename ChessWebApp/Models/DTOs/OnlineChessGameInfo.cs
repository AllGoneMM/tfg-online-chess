using System.Text.Json;
using ChessLibrary.Engine.Movement;
using ChessLibrary.Engine;
using ChessLibrary.Models.Pieces;
using ChessLibrary.Models;
using ChessLibrary.Utils;

namespace ChessWebApp.Models.DTOs
{
    public class OnlineChessGameInfo(string connectionId)
    {
        public readonly string ConnectionId = connectionId;
        public string? OpponentUsername;
        public string? Username;
        public string? Fen;
        public PieceTeam? Turn;
        public State? State;
        public List<Move> LegalMoves { get; set; } = new List<Move>();
        public MoveResult? MoveResult { get; set; }
        public Square? Promotion { get; set; }
        public string? ErrorMessage { get; set; }
        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
