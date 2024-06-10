namespace ChessLibrary.Engine.Movement;

public class SelectSquareResult
{
    private static readonly SelectSquareResult _success = new SelectSquareResult { IsSuccessful = true };
    private static readonly SelectSquareResult _gameEnded = new SelectSquareResult { IsGameEnded = true };
    private static readonly SelectSquareResult _invalidPositionFormat = new SelectSquareResult { IsInvalidPositionFormat = true };
    private static readonly SelectSquareResult _emptySquare = new SelectSquareResult { IsEmptySquare = true };
    private static readonly SelectSquareResult _enemyPiece = new SelectSquareResult { IsEnemyPiece = true };
    private static readonly SelectSquareResult _noAvailableMoves = new SelectSquareResult { IsNoAvailableMoves = true };
    private static readonly SelectSquareResult _promotePawn = new SelectSquareResult { IsPromotePawn = true };

    public bool IsSuccessful { get; protected set; }
    public bool IsGameEnded { get; protected set; }
    public bool IsInvalidPositionFormat { get; protected set; }
    public bool IsEmptySquare { get; protected set; }
    public bool IsEnemyPiece { get; protected set; }
    public bool IsNoAvailableMoves { get; protected set; }
    public bool IsPromotePawn { get; protected set; }

    public static SelectSquareResult Success => _success;
    public static SelectSquareResult GameEnded => _gameEnded;
    public static SelectSquareResult InvalidPositionFormat => _invalidPositionFormat;
    public static SelectSquareResult EmptySquare => _emptySquare;
    public static SelectSquareResult EnemyPiece => _enemyPiece;
    public static SelectSquareResult NoAvailableMoves => _noAvailableMoves;
    public static SelectSquareResult PromotePawn => _promotePawn;

    public override string ToString()
    {
        return IsGameEnded ? "GameEnded" :
               IsInvalidPositionFormat ? "InvalidPositionFormat" :
               IsEmptySquare ? "EmptySquare" :
               IsEnemyPiece ? "EnemyPiece" :
               IsNoAvailableMoves ? "NoAvailableMoves" :
               IsPromotePawn ? "PromotePawn" :
               IsSuccessful ? "Successful" : "Failed";
    }
}

