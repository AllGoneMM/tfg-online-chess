using ChessLibrary;
using ChessLibrary.Engine.Movement;
using ChessLibrary.Models.Pieces;
using ChessWebApp.Models.DTOs;

namespace ChessWebApp.Models
{
    public class OnlineChessGame(ChessPlayer player1, ChessPlayer player2)
    {
        public string GameId { get; } = Guid.NewGuid().ToString();
        public ChessGame Game { get; set; } = new ChessGame();
        public ChessPlayer Player1 { get; set; } = player1;
        public ChessPlayer Player2 { get; set; } = player2;

        public OnlineChessResponse GetResponse(string connectionId)
        {
            ChessPlayer? player = GetPlayer(connectionId);
            ChessPlayer? opponent = GetOpponent(connectionId);

            if(player == null || opponent == null)
            {
                return new OnlineChessResponse
                {
                    ErrorMessage = "Player or opponent not found"
                };
            }

            var response = new OnlineChessResponse
            {
                Fen = Game.ToString(),
                Turn = Game.Turn,
                State = Game.State,
                LegalMoves = Game.GetAllLegalMoves(player.Team),
                Promotion = Game.Promotion,
                OpponentUsername = opponent.Username,
                Username = player.Username,
                Team = player.Team,
                MoveHistory = Game.MoveHistory
            };

            return response;
        }

        public OnlineChessResponse GetMoveResponse(string connectionId, string move)
        {
            ChessPlayer? player = GetPlayer(connectionId);
            ChessPlayer? opponent = GetOpponent(connectionId);

            if(player == null || opponent == null)
            {
                return new OnlineChessResponse
                {
                    ErrorMessage = "Player or opponent not found"
                };
            }

            if(player.Team != Game.Turn)
            {
                return new OnlineChessResponse
                {
                    ErrorMessage = "It's not your turn"
                };
            }
            

            MoveResult moveResult = Game.Move(move);
            var response = new OnlineChessResponse
            {
                Fen = Game.ToString(),
                Turn = Game.Turn,
                State = Game.State,
                LegalMoves = Game.GetAllLegalMoves(player.Team),
                Promotion = Game.Promotion,
                MoveResult = moveResult,
                OpponentUsername = opponent.Username,
                Username = player.Username,
                Team = player.Team,
                MoveHistory = Game.MoveHistory
            };

            return response;
        }

        public ChessPlayer? GetPlayer(string connectionId)
        {
            if (connectionId == Player1.ConnectionId)
            {
                return Player1;
            }
            else if (connectionId == Player2.ConnectionId)
            {
                return Player2;
            }
            else
            {
                return null;
            }
        }

        public ChessPlayer? GetOpponent(string connectionId)
        {
            if (connectionId == Player1.ConnectionId)
            {
                return Player2;
            }
            else if (connectionId == Player2.ConnectionId)
            {
                return Player1;
            }
            else
            {
                return null;
            }
        }
    }
}
