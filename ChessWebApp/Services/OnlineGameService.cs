using System.Collections.Concurrent;
using ChessLibrary;
using ChessLibrary.Engine.Movement;
using ChessWebApp.Models.DTOs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using ChessLibrary.Engine;
using ChessLibrary.Models.Pieces;

namespace ChessWebApp.Services
{
    public class OnlineGameService : IOnlineGameService
    {
        private readonly ConcurrentDictionary<string, ChessGame> _groupToGame = new ConcurrentDictionary<string, ChessGame>();
        private readonly ConcurrentDictionary<PlayerInfo, string> _playerInfoToGroup = new ConcurrentDictionary<PlayerInfo, string>();
        private readonly ConcurrentDictionary<string, PlayerInfo> _players = new ConcurrentDictionary<string, PlayerInfo>();
        private readonly ConcurrentQueue<PlayerInfo> _waitingPlayers = new ConcurrentQueue<PlayerInfo>();
        private readonly ILogger<OnlineGameService> _logger;

        public OnlineGameService(ILogger<OnlineGameService> logger)
        {
            _logger = logger;
        }

        public async Task<Dictionary<string, (ChessGameResponse, ChessGameInfoResponse)>> JoinQueue(PlayerInfo playerInfo)
        {
            _players.AddOrUpdate(playerInfo.ConnectionId, playerInfo, (_, _) => playerInfo); _waitingPlayers.Enqueue(playerInfo);
            return await TryStartGame();
        }

        private async Task<Dictionary<string, (ChessGameResponse, ChessGameInfoResponse)>> TryStartGame()
        {
            var result = new Dictionary<string, (ChessGameResponse, ChessGameInfoResponse)>();

            if (_waitingPlayers.Count >= 2)
            {
                if (_waitingPlayers.TryDequeue(out var player1Info) && _waitingPlayers.TryDequeue(out var player2Info))
                {
                    var group = Guid.NewGuid().ToString();

                    _playerInfoToGroup.AddOrUpdate(player1Info, group, (_, _) => group);
                    _playerInfoToGroup.AddOrUpdate(player2Info, group, (_, _) => group);

                    _groupToGame.TryRemove(group, out _);
                    var game = _groupToGame.GetOrAdd(group, _ => new ChessGame());

                    player1Info.Team = PieceTeam.WHITE;
                    player2Info.Team = PieceTeam.BLACK;

                    var player1GameInfo = new ChessGameInfoResponse
                    {
                        PlayerInformation = new PlayerInfo() { Team = PieceTeam.WHITE, Username = player1Info.Username },
                        OpponentInformation = new PlayerInfo() { Team = PieceTeam.BLACK, Username = player2Info.Username }
                    };

                    var player2GameInfo = new ChessGameInfoResponse
                    {
                        PlayerInformation = new PlayerInfo() { Team = PieceTeam.BLACK, Username = player2Info.Username },
                        OpponentInformation = new PlayerInfo() { Team = PieceTeam.WHITE, Username = player1Info.Username }
                    };

                    ChessGameResponse player1GameResponse;
                    ChessGameResponse player2GameResponse;

                    player1GameResponse = new ChessGameResponse()
                    {
                        Fen = game.ToString(),
                        State = game.State,
                        Turn = game.Turn,
                        LegalMoves = game.GetAllLegalMoves(player1Info.Team),
                        Promotion = game.Promotion,
                    };
                    player2GameResponse = new ChessGameResponse()
                    {
                        Fen = game.ToString(),
                        State = game.State,
                        Turn = game.Turn,
                        LegalMoves = game.GetAllLegalMoves(player2Info.Team),
                        Promotion = game.Promotion,
                    };

                    result.Add(player1Info.ConnectionId, (player1GameResponse, player1GameInfo));
                    result.Add(player2Info.ConnectionId, (player2GameResponse, player2GameInfo));
                }
            }

            return result;
        }

