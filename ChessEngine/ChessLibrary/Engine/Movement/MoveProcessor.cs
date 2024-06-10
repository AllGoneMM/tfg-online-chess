using ChessLibrary.Models.Pieces;
using ChessLibrary.Models;

namespace ChessLibrary.Engine.Movement
{
    public class MoveProcessor(Context context)
    {
        private Move? _move;
        private string _currentFen;

        public void MakeMove(Move move)
        {
            Piece piece = context.Board.GetPiece(move.OriginIndex);
            Board board = context.Board;
            PieceTeam turn = context.Turn;
            int targetIndex = move.TargetIndex;
            switch (move.MoveType)
            {
                case MoveType.NONE:
                    board.Move(move);
                    break;
                case MoveType.CASTLING:
                    board.Move(move);
                    if (turn == PieceTeam.WHITE)
                    {
                        const int kingSideCastling = 62;
                        const int queenSideCastling = 58;

                        if (targetIndex == kingSideCastling)
                        {
                            Move rookMove = new Move(61, 63, MoveType.NONE);
                            board.Move(rookMove);
                        }
                        else if (targetIndex == queenSideCastling)
                        {
                            Move rookMove = new Move(59, 56, MoveType.NONE);
                            board.Move(rookMove);
                        }
                    }
                    else
                    {
                        const int kingSideCastling = 6;
                        const int queenSideCastling = 2;

                        if (targetIndex == kingSideCastling)
                        {
                            Move rookMove = new Move(5, 7, MoveType.NONE);
                            board.Move(rookMove);
                        }
                        else if (targetIndex == queenSideCastling)
                        {
                            Move rookMove = new Move(3, 0, MoveType.NONE);
                            board.Move(rookMove);
                        }
                    }
                    break;
                case MoveType.PROMOTION:
                    board.Move(move);
                    context.Promotion = board.GetSquare(move.TargetIndex);
                    break;
                case MoveType.DOBLE_STEP:
                    board.Move(move);
                    if (turn == PieceTeam.WHITE)
                    {
                        context.EnPassant = board.GetSquare(targetIndex + MoveDirection.Down);
                    }
                    else
                    {
                        context.EnPassant = board.GetSquare(targetIndex + MoveDirection.Up);
                    }
                    break;
                case MoveType.EN_PASSANT:
                    board.Move(move);
                    if (turn == PieceTeam.WHITE)
                    {
                        board.RemovePiece(targetIndex + MoveDirection.Down);
                    }
                    else
                    {
                        board.RemovePiece(targetIndex + MoveDirection.Up);
                    }
                    break;
            }
        }

