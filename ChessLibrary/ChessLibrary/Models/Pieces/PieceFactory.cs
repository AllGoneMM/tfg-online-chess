namespace ChessLibrary.Models.Pieces
{
    public class PieceFactory
    {
        public Piece CreatePiece(PieceTeam pieceTeam, PieceType pieceType)
        {
            switch (pieceType)
            {
                case PieceType.KING:
                    return new King(pieceTeam, pieceType);
                case PieceType.ROOK:
                    return new Rook(pieceTeam, pieceType);
                case PieceType.KNIGHT:
                    return new Knight(pieceTeam, pieceType);
                case PieceType.QUEEN:
                    return new Queen(pieceTeam, pieceType);
                case PieceType.BISHOP:
                    return new Bishop(pieceTeam, pieceType);
                default:
                    return new Pawn(pieceTeam, pieceType);
            }
        }
        public Piece CreatePiece(char pieceRepresentation)
        {
            PieceTeam pieceTeam = char.IsUpper(pieceRepresentation) ? PieceTeam.WHITE : PieceTeam.BLACK;
            switch (char.ToUpper(pieceRepresentation))
            {
                case 'K':
                    return new King(pieceTeam, PieceType.KING);
                case 'R':
                    return new Rook(pieceTeam, PieceType.ROOK);
                case 'N':
                    return new Knight(pieceTeam, PieceType.KNIGHT);
                case 'Q':
                    return new Queen(pieceTeam, PieceType.QUEEN);
                case 'B':
                    return new Bishop(pieceTeam, PieceType.BISHOP);
                default:
                    return new Pawn(pieceTeam, PieceType.PAWN);
            }
        }
    }
}
