﻿using ChessLibrary.Engine;
using ChessLibrary.Engine.Movement;

namespace ChessLibrary.Models.Pieces
{
    public class Rook(PieceTeam team, PieceType type) : Piece(team, type)
    {
        public override List<Move> GetLegalMoves(Context context, int originIndex)
        {
            List<Move> legalMoves = new();
            int[] rookDirections = new int[4] { MoveDirection.Up, MoveDirection.Down, MoveDirection.Left, MoveDirection.Right };
            Func<int, int, bool> isInsideBounds = MoveValidator.IsMoveInsideBounds;
            Func<int, bool> squareContainsAllyPiece = (squareIndex) =>
            {
                return context.Board.ContainsAllyPiece(squareIndex, Team);
            };
            Func<int, bool> squareContainsEnemyPiece = (squareIndex) =>
            {
                return context.Board.ContainsEnemyPiece(squareIndex, Team);
            };
            foreach (int direction in rookDirections)
            {
                for (int moveLocation = originIndex; isInsideBounds(direction, moveLocation) && !squareContainsAllyPiece(direction + moveLocation); moveLocation += direction)
                {
                    Move move = new Move(direction + moveLocation, originIndex, MoveType.NONE);
                    legalMoves.Add(move);
                    if (squareContainsEnemyPiece(direction + moveLocation))
                    {
                        break;
                    }
                }
            }

            return legalMoves;
        }
    }
}