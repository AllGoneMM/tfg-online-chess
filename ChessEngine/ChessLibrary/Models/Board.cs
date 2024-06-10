using ChessLibrary.Engine.Movement;
using ChessLibrary.Models.Pieces;
using ChessLibrary.Utils;

namespace ChessLibrary.Models;

public class Board
{
    public static readonly int[] TopBounds = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 };

    public static readonly int[] BottomBounds = new int[] { 56, 57, 58, 59, 60, 61, 62, 63 };

    public static readonly int[] LeftBounds = new int[] { 0, 8, 16, 24, 32, 40, 48, 56 };

    public static readonly int[] RightBounds = new int[] { 7, 15, 23, 31, 39, 47, 55, 63 };

    public static readonly int[] WhitePawnStartingPosition = new int[] { 48, 49, 50, 51, 52, 53, 54, 55 };

    public static readonly int[] BlackPawnStartingPosition = new int[] { 8, 9, 10, 11, 12, 13, 14, 15 };

    public Square[] Layout { get; }

    public Board()
    {
        this.Layout = FenConverter.ToSquareArray(FenConverter.StartingFenString);
    }

    public Board(string fen)
    {
        this.Layout = FenConverter.ToSquareArray(fen);
    }



    public string GetFen()
    {
        return FenConverter.ToFenString(this.Layout);
    }

    public bool IsSquareEmpty(int squareIndex)
    {
        return this.Layout[squareIndex].IsEmpty();
    }

    public bool ContainsEnemyPiece(int squareIndex, PieceTeam currentTurn)
    {
        Piece? piece = GetPiece(squareIndex);
        if (piece != null)
        {
            return piece.Team != currentTurn;
        }
        return false;
    }

    public Piece? GetPiece(int squareIndex)
    {
        return this.Layout[squareIndex].Piece;
    }

    public Square GetSquare(int squareIndex)
    {
        return this.Layout[squareIndex];
    }

    public Square? GetSquare(string position)
    {
        position = position.ToLower();
        foreach (var square in Layout)
        {
            if (square.SquarePosition == position)
            {
                return square;
            }
        }
        return null;
    }


    public Square[] GetTileLayout()
    {
        return this.Layout;
    }

    public List<Piece> GetAllPieces()
    {
        List<Piece> allPieces = new List<Piece>();
        foreach (var square in Layout)
        {
            if (!square.IsEmpty())
            {
                allPieces.Add(square.Piece);
            }
        }
        return allPieces;
    }

    public List<Piece> GetAllWhitePieces()
    {
        List<Piece> allPieces = new List<Piece>();
        foreach (var square in Layout)
        {
            if (!square.IsEmpty() && square.Piece.Team == PieceTeam.WHITE)
            {
                allPieces.Add(square.Piece);
            }
        }
        return allPieces;
    }

    public List<Piece> GetAllBlackPieces()
    {
        List<Piece> allPieces = new List<Piece>();
        foreach (var square in Layout)
        {
            if (!square.IsEmpty() && square.Piece.Team == PieceTeam.BLACK)
            {
                allPieces.Add(square.Piece);
            }
        }
        return allPieces;
    }

    public List<Square> GetAllEnemySquares(PieceTeam currentTurn)
    {
        List<Square> enemySquares = new List<Square>();
        foreach (var square in Layout)
        {
            if (ContainsEnemyPiece(square.SquareIndex, currentTurn))
            {
                enemySquares.Add(square);
            }
        }
        return enemySquares;
    }

    public bool ContainsAllyPiece(int squareIndex, PieceTeam currentTurn)
    {
        Piece? piece = GetPiece(squareIndex);
        if (piece != null)
        {
            return piece.Team == currentTurn;
        }
        return false;
    }
    public List<Square> GetAllAllySquares(PieceTeam currentTurn)
    {
        List<Square> allySquares = new List<Square>();
        foreach (var square in Layout)
        {
            if (ContainsAllyPiece(square.SquareIndex, currentTurn))
            {
                allySquares.Add(square);
            }
        }
        return allySquares;
    }

    public bool ContainsPiece(int index)
    {
        if (GetPiece(index) == null) return false;
        return true;
    }

    public void RemovePiece(int index)
    {
        GetSquare(index)?.RemovePiece();
    }

    public void Move(Move move)
    {
        PieceFactory pieceFactory = new();
        if (!ContainsPiece(move.OriginIndex)) return;

        var destinationSquare = GetSquare(move.TargetIndex);

        var piece = GetPiece(move.OriginIndex);

        var pieceCopy = pieceFactory.CreatePiece((char)piece);

        RemovePiece(move.OriginIndex);
        destinationSquare?.SetPiece(pieceCopy);
    }
}