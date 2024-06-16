using System.Security.Cryptography;
using ChessLibrary.Models.Pieces;
using System.Text;
using System.Text.Json;

namespace ChessWebApp.Models.DTOs
{
    public class ChessPlayer
    {
        public ChessPlayer(string connectionId)
        {
            this.ConnectionId = connectionId;
            Random random = new Random();
            this.Username = "Guest#" + random.Next(1000, 9999);
        }

        public readonly string ConnectionId;
        public PieceTeam Team { get; set; }
        public string Username;
    }
}
