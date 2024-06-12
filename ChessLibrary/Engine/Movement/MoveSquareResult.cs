namespace ChessLibrary.Engine.Movement;

public class MoveSquareResult
{
    private static readonly MoveSquareResult _success = new MoveSquareResult { IsSuccessful = true };
    private static readonly MoveSquareResult _noPieceSelected = new MoveSquareResult { IsNoPieceSelected = true };
    private static readonly MoveSquareResult _invalidMoveFormat = new MoveSquareResult { IsInvalidMoveFormat = true };
    private static readonly MoveSquareResult _illegalMove = new MoveSquareResult { IsIllegalMove = true };

    public bool IsSuccessful { get; protected set; }
    public bool IsNoPieceSelected { get; protected set; }
    public bool IsInvalidMoveFormat { get; protected set; }
    public bool IsIllegalMove { get; protected set; }

    public static MoveSquareResult Success => _success;
    public static MoveSquareResult NoPieceSelected => _noPieceSelected;
    public static MoveSquareResult InvalidMoveFormat => _invalidMoveFormat;
    public static MoveSquareResult IllegalMove => _illegalMove;

    public override string ToString()
    {
        return IsNoPieceSelected ? "NoPieceSelected" :
            IsInvalidMoveFormat ? "InvalidMoveFormat" :
            IsIllegalMove ? "IllegalMove" :
            IsSuccessful ? "Successful" : "Failed";
    }
}
