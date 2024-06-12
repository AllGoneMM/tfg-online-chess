using ChessLibrary.Engine;
using ChessLibrary.Engine.Movement;
using ChessLibrary.Exceptions;
using ChessLibrary.Models;
using ChessLibrary.Models.Pieces;
using ChessLibrary.Utils;

namespace ChessLibrary
{
    public class ChessGame
    {
        private readonly Context _gameContext;

        public Board Board => _gameContext.Board;
        public PieceTeam Turn
        {
            get => _gameContext.Turn;
            private set => _gameContext.Turn = value;
        }
        public bool KingSideCastlingWhite
        {
            get => _gameContext.KingSideCastlingWhite;
            private set => _gameContext.KingSideCastlingWhite = value;
        }

        public bool QueenSideCastlingWhite
        {
            get => _gameContext.QueenSideCastlingWhite;
            private set => _gameContext.QueenSideCastlingWhite = value;
        }

        public bool KingSideCastlingBlack
        {
            get => _gameContext.KingSideCastlingBlack;
            private set => _gameContext.KingSideCastlingBlack = value;
        }

        public bool QueenSideCastlingBlack
        {
            get => _gameContext.QueenSideCastlingBlack;
            private set => _gameContext.QueenSideCastlingBlack = value;
        }

        public Square EnPassant
        {
            get => _gameContext.EnPassant;
            private set => _gameContext.EnPassant = value;
        }

        public int HalfMoveClock
        {
            get => _gameContext.HalfMoveClock;
            private set => _gameContext.HalfMoveClock = value;
        }

        public int TotalMoves
        {
            get => _gameContext.TotalMoves;
            private set => _gameContext.TotalMoves = value;
        }
        public State State
        {
            get => _gameContext.State;
            private set => _gameContext.State = value;
        }
        public Square? Promotion
        {
            get => _gameContext.Promotion;
            private set => _gameContext.Promotion = value;
        }

        public List<string> MoveHistory
        {
            get
            {
                List<string> moveHistory = new();
                foreach ((Move move, string fen) in _gameContext.MoveHistory)
                {
                    moveHistory.Add(move + " " + fen);
                }
                return moveHistory;
            }
        }

    /// <summary>
    /// Returns the current square position in chess notation, if no square is selected returns null.
    /// </summary>
    public string? CurrentSquare => _currentSquare?.SquarePosition;

    private Square? _currentSquare;

    /// <summary>
    /// Initializes a new instance of the ChessGame with a default chess game startup.
    /// </summary>
    public ChessGame()
    {
        _gameContext = new Context();
    }
    /// <summary>
    /// Initializes a new instance of the ChessGame class using a FEN string.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when parameter is null.</exception>
    /// <exception cref="FenFormatException">Thrown when fen is invalid.</exception>
    public ChessGame(string fen)
    {
        if (fen == null)
        {
            throw new ArgumentNullException(nameof(fen), "Parameter cannot be null.");
        }

        if (!FenConverter.IsFenValid(fen))
        {
            throw new FenFormatException("Fen string format is not valid");
        }
        _gameContext = new Context(fen);
    }
    /// <summary>
    /// Returns the current square moves, if no square is selected returns an empty list.
    /// </summary>
    public List<Move> CurrentSquareMoves { get; private set; } = [];


    public SelectSquareResult SelectSquare(string squarePosition)
    {
        Square? selectedSquare = Board.GetSquare(squarePosition);
        if (selectedSquare == null) return SelectSquareResult.InvalidPositionFormat;

        if (State != State.IN_PROGRESS) return SelectSquareResult.GameEnded;

        if (Promotion != null) return SelectSquareResult.PromotePawn;

        Piece? selectedPiece = selectedSquare.Piece;
        if (selectedPiece == null) return SelectSquareResult.EmptySquare;
        if (selectedPiece.Team != Turn) return SelectSquareResult.EnemyPiece;

        MoveValidator validator = new(_gameContext);
        var selectedPieceLegalMoves = validator.GetLegalMoves(selectedSquare);

        if (selectedPieceLegalMoves.Count == 0) return SelectSquareResult.NoAvailableMoves;

        _currentSquare = selectedSquare;
        CurrentSquareMoves = selectedPieceLegalMoves;
        return SelectSquareResult.Success;
    }

    public MoveSquareResult MoveSquare(string squarePosition)
    {

        Move? move = null;
        foreach (Move m in CurrentSquareMoves)
        {
            if (m.TargetIndex == Board.GetSquare(squarePosition).SquareIndex)
            {
                move = m;
                break;
            }
        }
        if (move == null) return MoveSquareResult.IllegalMove;

        MoveProcessor moveProcessor = new(_gameContext);
        moveProcessor.ProcessMove(move);

        DeselectSquare();
        return MoveSquareResult.Success;
    }

    public MoveResult Move(string moveString)
    {
        string selectSquarePosition = moveString.Substring(0, 2);
        SelectSquareResult selectSquareResult = SelectSquare(selectSquarePosition);

        string moveSquarePosition = moveString.Substring(2, 2);
        MoveSquareResult moveSquareResult = MoveSquare(moveSquarePosition);

        return new MoveResult(selectSquareResult, moveSquareResult);
    }

    public void Promote(string pieceFen)
    {
        pieceFen = pieceFen.ToLower();
        PieceType promoteType = PieceType.NONE;
        foreach (Square t in Board.Layout)
        {
            switch (pieceFen)
            {
                case "q": promoteType = PieceType.QUEEN; break;
                case "r": promoteType = PieceType.ROOK; break;
                case "b": promoteType = PieceType.BISHOP; break;
                case "n": promoteType = PieceType.KNIGHT; break;
                default: throw new ArgumentException();
            }
        }
        MoveProcessor moveProcessor = new(_gameContext);
        moveProcessor.ProcessPromotion(promoteType);
    }

    public void DeselectSquare()
    {
        _currentSquare = null;
        CurrentSquareMoves = [];
    }


    public override string ToString()
    {
        return _gameContext.ToString();
    }
}
}
