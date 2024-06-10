using ChessLibrary.Engine;
using ChessLibrary.Engine.Movement;
using ChessLibrary.Exceptions;
using ChessLibrary.Models.Pieces;
using Stockfish.NET;

namespace ChessLibrary.UITests
{
    public static class ChessAIGame
    {
        public static void Start(PieceTeam playerTeam, string fen = "", PieceTeam turn = PieceTeam.WHITE)
        {
            IStockfish stockfish = new Stockfish.NET.Stockfish(@"stockfish.exe");
            ChessGame chess;
            if (string.IsNullOrEmpty(fen))
            {
                chess = new ChessGame();
            }
            else
            {
                chess = new ChessGame(fen);
            }
            ChessUI UI = new ChessUI();

            string errorMessage = "";
            string selectTile = "";
            string moveTile = "";

            while (chess.State == State.IN_PROGRESS)
            {
                if (chess.Turn == playerTeam)
                {
                    try
                    {
                        UI.ShowSelectionLayout(chess.Board.GetTileLayout(), chess.Turn, errorMessage);
                        selectTile = Console.ReadLine();
                        selectTile = selectTile.ToUpper();


                        chess.SelectSquare(selectTile);
                        List<Move> legalMoves = chess.CurrentSquareMoves;
                        errorMessage = "";
                        bool validInput = false;

                        do
                        {
                            UI.ShowMoveLayout(chess.Board.GetTileLayout(), chess.Turn, legalMoves, errorMessage);
                            moveTile = Console.ReadLine();
                            moveTile = moveTile.ToUpper();
                            errorMessage = "";

                            try
                            {
                                if (moveTile == "X")
                                {
                                    chess.DeselectSquare();
                                    errorMessage = "";
                                    validInput = true;
                                }
                                else
                                {
                                    chess.MoveSquare(moveTile);
                                    while (chess.Promotion != null)
                                    {
                                        UI.ShowPromotion(chess.Board.GetTileLayout(), errorMessage);
                                        string promoteFen = Console.ReadLine();
                                        try
                                        {
                                            chess.Promote(promoteFen);
                                            errorMessage = "";
                                        }
                                        catch (ArgumentException)
                                        {
                                            errorMessage = $"\"{promoteFen}\" no es una pieza válida";
                                        }
                                    }

                                    validInput = true;
                                }
                            }
                            catch (IllegalMoveException)
                            {
                                errorMessage = $"No se puede realizar un movimiento a \"{moveTile}\"";
                            }
                            catch (ArgumentException)
                            {
                                errorMessage = $"La casilla \"{selectTile}\" no existe";
                            }

                        } while (!validInput);

                    }
                    catch (ArgumentException)
                    {
                        errorMessage = $"La casilla \"{selectTile}\" no existe";
                    }
                    catch (IllegalTileException)
                    {
                        errorMessage = $"La casilla \"{selectTile}\" está vacía o contiene una pieza enemiga";
                    }
                    catch (NoLegalMovesException)
                    {
                        errorMessage = $"No hay ningún movimiento disponible en la casilla \"{selectTile}\"";
                    }
                }
                else
                {
                    string fullFen = chess.ToString();
                    stockfish.SetFenPosition(fullFen);
                    string stockfishMove = stockfish.GetBestMove();
                    string selectStockFishTile = stockfishMove.Substring(0, 2);
                    string moveStockGishTile = stockfishMove.Substring(2, 2);
                    chess.SelectSquare(selectStockFishTile);
                    chess.MoveSquare(moveStockGishTile);
                    if (chess.Promotion != null)
                    {
                        chess.Promote("q");
                    }
                }
            }
            UI.ShowLayout(chess.Board.GetTileLayout());
            if (chess.State == State.WIN_WHITE)
            {
                Console.WriteLine("VICTORIA BLANCAS");
            }
            else Console.WriteLine("VICTORIA NEGRAS");
            Console.ReadKey();
        }
    }
}
