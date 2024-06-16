using System.Collections.Concurrent;
using System.Text.Json;
using ChessLibrary.Engine.Movement;
using ChessWebApp.Models.DTOs;
using ChessWebApp.Services;
using Microsoft.AspNetCore.SignalR;
using ChessWebApp.Models;

namespace ChessWebApp.Hubs
{
    public class OnlineGameHub : Hub
    {
        public OnlineGameHub(IOnlineGameService gameService, ILogger<OnlineGameHub> logger)
        {
            _gameService = gameService;
            _logger = logger;
        }
        private readonly IOnlineGameService _gameService;
        private readonly ILogger<OnlineGameHub> _logger;


        public override async Task OnConnectedAsync()
        {
            // Cancelar el temporizador y enviar la información del juego al cliente
            var gameRoom = _gameService.GetGameByConnectionId(Context.ConnectionId);
            if (gameRoom != null)
            {
                ChessPlayer? currentPlayer = gameRoom.GetPlayer(Context.ConnectionId);

                if (currentPlayer != null)
                {
                    OnlineChessResponse player1Response = gameRoom.GetResponse(currentPlayer.ConnectionId);

                    string jsonPlayer1Response = JsonSerializer.Serialize(player1Response);

                    await Clients.Client(currentPlayer.ConnectionId).SendAsync("ReceiveResponse", jsonPlayer1Response);
                }
            }


            // En caso de que no sea una reconexión, entrar en la cola
            else
            {
                _logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);

                // Entrar en la cola
                var queuePlayer = new ChessPlayer(Context.ConnectionId);
                if (Context.User != null && Context.User.Identity != null && Context.User.Identity.Name != null)
                {
                    // user is authenticated, access username using Context.User.Identity.Name
                    queuePlayer.Username = Context.User.Identity.Name;
                }

                // Obtener la información del jugador y del oponente y enviarla a ambos
                OnlineChessGame? game = _gameService.JoinQueue(queuePlayer);
                if (game != null)
                {
                    ChessPlayer? currentPlayer = game.GetPlayer(Context.ConnectionId);
                    ChessPlayer? opponent = game.GetOpponent(Context.ConnectionId);

                    if (currentPlayer != null || opponent != null)
                    {
                        OnlineChessResponse player1Response = game.GetResponse(currentPlayer.ConnectionId);
                        OnlineChessResponse player2Response = game.GetResponse(opponent.ConnectionId);

                        string jsonPlayer1Response = JsonSerializer.Serialize(player1Response);
                        string jsonPlayer2Response = JsonSerializer.Serialize(player2Response);

                        await Clients.Client(currentPlayer.ConnectionId).SendAsync("ReceiveInitialResponse", jsonPlayer1Response);
                        await Clients.Client(opponent.ConnectionId).SendAsync("ReceiveInitialResponse", jsonPlayer2Response);
                    }
                }
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // Si el jugador se encuentra en una partida activa, damos por ganador al enemigo
            if (_gameService.GetGameByConnectionId(Context.ConnectionId) != null)
            {
                _logger.LogInformation("Client lost connection: {ConnectionId}", Context.ConnectionId);

                var gameRoom = _gameService.GetGameByConnectionId(Context.ConnectionId);

                if (gameRoom != null)
                {
                    ChessPlayer? disconnectedPlayer = gameRoom.GetPlayer(Context.ConnectionId);
                    ChessPlayer? opponentPlayer = gameRoom.GetOpponent(Context.ConnectionId);
                    if (disconnectedPlayer != null && opponentPlayer != null)
                    {
                        gameRoom.Game.AbortGame(disconnectedPlayer.Team);
                        string opponentPlayerResponseJson = JsonSerializer.Serialize(gameRoom.GetResponse(opponentPlayer.ConnectionId));
                        _gameService.RemoveGame(gameRoom.GameId);
                        await Clients.Client(opponentPlayer.ConnectionId).SendAsync("ReceiveResponse", opponentPlayerResponseJson);
                    }
                }
            }
            else
            {
                _logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);
                _gameService.LeaveQueue(Context.ConnectionId);
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task<string> ProcessMove(string move)
        {
            var gameRoom = _gameService.GetGameByConnectionId(Context.ConnectionId);
            if (gameRoom == null)
            {
                return JsonSerializer.Serialize(new OnlineChessResponse
                {
                    ErrorMessage = "You are not in a game"
                });
            }

            OnlineChessResponse response = gameRoom.GetMoveResponse(Context.ConnectionId, move);
            MoveResult? moveResult = response.MoveResult;

            if (moveResult == null || !moveResult.IsSuccessful())
            {
                return JsonSerializer.Serialize(response);
            }

            if (response.Promotion != null)
            {
                return JsonSerializer.Serialize(response);
            }

            ChessPlayer? currentPlayer = gameRoom.GetPlayer(Context.ConnectionId);
            ChessPlayer? opponentPlayer = gameRoom.GetOpponent(Context.ConnectionId);

            if (currentPlayer == null || opponentPlayer == null)
            {
                return JsonSerializer.Serialize(new OnlineChessResponse
                {
                    ErrorMessage = "Player or opponent not found"
                });
            }

            string currentPlayerResponseJson = JsonSerializer.Serialize(response);
            string opponentPlayerResponseJson = JsonSerializer.Serialize(gameRoom.GetResponse(opponentPlayer.ConnectionId));

            await Clients.Client(opponentPlayer.ConnectionId).SendAsync("ReceiveResponse", opponentPlayerResponseJson);

            return currentPlayerResponseJson;
        }

        public async Task<string> Promote(string pieceChar)
        {
            var gameRoom = _gameService.GetGameByConnectionId(Context.ConnectionId);
            if (gameRoom == null)
            {
                return JsonSerializer.Serialize(new OnlineChessResponse
                {
                    ErrorMessage = "You are not in a game"
                });
            }

            ChessPlayer? currentPlayer = gameRoom.GetPlayer(Context.ConnectionId);
            ChessPlayer? opponentPlayer = gameRoom.GetOpponent(Context.ConnectionId);

            if (currentPlayer == null || opponentPlayer == null)
            {
                return JsonSerializer.Serialize(new OnlineChessResponse
                {
                    ErrorMessage = "Player or opponent not found"
                });
            }

            bool promotionResult = gameRoom.Game.Promote(pieceChar);
            if (!promotionResult)
            {
                return JsonSerializer.Serialize(new OnlineChessResponse
                {
                    ErrorMessage = "No promotion needed"
                });
            }

            string currentPlayerResponseJson = JsonSerializer.Serialize(gameRoom.GetResponse(currentPlayer.ConnectionId));
            string opponentPlayerResponseJson = JsonSerializer.Serialize(gameRoom.GetResponse(opponentPlayer.ConnectionId));

            await Clients.Client(opponentPlayer.ConnectionId).SendAsync("ReceiveResponse", opponentPlayerResponseJson);

            return currentPlayerResponseJson;
        }
    }
}
