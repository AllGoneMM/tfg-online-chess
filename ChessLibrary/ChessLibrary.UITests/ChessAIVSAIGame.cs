using ChessLibrary.Engine;
using ChessLibrary.Engine.Movement;
using ChessLibrary.Models.Pieces;
using Stockfish.NET;

namespace ChessLibrary.UITests
{
    public static class ChessAIVSAIGame
    {
        public static void Start()
        {
            Random random = new Random();
            IStockfish stockfishAI1 = new Stockfish.NET.Stockfish(@"stockfish.exe");
            

            ChessGame chess = new ChessGame();
            ChessUI chessUI = new ChessUI();
            int depth = random.Next(1, 20);
            stockfishAI1.Depth = depth;
            while (chess.State == State.IN_PROGRESS)
            {
                
                chessUI.ShowLayout(chess.Board.GetTileLayout());
                string fullFen = chess.ToString();


                stockfishAI1.SetFenPosition(fullFen);
                string stockfishMove = stockfishAI1.GetBestMove();
                MoveResult result = chess.Move(stockfishMove);
                if (!result.IsSuccessful())
                {
                    throw new Exception("Invalid move");
                }


                if (chess.Promotion != null)
                {
                    chess.Promote("q");
                }
            }
            chessUI.ShowLayout(chess.Board.GetTileLayout());
            if (chess.State == State.WIN_BLACK)
            {
                Console.WriteLine("VICTORIA NEGRAS");
                foreach (var history in chess.MoveHistory)
                {
                    Console.WriteLine(history);
                }

            }
            else if(chess.State == State.WIN_WHITE)
            {
                Console.WriteLine("VICTORIA BLANCAS");
                foreach (var history in chess.MoveHistory)
                {
                    Console.WriteLine(history);
                }
            }
            else if (chess.State == State.DRAW_FIFTY_MOVE_RULE)
            {
                Console.WriteLine("EMPATE 50 MOVIMIENTOS");
                foreach (var history in chess.MoveHistory)
                {
                    Console.WriteLine(history);
                }
            }
            else if (chess.State == State.DRAW_INSUFFICIENT_MATERIAL)
            {
                Console.WriteLine("EMPATE POR INSUFICIENCIA DE MATERIAL");
                foreach (var history in chess.MoveHistory)
                {
                    Console.WriteLine(history);
                }
            }
            else if (chess.State == State.DRAW_THREEFOLD_REPETITION)
            {
                Console.WriteLine("EMPATE POR REPETICION DE 3 MOVIMIENTOS");
                foreach (var history in chess.MoveHistory)
                {
                    Console.WriteLine(history);
                }
            }
        }
    }
}
