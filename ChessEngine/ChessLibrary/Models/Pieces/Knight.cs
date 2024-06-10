using ChessLibrary.Engine;
using ChessLibrary.Engine.Movement;

namespace ChessLibrary.Models.Pieces
{
    public class Knight(PieceTeam team, PieceType type) : Piece(team, type)
    {
        public override List<Move> GetLegalMoves(Context context, int originIndex)
        {
            List<Move> legalMoves = new();
            int[] knightStartingDirections = new int[4] { MoveDirection.Up, MoveDirection.Down, MoveDirection.Left, MoveDirection.Right };
            Func<int, int, bool> isInsideBounds = MoveValidator.IsMoveInsideBounds;
            Func<int, bool> squareContainsAllyPiece = (squareIndex) =>
            {
                return context.Board.ContainsAllyPiece(squareIndex, Team);
            };

            foreach (int direction in knightStartingDirections)
            {
                int tempLocation = originIndex;

                if (isInsideBounds(direction, tempLocation))
                {
                    tempLocation += direction;
                    if (isInsideBounds(direction, tempLocation))
                    {
                        tempLocation += direction;

                        if (direction == MoveDirection.Up | direction == MoveDirection.Down)
                        {
                            if (isInsideBounds(MoveDirection.Left, tempLocation) && !squareContainsAllyPiece(MoveDirection.Left + tempLocation))
                            {
                                Move move = new Move(MoveDirection.Left + tempLocation, originIndex, MoveType.NONE);
                                legalMoves.Add(move);
                            }
                            if (isInsideBounds(MoveDirection.Right, tempLocation) && !squareContainsAllyPiece(MoveDirection.Right + tempLocation))
                            {
                                Move move = new Move(MoveDirection.Right + tempLocation, originIndex, MoveType.NONE);
                                legalMoves.Add(move);
                            }
                        }
                        if (direction == MoveDirection.Left | direction == MoveDirection.Right)
                        {
                            if (isInsideBounds(MoveDirection.Up, tempLocation) && !squareContainsAllyPiece(MoveDirection.Up + tempLocation))
                            {
                                Move move = new Move(MoveDirection.Up + tempLocation, originIndex, MoveType.NONE);
                                legalMoves.Add(move);
                            }
                            if (isInsideBounds(MoveDirection.Down, tempLocation) && !squareContainsAllyPiece(MoveDirection.Down + tempLocation))
                            {
                                Move move = new Move(MoveDirection.Down + tempLocation, originIndex, MoveType.NONE);
                                legalMoves.Add(move);
                            }
                        }
                    }
                }
            }

            return legalMoves;
        }
    }
}