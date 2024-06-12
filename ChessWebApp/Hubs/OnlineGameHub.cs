using System.Text.Json;
using System.Threading.Tasks;
using ChessWebApp.Models.DTOs;
using ChessWebApp.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace ChessWebApp.Hubs
{
    public class OnlineGameHub : Hub
    {
        private readonly IOnlineGameService _onlineGameService;
        private readonly ILogger<OnlineGameHub> _logger;

        public OnlineGameHub(IOnlineGameService onlineGameService, ILogger<OnlineGameHub> logger)
        {
            _onlineGameService = onlineGameService;
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);
            await JoinQueue();
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await _onlineGameService.LeaveGame(Context.ConnectionId);
            _logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        private async Task JoinQueue()
        {
            PlayerInfo playerInfo = new PlayerInfo
            {
                ConnectionId = Context.ConnectionId
            };

            if (Context.User.Identity.IsAuthenticated)
            {
                playerInfo.Username = Context.User.Identity.Name;
            }
            else
            {
                playerInfo.Username = "Guest";
            }

            var dictionary = await _onlineGameService.JoinQueue(playerInfo);

            if (dictionary.Count == 2)
            {
                foreach (var kvp in dictionary)
                {
                    var key = kvp.Key;
                    var value = kvp.Value;

                    if (value.Item1 != null && value.Item2 != null)
                    {
                        var gameInfoJson = JsonSerializer.Serialize(value.Item2);
                        var gameResponseJson = JsonSerializer.Serialize(value.Item1);
                        await Clients.Client(kvp.Key).SendAsync("ReceiveGameInfo", gameResponseJson, gameInfoJson);
                    }
                }
            }
        }

        public async Task LeaveQueue()
        {
            await _onlineGameService.LeaveQueue(Context.ConnectionId);
        }

        public async Task LeaveGame()
        {
            await _onlineGameService.LeaveGame(Context.ConnectionId);
        }

        public async Task<string> ProcessMove(string move)
        {
            var response = _onlineGameService.ProcessMove(Context.ConnectionId, move);
            if (response.Item1.MoveResult != null && response.Item1.MoveResult.IsSuccessful())
            {
                // Retrieve the group the player belongs to
                string group = _onlineGameService.GetGroup(Context.ConnectionId);

                if (!string.IsNullOrEmpty(group))
                {
                    // Get the opponent's connection ID
                    var playersInGroup = _onlineGameService.GetPlayersInGroup(group);
                    var opponentConnectionId = playersInGroup.FirstOrDefault(id => id != Context.ConnectionId);

                    if (opponentConnectionId != null)
                    {
                        string opponentResponseJson = JsonSerializer.Serialize(response.Item2);
                        // Send the move to the opponent
                        await Clients.Client(opponentConnectionId).SendAsync("ReceiveOpponentPlayerMove", opponentResponseJson);
                    }
                }
            }

            string responseJson = JsonSerializer.Serialize(response.Item1);
            return responseJson;
        }

    }
}