        public void ProcessMove(Move move)
        {
            _move = move;
            Board board = context.Board;
            PieceTeam turn = context.Turn;
            int targetIndex = move.TargetIndex;
            int originIndex = move.OriginIndex;
            Piece? piece = board.GetPiece(originIndex);
            _currentFen = context.ToString();
            if (!board.ContainsEnemyPiece(move.TargetIndex, turn) && piece.Type != PieceType.PAWN)
            {
                context.HalfMoveClock += 1;
            }
            else
            {
                context.HalfMoveClock = 0;
            }

            // Borramos en passant
            context.EnPassant = null;

            // Si el rey se mueve, deshabilitamos el enroque
            if (piece.Type == PieceType.KING)
            {
                if (turn == PieceTeam.WHITE)
                {
                    context.KingSideCastlingWhite = false;
                    context.QueenSideCastlingWhite = false;
                }
                else
                {
                    context.KingSideCastlingBlack = false;
                    context.QueenSideCastlingBlack = false;
                }
            }

            // Si la torre se mueve, deshabilitamos el enroque
            if (piece.Type == PieceType.ROOK)
            {
                if (turn == PieceTeam.WHITE)
                {
                    if (originIndex == 56)
                    {
                        context.QueenSideCastlingWhite = false;
                    }
                    else if (originIndex == 63)
                    {
                        context.KingSideCastlingWhite = false;
                    }
                }
                else
                {
                    if (originIndex == 0)
                    {
                        context.QueenSideCastlingBlack = false;
                    }
                    else if (originIndex == 7)
                    {
                        context.KingSideCastlingBlack = false;
                    }
                }
            }
            // Si hay castling disponible, comprobamos si hemos capturado una torre
            if (turn == PieceTeam.WHITE)
            {
                
                if (context.KingSideCastlingBlack)
                {
                    if (move.TargetIndex == 7)
                    {
                        if (board.ContainsEnemyPiece(7, turn))
                        {
                            if (board.GetPiece(7).Type == PieceType.ROOK)
                            {
                                context.KingSideCastlingBlack = false;
                            }
                        }
                    }
                }

                if (context.QueenSideCastlingBlack)
                {
                    if (move.TargetIndex == 0)
                    {
                        if (board.ContainsEnemyPiece(0, turn))
                        {
                            if (board.GetPiece(0).Type == PieceType.ROOK)
                            {
                                context.QueenSideCastlingBlack = false;
                            }
                        }
                    }
                }
            }
            else
            {
                if (context.KingSideCastlingWhite)
                {
                    if (move.TargetIndex == 63)
                    {
                        if (board.ContainsEnemyPiece(63, turn))
                        {
                            if (board.GetPiece(63).Type == PieceType.ROOK)
                            {
                                context.KingSideCastlingWhite = false;
                            }
                        }
                    }
                }

                if (context.QueenSideCastlingWhite)
                {
                    if (move.TargetIndex == 56)
                    {
                        if (board.ContainsEnemyPiece(56, turn))
                        {
                            if (board.GetPiece(56).Type == PieceType.ROOK)
                            {
                                context.QueenSideCastlingWhite = false;
                            }
                        }
                    }
                }
            }

            switch (move.MoveType)
            {
                case MoveType.NONE:
                    board.Move(move);
                    break;
                case MoveType.CASTLING:
                    board.Move(move);
                    if (turn == PieceTeam.WHITE)
                    {
                        const int kingSideCastling = 62;
                        const int queenSideCastling = 58;

                        if (targetIndex == kingSideCastling)
                        {
                            Move rookMove = new Move(61, 63, MoveType.NONE);
                            board.Move(rookMove);
                        }
                        else if (targetIndex == queenSideCastling)
                        {
                            Move rookMove = new Move(59, 56, MoveType.NONE);
                            board.Move(rookMove);
                        }
                    }
                    else
                    {
                        const int kingSideCastling = 6;
                        const int queenSideCastling = 2;

                        if (targetIndex == kingSideCastling)
                        {
                            Move rookMove = new Move(5, 7, MoveType.NONE);
                            board.Move(rookMove);
                        }
                        else if (targetIndex == queenSideCastling)
                        {
                            Move rookMove = new Move(3, 0, MoveType.NONE);
                            board.Move(rookMove);
                        }
                    }
                    break;
                case MoveType.PROMOTION:
                    board.Move(move);
                    context.Promotion = board.GetSquare(move.TargetIndex);
                    break;
                case MoveType.DOBLE_STEP:
                    board.Move(move);
                    if (turn == PieceTeam.WHITE)
                    {
                        context.EnPassant = board.GetSquare(targetIndex + MoveDirection.Down);
                    }
                    else
                    {
                        context.EnPassant = board.GetSquare(targetIndex + MoveDirection.Up);
                    }
                    break;
                case MoveType.EN_PASSANT:
                    board.Move(move);
                    if (turn == PieceTeam.WHITE)
                    {
                        board.RemovePiece(targetIndex + MoveDirection.Down);
                    }
                    else
                    {
                        board.RemovePiece(targetIndex + MoveDirection.Up);
                    }
                    break;
            }

            // Si no es un movimiento de promoción, cambiamos el turno, en caso contrario habría que procesar la promoción
            if (move.MoveType != MoveType.PROMOTION)
            {
                UpdateContext();
            }
        }

