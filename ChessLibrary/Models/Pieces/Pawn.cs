using ChessLibrary.Engine;
using ChessLibrary.Engine.Movement;

namespace ChessLibrary.Models.Pieces
{
    public class Pawn(PieceTeam team, PieceType type) : Piece(team, type)
    {
        public override List<Move> GetLegalMoves(Context context, int originIndex)
        {
            List<Move> legalMoves = [];

            const int up = MoveDirection.Up;
            const int down = MoveDirection.Down;
            const int upLeft = MoveDirection.UpLeft;
            const int upRight = MoveDirection.UpRight;
            const int downLeft = MoveDirection.DownLeft;
            const int downRight = MoveDirection.DownRight;

            Func<int, int, bool> isInsideBounds = MoveValidator.IsMoveInsideBounds;
            Func<int, bool> isSquareEmpty = context.Board.IsSquareEmpty;
            Func<int, bool> isPromotionMove = (squareIndex) =>
            {
                if (Team == PieceTeam.WHITE)
                {
                    return Board.TopBounds.Contains(squareIndex);
                }
                else
                {
                    return Board.BottomBounds.Contains(squareIndex);
                }
            };
            Func<int, bool> squareContainsEnemyPiece = (squareIndex) =>
            {
                return context.Board.ContainsEnemyPiece(squareIndex, Team);
            };
            Func<bool> isOnStartingPosition = () =>
            {
                if (Team == PieceTeam.WHITE)
                {
                    return Board.WhitePawnStartingPosition.Contains(originIndex);
                }
                else
                {
                    return Board.BlackPawnStartingPosition.Contains(originIndex);
                }
            };
            Func<int, bool> canEnPassant = (targetIndex) =>
            {
                Square? enPassant = context.EnPassant;
                if (enPassant == null)
                {
                    return false;
                }

                if (enPassant.SquareIndex == targetIndex)
                {
                    return true;
                }
                return false;
            };
            // WHITE PIECE
            if (Team == PieceTeam.WHITE)
            {
                // ONE STEP AND DOUBLE STEP

                if (isInsideBounds(up, originIndex) &&
                    isSquareEmpty(originIndex + up))
                {
                    int targetIndex = originIndex + up;
                    if (isPromotionMove(targetIndex))
                    {
                        Move move = new Move(targetIndex, originIndex, MoveType.PROMOTION);
                        legalMoves.Add(move);
                    }
                    else
                    {
                        Move move = new Move(targetIndex, originIndex, MoveType.NONE);
                        legalMoves.Add(move);
                    }


                    if (isOnStartingPosition() &&
                        isInsideBounds(up, targetIndex) &&
                        isSquareEmpty(up + targetIndex))
                    {
                        targetIndex += up;
                        Move move = new Move(targetIndex, originIndex, MoveType.DOBLE_STEP);
                        legalMoves.Add(move);
                    }
                }

                // DIAGONAL CAPTURE
                if (isInsideBounds(upLeft, originIndex) &&
                    squareContainsEnemyPiece(upLeft + originIndex))
                {
                    int moveLocation = originIndex + upLeft;
                    if (isPromotionMove(moveLocation))
                    {
                        Move move = new Move(moveLocation, originIndex, MoveType.PROMOTION);
                        legalMoves.Add(move);
                    }
                    else
                    {
                        Move move = new Move(moveLocation, originIndex, MoveType.NONE);
                        legalMoves.Add(move);
                    }
                }

                if (isInsideBounds(upRight, originIndex) &&
                    squareContainsEnemyPiece(upRight + originIndex))
                {
                    int moveLocation = originIndex + upRight;
                    if (isPromotionMove(moveLocation))
                    {
                        Move move = new Move(moveLocation, originIndex, MoveType.PROMOTION);
                        legalMoves.Add(move);
                    }
                    else
                    {
                        Move move = new Move(moveLocation, originIndex, MoveType.NONE);
                        legalMoves.Add(move);
                    }
                }

                // EN PASSANT
                if (canEnPassant(upLeft + originIndex))
                {
                    int moveLocation = originIndex + upLeft;
                    Move move = new Move(moveLocation, originIndex, MoveType.EN_PASSANT);
                    legalMoves.Add(move);
                }

                if (canEnPassant(upRight + originIndex))
                {
                    int moveLocation = originIndex + upRight;
                    Move move = new Move(moveLocation, originIndex, MoveType.EN_PASSANT);
                    legalMoves.Add(move);
                }
            }

            // BLACK PIECE
            else
            {
                // ONE STEP OR DOUBLE STEP
                if (isInsideBounds(down, originIndex) &&
                    isSquareEmpty(down + originIndex))
                {
                    int moveLocation = originIndex + down;
                    if (isPromotionMove(moveLocation))
                    {
                        Move move = new Move(moveLocation, originIndex, MoveType.PROMOTION);
                        legalMoves.Add(move);
                    }
                    else
                    {
                        Move move = new Move(moveLocation, originIndex, MoveType.NONE);
                        legalMoves.Add(move);
                    }

                    if (isOnStartingPosition() &&
                        isInsideBounds(down, moveLocation) &&
                        isSquareEmpty(down + moveLocation))
                    {
                        moveLocation += down;
                        Move move = new Move(moveLocation, originIndex, MoveType.DOBLE_STEP);
                        legalMoves.Add(move);
                    }
                }

                // DIAGONAL CAPTURE
                if (MoveValidator.IsMoveInsideBounds(downLeft, originIndex) &&
                    squareContainsEnemyPiece(downLeft + originIndex))
                {
                    int moveLocation = originIndex + downLeft;
                    if (isPromotionMove(moveLocation))
                    {
                        Move move = new Move(moveLocation, originIndex, MoveType.PROMOTION);
                        legalMoves.Add(move);
                    }
                    else
                    {
                        Move move = new Move(moveLocation, originIndex, MoveType.NONE);
                        legalMoves.Add(move);
                    }
                }

                if (MoveValidator.IsMoveInsideBounds(downRight, originIndex) &&
                    squareContainsEnemyPiece(downRight + originIndex))
                {
                    int moveLocation = originIndex + downRight;
                    if (isPromotionMove(moveLocation))
                    {
                        Move move = new Move(moveLocation, originIndex, MoveType.PROMOTION);
                        legalMoves.Add(move);
                    }
                    else
                    {
                        Move move = new Move(moveLocation, originIndex, MoveType.NONE);
                        legalMoves.Add(move);
                    }
                }

                // EN PASSANT
                if (canEnPassant(downLeft + originIndex))
                {
                    int moveLocation = originIndex + downLeft;
                    Move move = new Move(moveLocation, originIndex, MoveType.EN_PASSANT);
                    legalMoves.Add(move);
                }

                if (canEnPassant(downRight + originIndex))
                {
                    int moveLocation = originIndex + downRight;
                    Move move = new Move(moveLocation, originIndex, MoveType.EN_PASSANT);
                    legalMoves.Add(move);
                }
            }

            return legalMoves;
        }
    }
}