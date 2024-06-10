using ChessLibrary;
using ChessLibrary.Engine.Movement;
using Microsoft.AspNetCore.SignalR;

namespace ChessWebApp.Hubs
{
    public class OfflineGameHub(ChessGame game) : Hub
    {
        public override Task OnConnectedAsync()
        {
            game = new ChessGame();
            return base.OnConnectedAsync();
        }
        public async Task<bool> IsLegalMove(string move)
        {
            // Aquí implementa la lógica para verificar si el movimiento es legal
            // Supongamos que tienes una clase ChessGame con un método IsLegalMove
            MoveResult result = game.Move(move);
            if (result.IsSuccessful())
            {
                return true;
            }
            return false;
        }
    }
}