        private void UpdateContext()
        {
            // Si es el turno de las negras, incrementamos el contador de movimientos
            if (context.Turn == PieceTeam.BLACK)
            {
                context.TotalMoves += 1;
            }

            // Grabamos el movimiento, la cadena fen vinculada con el movimiento debe ser
            // la anterior a la realización del propio movimiento
            if (_move != null)
            {
                context.MoveHistory.Add((_move, _currentFen));
            }

            // Cambiamos el turno
            context.ChangeTurn();


            MoveValidator moveValidator = new MoveValidator(context);


            // Comprobamos si hay jaque mate
            List<Move> allLegalMoves = moveValidator.GetAllAllyMoves();

            // Si hay jaque mate, ofrecemos la victoria al equipo contrario
            if (allLegalMoves.Count == 0)
            {
                bool isCheckMate = false;
                List<Move> enemyMoves = moveValidator.GetAllEnemyMoves(context);
                foreach (var move in enemyMoves)
                {
                    int targetIndex = move.TargetIndex;
                    if (context.Board.ContainsAllyPiece(targetIndex, context.Turn))
                    {
                        if (context.Board.GetPiece(targetIndex).Type == PieceType.KING)
                        {
                            isCheckMate = true;
                            break;
                        }
                    }
                }
                if (isCheckMate)
                {
                    context.State = context.Turn == PieceTeam.WHITE ? State.WIN_BLACK : State.WIN_WHITE;
                }
                else
                {
                    context.State = State.DRAW_STALEMATE;
                }

            }

            // TODO: Empate por ahogado => Para esto habría que modificar la detección de jaque mate para distinguirlo de ahogado

            // TODO: Empate por regla de los 50 movimientos

            if (context.HalfMoveClock == 50)
            {
                context.State = State.DRAW_FIFTY_MOVE_RULE;
            }

            // TODO: Empate por regla de los 3 movimientos repetidos
            if (context.HalfMoveClock >= 9)
            {
                int repetitionCount = 1;
                int historyCount = context.MoveHistory.Count();

                string[] fenParts = context.ToString().Split(" ");
                string fen = fenParts[0] + " " + fenParts[1] + " " + fenParts[2] + " " + fenParts[3];
                string[] fenPartsHistory1 = context.MoveHistory[historyCount - 4].Item2.Split(" ");
                string fenHistory1 = fenPartsHistory1[0] + " " + fenPartsHistory1[1] + " " + fenPartsHistory1[2] + " " + fenPartsHistory1[3];
                if (fen == fenHistory1)
                {
                    repetitionCount++;
                }
                string[] fenPartsHistory2 = context.MoveHistory[historyCount - 8].Item2.Split(" ");
                string fenHistory2 = fenPartsHistory2[0] + " " + fenPartsHistory2[1] + " " + fenPartsHistory2[2] + " " + fenPartsHistory2[3];
                if (fenHistory1 == fenHistory2)
                {
                    repetitionCount++;
                }

                if (repetitionCount == 3)
                {
                    context.State = State.DRAW_THREEFOLD_REPETITION;
                }
            }
            // TODO: Empate por falta de material
            List<Piece> whitePieces = context.Board.GetAllWhitePieces();
            List<Piece> blackPieces = context.Board.GetAllBlackPieces();
            int totalWhitePieces = whitePieces.Count;
            int totalBlackPieces = blackPieces.Count;
            int totalPieces = totalWhitePieces + totalBlackPieces;

            if (totalPieces == 2)
            {
                context.State = State.DRAW_INSUFFICIENT_MATERIAL;
            }
            else if (totalWhitePieces <= 2 && totalBlackPieces <= 2)
            {
                bool insufficientMaterial = true;
                foreach (Piece piece in whitePieces)
                {
                    if (piece.Type != PieceType.BISHOP && piece.Type != PieceType.KNIGHT && piece.Type != PieceType.KING)
                    {
                        insufficientMaterial = false;
                        break;
                    }
                }

                foreach (Piece piece in blackPieces)
                {
                    if (piece.Type != PieceType.BISHOP && piece.Type != PieceType.KNIGHT && piece.Type != PieceType.KING)
                    {
                        insufficientMaterial = false;
                        break;
                    }
                }

                if (insufficientMaterial)
                {
                    context.State = State.DRAW_INSUFFICIENT_MATERIAL;
                }
            }
        }

        public void ProcessPromotion(PieceType pieceType)
        {
            Board board = context.Board;
            Square promotionSquare = context.Promotion;
            PieceFactory factory = new PieceFactory();
            Piece piece = board.GetPiece(promotionSquare.SquareIndex);
            promotionSquare.SetPiece(factory.CreatePiece(piece.Team, pieceType));
            context.Promotion = null;

            UpdateContext();
        }
    }
}
