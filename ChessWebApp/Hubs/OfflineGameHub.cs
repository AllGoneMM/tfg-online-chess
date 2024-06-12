using Microsoft.AspNetCore.SignalR;
using System.Text.Json;
using System.Threading.Tasks;
using ChessLibrary.Models.Pieces;
using ChessWebApp.Services;
using ChessWebApp.Models.DTOs;

namespace ChessWebApp.Hubs
{
    public class OfflineGameHub : Hub
    {
        private readonly IOfflineGameService _gameService;
        private readonly ILogger<OfflineGameHub> _logger;

        public OfflineGameHub(IOfflineGameService gameService, ILogger<OfflineGameHub> logger)
        {
            _gameService = gameService;
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            // Handle the connection
            _logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _gameService.TryRemoveGame(Context.ConnectionId);
            _gameService.TryRemovePlayer(Context.ConnectionId);
            _logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task<string> StartGame(string color)
        {
            ChessGameResponse response = new ChessGameResponse();
            
            if(color != "white" && color != "black")
            {
                response.ErrorMessage = "Invalid team";
                return JsonSerializer.Serialize(response);
            }

            PieceTeam team = color == "white" ? PieceTeam.WHITE : PieceTeam.BLACK;
            PlayerInfo playerInfo = new PlayerInfo();
            playerInfo.ConnectionId = Context.ConnectionId;
            playerInfo.Team = team;
            if(Context.User.Identity.IsAuthenticated)
            {
                playerInfo.Username = Context.User.Identity.Name;
            }
            else
            {
                playerInfo.Username = "Guest";
            }

            response = _gameService.StartGame(playerInfo);
            return JsonSerializer.Serialize(response);
        }

        public async Task<string> ProcessMove(string move)
        {
            ChessGameResponse response = _gameService.ProcessMove(Context.ConnectionId, move);
            return JsonSerializer.Serialize(response);
        }

        public async Task GetStockfishMove()
        {
            ChessGameResponse response = _gameService.GetStockfishMove(Context.ConnectionId);
            string responseJson = JsonSerializer.Serialize(response);

            // Invocar método en el cliente
            await Clients.Caller.SendAsync("ReceiveStockfishMove", responseJson);
        }
    }
}
