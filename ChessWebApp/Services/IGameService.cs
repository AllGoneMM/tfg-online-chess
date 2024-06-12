using ChessLibrary;
using ChessWebApp.Models.DTOs;

namespace ChessWebApp.Services
{
    public interface IGameService
    {
        void TryRemoveGame(string connectionId);
        void TryRemovePlayer(string connectionId);
        ChessGameResponse StartGame(PlayerInfo playerInfo);
        ChessGameResponse ProcessMove(string connectionId, string move);
        ChessGameResponse GetStockfishMove(string connectionId);
    }
}
