using ChessLibrary;
using ChessWebApp.Models.DTOs;

namespace ChessWebApp.Services
{
    public interface IOfflineGameService
    {
        void TryRemoveGame(string connectionId);
        void TryRemovePlayer(string connectionId);
        ChessGameResponse StartGame(ChessPlayer playerInfo);
        ChessGameResponse ProcessMove(string connectionId, string move);
        ChessGameResponse GetStockfishMove(string connectionId);
    }
}
