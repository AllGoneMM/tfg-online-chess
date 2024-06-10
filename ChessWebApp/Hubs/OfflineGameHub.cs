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
        public async Task<string> ProcessMove(string move)
        {
            ChessGame game = _gameService.GetOrCreateGame(Context.ConnectionId);

            MoveResult result = game.Move(move);
            if (result.IsSuccessful())
            {
                IStockfish stockfishAI1 = new Stockfish.NET.Stockfish(@"C:\Users\Mykyta\source\repos\tfg-chess\ChessLibrary.UITests\stockfish.exe");
                stockfishAI1.SetFenPosition(game.ToString());
                game.Move(stockfishAI1.GetBestMove());
            }

            var gameJson = new
            {
                Fen = game.ToString(),
                State = game.State,
                Promotion = game.Promotion
            };

            return JsonSerializer.Serialize(gameJson);
        }
    }
}