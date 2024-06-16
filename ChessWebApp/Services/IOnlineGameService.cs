using ChessLibrary;
using ChessWebApp.Models;
using ChessWebApp.Models.DTOs;

namespace ChessWebApp.Services
{
    public interface IOnlineGameService
    {
        OnlineChessGame? JoinQueue(ChessPlayer player);
        void LeaveQueue(string connectionId);
        void SuspendGame(string contextConnectionId);
        OnlineChessGame? GetGameByConnectionId(string connectionId);
        public bool RemoveGame(string gameId);
    }
}