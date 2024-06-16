using System.Collections.Concurrent;
using ChessLibrary;
using ChessLibrary.Engine.Movement;
using ChessWebApp.Models.DTOs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using ChessLibrary.Engine;
using ChessLibrary.Models.Pieces;
using ChessWebApp.Models;

namespace ChessWebApp.Services
{

    public class OnlineGameService(ILogger<OnlineGameService> logger, IHttpContextAccessor accessor) : IOnlineGameService
    {
        private readonly ConcurrentDictionary<string, OnlineChessGame> _gameIdToOnlineChessGames = new ConcurrentDictionary<string, OnlineChessGame>();
        private readonly ConcurrentDictionary<string, string> _connectionIdToGameId = new ConcurrentDictionary<string, string>();
        private readonly ConcurrentQueue<ChessPlayer> _queue = new ConcurrentQueue<ChessPlayer>();
        private readonly ILogger<OnlineGameService> _logger = logger;

        public OnlineChessGame? JoinQueue(ChessPlayer player)
        {
            _queue.Enqueue(player);
            if (_queue.Count >= 2)
            {
                if (_queue.TryDequeue(out var player1) && _queue.TryDequeue(out var player2))
                {

                    Random random = new Random();
                    if (random.Next(0, 2) == 0)
                    {
                        player1.Team = PieceTeam.WHITE;
                        player2.Team = PieceTeam.BLACK;
                    }
                    else
                    {
                        player1.Team = PieceTeam.BLACK;
                        player2.Team = PieceTeam.WHITE;
                    }

                    var game = new OnlineChessGame(player1, player2);
                    AddGame(game);
                    return game;
                }
            }

            return null;
        }

        public void LeaveQueue(string connectionId)
        {
            var newQueue = new ConcurrentQueue<ChessPlayer>(_queue.Where(player => player.ConnectionId != connectionId));

            // Reemplazar la cola original con la nueva cola
            while (_queue.TryDequeue(out _)) { }
            foreach (var player in newQueue)
            {
                _queue.Enqueue(player);
            }
        }

        public OnlineChessGame? GetGameByConnectionId(string connectionId)
        {
            if (_connectionIdToGameId.TryGetValue(connectionId, out var gameId))
            {
                if (_gameIdToOnlineChessGames.TryGetValue(gameId, out var game))
                {
                    return game;
                }
            }
            return null;
        }

        private bool AddGame(OnlineChessGame game)
        {
            bool addedToGameDict = _gameIdToOnlineChessGames.TryAdd(game.GameId, game);
            bool addedPlayer1 = _connectionIdToGameId.TryAdd(game.Player1.ConnectionId, game.GameId);
            bool addedPlayer2 = _connectionIdToGameId.TryAdd(game.Player2.ConnectionId, game.GameId);

            // Si cualquiera de las adiciones falla, se revierte todo
            if (!addedToGameDict || !addedPlayer1 || !addedPlayer2)
            {
                // Revertir cualquier operación exitosa
                if (addedToGameDict)
                {
                    _gameIdToOnlineChessGames.TryRemove(game.GameId, out _);
                }
                if (addedPlayer1)
                {
                    _connectionIdToGameId.TryRemove(game.Player1.ConnectionId, out _);
                }
                if (addedPlayer2)
                {
                    _connectionIdToGameId.TryRemove(game.Player2.ConnectionId, out _);
                }
                return false;
            }

            return true;
        }

        public bool RemoveGame(string gameId)
        {
            if (_gameIdToOnlineChessGames.TryRemove(gameId, out var game))
            {
                _connectionIdToGameId.TryRemove(game.Player1.ConnectionId, out _);
                _connectionIdToGameId.TryRemove(game.Player2.ConnectionId, out _);
                return true;
            }

            return false;
        }

        public void SuspendGame(string connectionId)
        {
            var game = GetGameByConnectionId(connectionId);
            if (game != null)
            {
                if(game.Player1.ConnectionId == connectionId)
                {
                    game.Game.AbortGame((PieceTeam)game.Player1.Team);
                }
                else if(game.Player2.ConnectionId == connectionId)
                {
                    game.Game.AbortGame((PieceTeam)game.Player2.Team);

                }
            }
        }

    }
}
