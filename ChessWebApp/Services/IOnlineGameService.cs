using ChessLibrary;
using ChessWebApp.Models.DTOs;

namespace ChessWebApp.Services
{
    public interface IOnlineGameService
    {
        (ChessGameResponse, ChessGameResponse) ProcessMove(string connectionId, string move);
        public void TryRemoveGame(PlayerInfo playerInfo);
        public void TryRemovePlayer(PlayerInfo playerInfo);
        Task<Dictionary<string, (ChessGameResponse, ChessGameInfoResponse)>> JoinQueue(PlayerInfo playerInfo);
        Task LeaveQueue(string connectionId);
        Task LeaveGame(string connectionId);
        string GetGroup(string connectionId);
        List<string> GetPlayersInGroup(string group);
    }
}