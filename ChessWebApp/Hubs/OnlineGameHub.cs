using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using ChessLibrary;
using ChessLibrary.Engine.Movement;
using ChessLibrary.Models.Pieces;
using Microsoft.AspNetCore.SignalR;

namespace ChessWebApp.Hubs
{
    public class OnlineGameHub : Hub
    {
        private static readonly ConcurrentDictionary<string, string> _connectionIdToGroup = new ConcurrentDictionary<string, string>();
        private static readonly ConcurrentDictionary<string, ChessGame> _groupToGame = new ConcurrentDictionary<string, ChessGame>();
        private static readonly ConcurrentDictionary<string, string> _connectionIdToColor = new ConcurrentDictionary<string, string>();
        private static ConcurrentQueue<string> _waitingPlayers = new ConcurrentQueue<string>();

        public async Task JoinQueue()
        {
            _waitingPlayers.Enqueue(Context.ConnectionId);
            await TryStartGame();
        }

        private async Task TryStartGame()
        {
            if (_waitingPlayers.Count >= 2)
            {
                string player1, player2;
                if (_waitingPlayers.TryDequeue(out player1) && _waitingPlayers.TryDequeue(out player2))
                {
                    string group = Guid.NewGuid().ToString();
                    await Groups.AddToGroupAsync(player1, group);
                    await Groups.AddToGroupAsync(player2, group);

                    _connectionIdToGroup[player1] = group;
                    _connectionIdToGroup[player2] = group;

                    AssignPlayerColor(player1, "White");
                    AssignPlayerColor(player2, "Black");

                    var game = new ChessGame();
                    _groupToGame[group] = game;

                    // Optionally, you can send some initial game state to both players
                    await Clients.Group(group).SendAsync("StartGame", game.ToString());
                }
            }
        }

        private void AssignPlayerColor(string connectionId, string color)
        {
            _connectionIdToColor[connectionId] = color;
        }

        public async Task LeaveQueue()
        {
            string removedPlayer;
            var newQueue = new ConcurrentQueue<string>();

            while (_waitingPlayers.TryDequeue(out removedPlayer))
            {
                if (removedPlayer != Context.ConnectionId)
                {
                    newQueue.Enqueue(removedPlayer);
                }
            }

            _waitingPlayers = newQueue;
        }

        public async Task LeaveGame()
        {
            if (_connectionIdToGroup.TryGetValue(Context.ConnectionId, out var group))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, group);
                _connectionIdToGroup.TryRemove(Context.ConnectionId, out _);
                _connectionIdToColor.TryRemove(Context.ConnectionId, out _);
            }
        }

        public async Task SendMove(string move)
        {
            if (_connectionIdToGroup.TryGetValue(Context.ConnectionId, out var group))
            {
                if (_groupToGame.TryGetValue(group, out var game))
                {
                    // Check if it's the turn of the player making the move
                    string playerColor = _connectionIdToColor[Context.ConnectionId];
                    string currentTurnColor = game.Turn == PieceTeam.WHITE ? "White" : "Black";

                    if (playerColor == currentTurnColor)
                    {
                        MoveResult result = game.Move(move);
                        if (result.IsSuccessful())
                        {
                            await Clients.Group(group).SendAsync("ReceiveMove", game.ToString());
                        }
                        else
                        {
                            // Handle invalid move
                            await Clients.Caller.SendAsync("InvalidMove", game.ToString());
                        }
                    }
                    else
                    {
                        // Notify the player that it's not their turn
                        await Clients.Caller.SendAsync("NotYourTurn", game.ToString());
                    }
                }
            }
        }

        public async Task<string> GetLegalMoves(string originSquare)
        {
            if (_connectionIdToGroup.TryGetValue(Context.ConnectionId, out var group))
            {
                if (_groupToGame.TryGetValue(group, out var game))
                {
                    game.SelectSquare(originSquare);

                    var legalMoves = game.CurrentSquareMoves;
                    List<string> legalMovesString = new List<string>();
                    foreach (Move move in legalMoves)
                    {
                        string destinationSquare = move.ToString().Substring(2, 2);
                        legalMovesString.Add(destinationSquare);
                    }

                    // Deselect the square and log the state
                    game.DeselectSquare();
                    Console.WriteLine($"CurrentSquare after deselecting: {game.CurrentSquare}");

                    // Return the serialized legal moves
                    return JsonSerializer.Serialize(legalMovesString);
                }
            }
            return JsonSerializer.Serialize(new List<string>());
        }
    }
}
