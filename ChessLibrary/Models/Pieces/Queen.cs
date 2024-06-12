using ChessLibrary.Engine;
using ChessLibrary.Engine.Movement;

namespace ChessLibrary.Models.Pieces
{
    public class Queen(PieceTeam team, PieceType type) : Piece(team, type)
    {
        public override List<Move> GetLegalMoves(Context context, int originIndex)
        {
            List<Move> legalMoves = new();
            Rook rook = new Rook(Team, PieceType.ROOK);
            Bishop bishop = new Bishop(Team, PieceType.BISHOP);
            legalMoves.AddRange(rook.GetLegalMoves(context, originIndex));
            legalMoves.AddRange(bishop.GetLegalMoves(context, originIndex));
            return legalMoves;
        }
    }
}