using ChessLibrary.Models;
using ChessLibrary.Models.Pieces;

namespace ChessLibrary.Engine.Movement;

public class Move
{
    public int OriginIndex { get; private set; }

    public int TargetIndex { get; private set; }

    public MoveType MoveType { get; private set; }

    public Move(int targetIndex, int originIndex, MoveType moveType)
    {
        this.MoveType = moveType;
        this.OriginIndex = originIndex;
        this.TargetIndex = targetIndex;
    }

    public override string ToString()
    {
        // Calcula la columna de la casilla basándose en el índice de casilla.
        char column = (char)('a' + (OriginIndex % 8)); // 'a' + (índice % 8)

        // Calcula la fila de la casilla basándose en el índice de casilla.
        char row = (char)('8' - (OriginIndex / 8)); // '8' - (índice / 8)

        char column2 = (char)('a' + (TargetIndex % 8)); // 'a' + (índice % 8)

        // Calcula la fila de la casilla basándose en el índice de casilla.
        char row2 = (char)('8' - (TargetIndex / 8)); // '8' - (índice / 8)

        // Concatena la columna y la fila para obtener la posición en notación de ajedrez.
        return $"{column}{row}{column2}{row2}";
    }
}