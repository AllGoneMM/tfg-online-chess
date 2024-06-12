using ChessLibrary.Models.Pieces;

namespace ChessLibrary.Models;

public class Square
{
    public Square(int squareIndex)
    {
        this.SquareIndex = squareIndex;
    }

    public Piece? Piece { get; private set; }

    public int SquareIndex { get; }

    public bool IsEmpty()
    {
        return Piece == null;
    }

    public void SetPiece(Piece piece)
    {
        this.Piece = piece;
    }

    public void RemovePiece()
    {
        this.Piece = null;
    }

    public string SquarePosition
    {
        get
        {
            // Calcula la columna de la casilla basándose en el índice de casilla.
            char column = (char)('a' + (SquareIndex % 8)); // 'a' + (índice % 8)

            // Calcula la fila de la casilla basándose en el índice de casilla.
            char row = (char)('8' - (SquareIndex / 8)); // '8' - (índice / 8)

            // Concatena la columna y la fila para obtener la posición en notación de ajedrez.
            return $"{column}{row}";
        }
    }
}
