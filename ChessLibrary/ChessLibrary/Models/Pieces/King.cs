using ChessLibrary.Engine;
using ChessLibrary.Engine.Movement;

namespace ChessLibrary.Models.Pieces
{
    public class King(PieceTeam team, PieceType type) : Piece(team, type)
    {
        public override List<Move> GetLegalMoves(Context context, int originIndex)
        {
            List<Move> legalMoves = [];

            const int up = MoveDirection.Up;
            const int down = MoveDirection.Down;
            const int left = MoveDirection.Left;
            const int right = MoveDirection.Right;
            const int upLeft = MoveDirection.UpLeft;
            const int upRight = MoveDirection.UpRight;
            const int downLeft = MoveDirection.DownLeft;
            const int downRight = MoveDirection.DownRight;
            Func<int, int, bool> isInsideBounds = MoveValidator.IsMoveInsideBounds;
            Func<int, bool> squareContainsAllyPiece = (squareIndex) =>
            {
                return context.Board.ContainsAllyPiece(squareIndex, Team);
            };
            Func<int, bool> isSquareEmpty = context.Board.IsSquareEmpty;
            int[] allDirections = new int[8] { left, right, up, down, upLeft, upRight, downLeft, downRight };

            foreach (int direction in allDirections)
            {
                if (isInsideBounds(direction, originIndex) &&
                    !squareContainsAllyPiece(direction + originIndex))
                {
                    Move move = new Move(direction + originIndex, originIndex, MoveType.NONE);
                    legalMoves.Add(move);
                }
            }

            //CASTLING ... AÑADIR QUE NO PUEDE ESTAR EN JAQUE

            //CASTLING PARA BLANCAS
            if (Team == PieceTeam.WHITE)
            {
                //KINGSIDE WHITE
                if (isInsideBounds(right, originIndex) && isSquareEmpty(right + originIndex) && isInsideBounds(right, originIndex + right) && isSquareEmpty(right + originIndex + right))
                {
                    if (!isSquareEmpty(63) && squareContainsAllyPiece(63) && context.KingSideCastlingWhite)
                    {
                        Move move = new Move(originIndex + MoveDirection.Right + MoveDirection.Right, originIndex, MoveType.CASTLING);
                        legalMoves.Add(move);
                    }
                }

                //QUEENSIDE WHITE
                if (isInsideBounds(MoveDirection.Left, originIndex) && isSquareEmpty(MoveDirection.Left+ originIndex) && isInsideBounds(MoveDirection.Left, originIndex + MoveDirection.Left) && isSquareEmpty(MoveDirection.Left + originIndex + MoveDirection.Left) && isInsideBounds(MoveDirection.Left, originIndex + MoveDirection.Left + MoveDirection.Left) && isSquareEmpty(MoveDirection.Left + originIndex + MoveDirection.Left + MoveDirection.Left))
                {
                    if (!isSquareEmpty(56) && squareContainsAllyPiece(56) && context.QueenSideCastlingWhite)
                    {
                        Move move = new Move(originIndex + MoveDirection.Left + MoveDirection.Left, originIndex, MoveType.CASTLING);
                        legalMoves.Add(move);
                    }
                }

            }
            //CASTLING PARA NEGRAS
            else
            {
                //KINGSIDE BLACK
                if (isInsideBounds(right, originIndex) && isSquareEmpty(right + originIndex) && isInsideBounds(MoveDirection.Right, originIndex + MoveDirection.Right) && isSquareEmpty(MoveDirection.Right + originIndex + MoveDirection.Right))
                {
                    if (!isSquareEmpty(7) && squareContainsAllyPiece(7) && context.KingSideCastlingBlack)
                    {
                        Move move = new Move(originIndex + MoveDirection.Right + MoveDirection.Right, originIndex, MoveType.CASTLING);
                        legalMoves.Add(move);
                    }
                }

                //QUEENSIDE BLACK
                if (isInsideBounds(MoveDirection.Left, originIndex) && isSquareEmpty(MoveDirection.Left + originIndex) && isInsideBounds(MoveDirection.Left, originIndex + MoveDirection.Left) && isSquareEmpty(MoveDirection.Left + originIndex + MoveDirection.Left) && isInsideBounds(MoveDirection.Left, originIndex + MoveDirection.Left + MoveDirection.Left) && isSquareEmpty(MoveDirection.Left + originIndex + MoveDirection.Left + MoveDirection.Left))
                {
                    if (!isSquareEmpty(0) && squareContainsAllyPiece(0) && context.QueenSideCastlingBlack)
                    {
                        Move move = new Move(originIndex + MoveDirection.Left + MoveDirection.Left, originIndex, MoveType.CASTLING);
                        legalMoves.Add(move);
                    }
                }
            }

            return legalMoves;
        }
    }


}