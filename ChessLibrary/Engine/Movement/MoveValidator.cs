using ChessLibrary.Models.Pieces;
using ChessLibrary.Models;

namespace ChessLibrary.Engine.Movement
{
    public class MoveValidator
    {
        private readonly Context _context;

        private readonly Board _board;

        private List<Move> _legalMoves;

        private Square? _originSquare;

        private Piece? _piece;

        private int _originIndex;

        public MoveValidator(Context context)
        {
            _context = context;
            _board = _context.Board;
            _legalMoves = new List<Move>();
        }
        public List<Move> GetAllAllyMoves()
        {
            List<Move> allAllyLegalMoves = new List<Move>();

            List<Square> allAllySquares = _board.GetAllAllySquares(_context.Turn);

            foreach (var square in allAllySquares)
            {
                allAllyLegalMoves.AddRange(GetLegalMoves(square));
            }

            return allAllyLegalMoves;
        }
        public List<Move> GetLegalMoves(Square originSquare)
        {
            _legalMoves = [];
            _originSquare = originSquare;
            _piece = originSquare.Piece;
            _originIndex = _originSquare.SquareIndex;

            // TODO: Estos métodos no deberían devolver ninguna lista, sino que deberían modificar la lista _legalMoves

            _legalMoves = _piece.GetLegalMoves(_context, _originIndex);
            RemoveCheckMoves();
            return _legalMoves;
        }

        // TODO: Relegar este método a la clase Piece, convertirla en abstracta y que cada pieza implemente su propio método


        public List<Move> GetAllEnemyMoves(Context enemyContext)
        {
            List<Move> allEnemyLegalMoves = new List<Move>();
            List<Square> allEnemySquares = enemyContext.Board.GetAllEnemySquares(_context.Turn);

            foreach (var square in allEnemySquares)
            {
                Piece enemyPiece = square.Piece;
                allEnemyLegalMoves.AddRange(enemyPiece.GetLegalMoves(enemyContext, square.SquareIndex));
            }

            return allEnemyLegalMoves;
        }





        private void RemoveCheckMoves()
        {
            List<Move> checkMoves = new List<Move>();
            foreach (Move move in _legalMoves)
            {
                //COPIA LA MESA
                Context enemyContext = new Context(_context.ToString());

                //REALIZA EL MOVIMIENTO EN LA MESA DE COPIA
                MoveProcessor processor = new MoveProcessor(enemyContext);
                processor.MakeMove(move);

                //RECORRE LA MESA DE COPIA Y VA GUARDANDO TODOS LOS MOVIMIENTOS LEGALES DE TODAS LAS PIEZAS ENEMIGAS
                List<Move> allEnemyLegalMoves = GetAllEnemyMoves(enemyContext);


                //VUELVE A RECORRER LA COPIA DE LA MESA, POR CADA PIEZA ALIADA, DETECTA SI EL REY SE ENCUENTRA EN LA LISTA DE MOVIMIENTOS ENEMIGOS
                List<Square> allAllySquares = enemyContext.Board.GetAllAllySquares(_context.Turn);
                foreach (var allySquare in allAllySquares)
                {
                    if (allySquare.Piece.Type == PieceType.KING)
                    {
                        int kingIndex = allySquare.SquareIndex;
                        foreach (Move enemyMove in allEnemyLegalMoves)
                        {
                            if (kingIndex == enemyMove.TargetIndex)
                            {
                                checkMoves.Add(move);
                            }
                            if (move.MoveType == MoveType.CASTLING)
                            {
                                // Si el rey está en jaque, no puede realizar castling
                                if (enemyMove.TargetIndex == kingIndex)
                                {
                                    checkMoves.Add(move);
                                }

                                // Si alguna de las casillas de camino está en jaque, no puede realizar castling
                                int[] whiteKingSideCastlingSquares = new int[] { 61, 62 };
                                int[] whiteQueenSideCastlingSquares = new int[] { 58, 59 };
                                int[] blackKingSideCastlingSquares = new int[] { 5, 6 };
                                int[] blackQueenSideCastlingSquares = new int[] { 2, 3 };

                                int kingTargetIndex = move.TargetIndex;
                                if (_context.Turn == PieceTeam.WHITE)
                                {
                                    if (kingTargetIndex == 62)
                                    {
                                        if (whiteKingSideCastlingSquares.Contains(enemyMove.TargetIndex)) checkMoves.Add(move);
                                    }
                                    else if (kingTargetIndex == 58)
                                    {
                                        if (whiteQueenSideCastlingSquares.Contains(enemyMove.TargetIndex)) checkMoves.Add(move);
                                    }
                                }
                                else
                                {
                                    if (kingTargetIndex == 6)
                                    {
                                        if (blackKingSideCastlingSquares.Contains(enemyMove.TargetIndex)) checkMoves.Add(move);
                                    }
                                    else if (kingTargetIndex == 2)
                                    {
                                        if (blackQueenSideCastlingSquares.Contains(enemyMove.TargetIndex)) checkMoves.Add(move);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            foreach (Move move in checkMoves)
            {
                _legalMoves.Remove(move);
            }
        }

        public static bool IsMoveInsideBounds(int moveDirection, int originSquareIndex)
        {
            switch (moveDirection)
            {
                case MoveDirection.Up:
                    return IsInsideBounds(originSquareIndex, Board.TopBounds);

                case MoveDirection.Down:
                    return IsInsideBounds(originSquareIndex, Board.BottomBounds);

                case MoveDirection.Left:
                    return IsInsideBounds(originSquareIndex, Board.LeftBounds);

                case MoveDirection.Right:
                    return IsInsideBounds(originSquareIndex, Board.RightBounds);

                case MoveDirection.UpLeft:
                    return IsInsideBounds(originSquareIndex, Board.TopBounds) && IsInsideBounds(originSquareIndex, Board.LeftBounds);

                case MoveDirection.UpRight:
                    return IsInsideBounds(originSquareIndex, Board.TopBounds) && IsInsideBounds(originSquareIndex, Board.RightBounds);

                case MoveDirection.DownLeft:
                    return IsInsideBounds(originSquareIndex, Board.BottomBounds) && IsInsideBounds(originSquareIndex, Board.LeftBounds);

                case MoveDirection.DownRight:
                    return IsInsideBounds(originSquareIndex, Board.BottomBounds) && IsInsideBounds(originSquareIndex, Board.RightBounds);
            }
            return false;
        }

        private static bool IsInsideBounds(int location, int[] bounds)
        {
            foreach (int bound in bounds)
            {
                if (location == bound)
                    return false;
            }
            return true;
        }

    }
}
