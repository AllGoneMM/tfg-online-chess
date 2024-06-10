namespace ChessLibrary.Engine.Movement;

public class MoveResult
{
    public SelectSquareResult SelectSquareResult { get; }
    public MoveSquareResult MoveSquareResult { get; }

    public MoveResult(SelectSquareResult selectionResult, MoveSquareResult moveResult)
    {
        SelectSquareResult = selectionResult;
        MoveSquareResult = moveResult;
    }

    public bool IsSuccessful()
    {
        return SelectSquareResult.IsSuccessful && MoveSquareResult.IsSuccessful;
    }

    public static MoveResult Success => new MoveResult(SelectSquareResult.Success, MoveSquareResult.Success);
}