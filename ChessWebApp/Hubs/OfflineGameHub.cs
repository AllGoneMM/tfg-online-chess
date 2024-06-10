using ChessLibrary;
using ChessLibrary.Engine.Movement;
using Microsoft.AspNetCore.SignalR;
using Stockfish.NET;
using System.Text.Json;
using ChessWebApp.Services;

namespace ChessWebApp.Hubs
{
    public class OfflineGameHub : Hub
    {
        private readonly GameService _gameService;

        public OfflineGameHub(GameService gameService)
        {
            _gameService = gameService;
        }

        public override Task OnConnectedAsync()
        {
            _gameService.GetOrCreateGame(Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            _gameService.RemoveGame(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }

        public async Task<string> GetStockfishMove()
        {
            IStockfish stockfishAI1 = new Stockfish.NET.Stockfish(@"C:\Users\shady\source\repos\tfg-chess\ChessLibrary.UITests\stockfish.exe");
            ChessGame game = _gameService.GetOrCreateGame(Context.ConnectionId);
            stockfishAI1.SetFenPosition(game.ToString());
            game.Move(stockfishAI1.GetBestMove());
            return game.ToString();
        }
        public async Task<string> StartGame()
        {
            _gameService.RemoveGame(Context.ConnectionId);
            ChessGame game = _gameService.GetOrCreateGame(Context.ConnectionId);
            return JsonSerializer.Serialize(new
            {
                Fen = game.ToString(),
                State = game.State,
                Promotion = game.Promotion
            });
        }

        public async Task<string> GetLegalMoves(string originSquare)
        {
            ChessGame game = _gameService.GetOrCreateGame(Context.ConnectionId);
            game.SelectSquare(originSquare);
            var legalMoves = game.CurrentSquareMoves;
            game.DeselectSquare();
            List<string> legalMovesString = new List<string>();
            foreach (Move move in legalMoves)
            {
                string destinationSquare = move.ToString().Substring(2,2);
                legalMovesString.Add(destinationSquare);
            }

            return JsonSerializer.Serialize(legalMovesString);
        }
        public async Task<string> ProcessMove(string move)
        {
            try
            {
                ChessGame game = _gameService.GetOrCreateGame(Context.ConnectionId);

                MoveResult result = game.Move(move);
                if (result.IsSuccessful())
                {
                    var response = new
                    {
                        Success = true,
                        Fen = game.ToString(),
                        State = game.State,
                        Promotion = game.Promotion
                    };

                    return JsonSerializer.Serialize(response);
                }
                else
                {
                    var response = new 
                    {
                        Success = false,
                        ErrorMessage = "Invalid move"
                    };

                    return JsonSerializer.Serialize(response);
                }
            }
            catch (Exception ex)
            {
                var response = new
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };

                return JsonSerializer.Serialize(response);
            }
        }
    }
}