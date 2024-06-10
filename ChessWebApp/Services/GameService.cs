using System.Collections.Concurrent;
using ChessLibrary;

namespace ChessWebApp.Services
{
    public class GameService
    {
        private readonly ConcurrentDictionary<string, ChessGame> _games;

        public GameService()
        {
            _games = new ConcurrentDictionary<string, ChessGame>();
        }

        public ChessGame GetOrCreateGame(string connectionId)
        {
            return _games.GetOrAdd(connectionId, _ => new ChessGame());
        }

        public void RemoveGame(string connectionId)
        {
            _games.TryRemove(connectionId, out _);
        }
    }
}
