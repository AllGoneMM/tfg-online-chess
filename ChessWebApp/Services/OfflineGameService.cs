using System.Collections.Concurrent;
using ChessLibrary;
using System.Text.Json;
using ChessLibrary.Engine;
using ChessLibrary.Engine.Movement;
using ChessWebApp.Models.DTOs;
using Stockfish.NET;

namespace ChessWebApp.Services
{
    public class OfflineGameService : IOfflineGameService
    {
        private readonly ConcurrentDictionary<string, ChessGame> _games;
        private readonly ConcurrentDictionary<string, ChessPlayer> _players;
        private readonly ILogger<OfflineGameService> _logger;
        private readonly string _stockfishPath;

        public OfflineGameService(ILogger<OfflineGameService> logger, IConfiguration configuration)
        {
            _games = new ConcurrentDictionary<string, ChessGame>();
            _players = new ConcurrentDictionary<string, ChessPlayer>();
            _logger = logger;
            _stockfishPath = configuration["StockfishPath"];
        }

        public void TryRemoveGame(string connectionId)
        {
            if (_games.TryRemove(connectionId, out _))
            {
                _logger.LogInformation("Game removed for connection {ConnectionId}", connectionId);
            }
            else
            {
                _logger.LogWarning("Attempted to remove non-existing game for connection {ConnectionId}", connectionId);
            }
        }
        public void TryRemovePlayer(string connectionId)
        {
            if (_players.TryRemove(connectionId, out _))
            {
                _logger.LogInformation("Player removed for connection {ConnectionId}", connectionId);
            }
            else
            {
                _logger.LogWarning("Attempted to remove non-existing player for connection {ConnectionId}", connectionId);
            }
        }

        public ChessGameResponse StartGame(ChessPlayer playerInfo)
        {
            _players.AddOrUpdate(playerInfo.ConnectionId, playerInfo, (_, _) => playerInfo);
            TryRemoveGame(playerInfo.ConnectionId);

            _logger.LogInformation("New game started for connection {ConnectionId}", playerInfo.ConnectionId);
            ChessGame game = _games.GetOrAdd(playerInfo.ConnectionId, _ => new ChessGame());
            ChessGameResponse response = new ChessGameResponse
            {
                Fen = game.ToString(),
                State = game.State,
                Turn = game.Turn,
                MoveHistory = game.MoveHistory
            };
            if(game.Turn == playerInfo.Team)
            {
                response.LegalMoves = game.GetAllLegalMoves();
                return response;
            }
            else
            {
                return GetStockfishMove(playerInfo.ConnectionId);
            }
        }

        public ChessGameResponse ProcessMove(string connectionId, string move)
        {
            _logger.LogInformation("Processing move {Move} for connection {ConnectionId}", move, connectionId);
            ChessGameResponse response = new ChessGameResponse();
            ChessGame game;
            ChessPlayer playerInfo;

            try
            {
                // If the player is not found, return an error message
                if (!_players.TryGetValue(connectionId, out playerInfo))
                {
                    response.ErrorMessage = "Player not found";
                    return response;
                }
                //If the game is not found, return an error message
                if (!_games.TryGetValue(connectionId, out game))
                {
                    response.ErrorMessage = "Game not found";
                    return response;
                }

                // Validate if the game is over
                if (game.State != State.IN_PROGRESS)
                {
                    response.ErrorMessage = "The game is over";
                    return response;
                }

                // Validate if it's the player's turn
                if (game.Turn != playerInfo.Team)
                {
                    response.ErrorMessage = "It's not your turn";
                    return response;
                }

                // Validate if the move is legal
                MoveResult moveResult = game.Move(move);
                response.MoveResult = moveResult;
                response.Fen = game.ToString();
                response.State = game.State;
                response.Promotion = game.Promotion;
                response.Turn = game.Turn;
                response.MoveHistory = game.MoveHistory;

                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error processing move for connection {ConnectionId}", connectionId);
                response.ErrorMessage = "Error processing the move";
                return response;
            }
        }

        public ChessGameResponse GetStockfishMove(string connectionId)
        {
            _logger.LogInformation("Getting Stockfish move for connection {ConnectionId}", connectionId);
            ChessGameResponse response = new ChessGameResponse();
            ChessGame game;
            ChessPlayer playerInfo;

            try
            {
                // If the player is not found, return an error message
                if (!_players.TryGetValue(connectionId, out playerInfo))
                {
                    response.ErrorMessage = "Player not found";
                    return response;
                }

                //If the game is not found, return an error message
                if (!_games.TryGetValue(connectionId, out game))
                {
                    response.ErrorMessage = "Game not found";
                    return response;
                }

                // Validate if the game is over
                if (game.State != State.IN_PROGRESS)
                {
                    response.ErrorMessage = "The game is over";
                    return response;
                }

                // Validate if it's the player's turn
                if (game.Turn == playerInfo.Team)
                {
                    response.ErrorMessage = "It's your turn";
                    return response;
                }


                // Generate the Stockfish move
                string stockfishMove = GenerateStockfishMove(game.ToString());

                // Attempt to make the Stockfish move
                MoveResult moveResult = game.Move(stockfishMove);

                if (!moveResult.IsSuccessful())
                {
                    throw new Exception("Stockfish move is not legal");
                }

                if (game.Promotion != null)
                {
                    game.Promote("q");
                }

                response.Fen = game.ToString();
                response.State = game.State;
                response.Turn = game.Turn;
                response.MoveHistory = game.MoveHistory;

                if (game.State == State.IN_PROGRESS)
                {
                    response.LegalMoves = game.GetAllLegalMoves();
                }

                return response;
            }

            catch (Exception e)
            {
                _logger.LogError(e, "Error processing move for connection {ConnectionId}", connectionId);
                response.ErrorMessage = "Error generating Stockfish move";
                return response;
            }
        }

        private string GenerateStockfishMove(string fen)
        {
            IStockfish stockfish = new Stockfish.NET.Stockfish(_stockfishPath);
            stockfish.SetFenPosition(fen);
            return stockfish.GetBestMove();
        }
    }
}