        public async Task LeaveQueue(string connectionId)
        {
            if (_players.TryGetValue(connectionId, out var playerInfo))
            {
                var newQueue = new ConcurrentQueue<PlayerInfo>();
                while (_waitingPlayers.TryDequeue(out var player))
                {
                    if (playerInfo != player)
                    {
                        newQueue.Enqueue(playerInfo);
                    }
                }
                while (newQueue.TryDequeue(out var player))
                {
                    _waitingPlayers.Enqueue(player);
                }
            }
        }

        public void TryRemoveGame(PlayerInfo playerInfo)
        {
            if (_playerInfoToGroup.TryGetValue(playerInfo, out var group))
            {
                _groupToGame.TryRemove(group, out _);
                _playerInfoToGroup.TryRemove(playerInfo, out _);
                _players.TryRemove(playerInfo.ConnectionId, out _);
            }
        }

        public void TryRemovePlayer(PlayerInfo playerInfo)
        {
            _playerInfoToGroup.TryRemove(playerInfo, out _);
            _players.TryRemove(playerInfo.ConnectionId, out _);
        }

        public (ChessGameResponse, ChessGameResponse) ProcessMove(string connectionId, string move)
        {
            var response = new ChessGameResponse();
            var opponentResponse = new ChessGameResponse();
            if (_players.TryGetValue(connectionId, out var playerInfo))
            {
                if (_playerInfoToGroup.TryGetValue(playerInfo, out var group) &&
                    _groupToGame.TryGetValue(group, out var game))
                {
                    MoveResult moveResult = game.Move(move);
                    response.MoveResult = moveResult;
                    response.Fen = game.ToString();
                    response.State = game.State;
                    response.Turn = game.Turn;
                    response.Promotion = game.Promotion;
                    response.LegalMoves = game.GetAllLegalMoves(playerInfo.Team);

                    if (moveResult.IsSuccessful())
                    {
                        opponentResponse.Fen = game.ToString();
                        opponentResponse.State = game.State;
                        opponentResponse.Turn = game.Turn;
                        opponentResponse.Promotion = game.Promotion;
                        opponentResponse.LegalMoves = game.GetAllLegalMoves();
                    }
                }
            }
            return (response, opponentResponse);
        }

        public async Task LeaveGame(string connectionId)
        {
            if (_players.TryGetValue(connectionId, out var playerInfo))
            {
                if (_playerInfoToGroup.TryGetValue(playerInfo, out var group))
                {
                    _groupToGame.TryRemove(group, out _);
                    _playerInfoToGroup.TryRemove(playerInfo, out _);
                    _players.TryRemove(connectionId, out _);
                }
            }
        }

        public string GetGroup(string connectionId)
        {
            _players.TryGetValue(connectionId, out var playerInfo);
            _playerInfoToGroup.TryGetValue(playerInfo, out var group);
            return group;
        }
        public List<string> GetPlayersInGroup(string group)
        {
            var players = new List<string>();

            foreach (var kvp in _playerInfoToGroup)
            {
                if (kvp.Value == group)
                {
                    players.Add(kvp.Key.ConnectionId);
                }
            }

            return players;
        }

        public (ChessGameResponse, ChessGameResponse) Promote(string connectionId, string pieceChar)
        {
            var response = new ChessGameResponse();
            var opponentResponse = new ChessGameResponse();
            if (_players.TryGetValue(connectionId, out var playerInfo))
            {
                if (_playerInfoToGroup.TryGetValue(playerInfo, out var group) &&
                    _groupToGame.TryGetValue(group, out var game))
                {
                    game.Promote(pieceChar);
                    response.Fen = game.ToString();
                    response.State = game.State;
                    response.Turn = game.Turn;
                    response.Promotion = game.Promotion;
                    response.LegalMoves = game.GetAllLegalMoves(playerInfo.Team);

                    opponentResponse.Fen = game.ToString();
                    opponentResponse.State = game.State;
                    opponentResponse.Turn = game.Turn;
                    opponentResponse.Promotion = game.Promotion;
                    opponentResponse.LegalMoves = game.GetAllLegalMoves();
                }
            }
            return (response, opponentResponse);
        }
    }
}
