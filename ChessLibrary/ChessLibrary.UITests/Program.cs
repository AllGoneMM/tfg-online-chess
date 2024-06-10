using ChessLibrary.Engine;
using ChessLibrary.Engine.Movement;
using ChessLibrary.Models.Pieces;
using ChessLibrary.Utils;
using Stockfish.NET;

namespace ChessLibrary.UITests
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //StartMenu();
            StressTest();
        }

        public static void StartMenu()
        {
            string option = "0";
            bool exit = false;
            do
            {
                Console.Clear();
                Console.WriteLine("MENU\n");
                Console.WriteLine("1 - Jugar contra IA\n");
                Console.WriteLine("2 - Jugar en local 1 vs 1\n");
                Console.WriteLine("3 - IA vs IA\n");
                Console.WriteLine("4 - Salir\n");
                Console.Write("\nIntroduce una opción (1-4): ");
                option = Console.ReadLine();
                switch (option)
                {
                    case "1":
                        StartAIGame();
                        break;
                    case "2":
                        StartLocalGame();
                        break;
                    case "3":
                        ChessAIVSAIGame.Start();
                        Console.ReadKey();
                        break;
                    case "4":
                        exit = true;
                        break;
                }

            } while (!exit);
        }

        public static void StartLocalGame()
        {
            bool startWithFen = false;
            bool startWithoutFen = false;
            string fen = "STARTING FEN";
            string turn = "TURN STRING";
            PieceTeam team = PieceTeam.NONE;
            do
            {
                Console.Clear();
                Console.Write("Pulsa ENTER para comenzar una nueva partida o introduce una cadena FEN: ");
                fen = Console.ReadLine();
                if (FenConverter.IsFenValid(fen))
                {
                    startWithFen = true;
                }
                else if (string.IsNullOrEmpty(fen))
                {
                    startWithoutFen = true;
                }
            } while (!startWithFen && !startWithoutFen);

            if (startWithFen)
            {
                do
                {
                    Console.Clear();
                    Console.Write("Introduce el equipo que empezará jugando (Blancas: b | Negras: n): ");
                    turn = Console.ReadLine();
                    turn = turn.ToLower();
                    if (turn == "b")
                    {
                        team = PieceTeam.WHITE;
                    }
                    else if (turn == "n")
                    {
                        team = PieceTeam.BLACK;
                    }

                } while (team == PieceTeam.NONE);
            }

            ChessLocalGame.Start(fen, team);
        }

        public static void StartAIGame()
        {
            bool startWithFen = false;
            bool startWithoutFen = false;
            string fen = "STARTING FEN";
            string turn = "TURN STRING";
            PieceTeam team = PieceTeam.NONE;
            PieceTeam playerTeam = PieceTeam.NONE;
            do
            {
                Console.Clear();
                Console.Write("Pulsa ENTER para comenzar una nueva partida o introduce una cadena FEN: ");
                fen = Console.ReadLine();
                if (FenConverter.IsFenValid(fen))
                {
                    startWithFen = true;
                }
                else if (string.IsNullOrEmpty(fen))
                {
                    startWithoutFen = true;
                }
            } while (!startWithFen && !startWithoutFen);

            if (startWithFen)
            {
                do
                {
                    Console.Clear();
                    Console.Write("Introduce el equipo que empezará jugando (Blancas: b | Negras: n): ");
                    turn = Console.ReadLine();
                    turn = turn.ToLower();
                    if (turn == "b")
                    {
                        team = PieceTeam.WHITE;
                    }
                    else if (turn == "n")
                    {
                        team = PieceTeam.BLACK;
                    }

                } while (team == PieceTeam.NONE);
            }
            while (playerTeam == PieceTeam.NONE)
            {
                Console.Clear();
                Console.Write("Introduce el equipo con el que vas a jugar (Blancas: b | Negras: n): ");
                turn = Console.ReadLine();
                turn = turn.ToLower();
                if (turn == "b")
                {
                    playerTeam = PieceTeam.WHITE;
                }
                else if (turn == "n")
                {
                    playerTeam = PieceTeam.BLACK;
                }
            }
            ChessAIGame.Start(playerTeam, fen, team);
        }

        public static void StressTest()
        {
            Random random = new Random();
            IStockfish stockfishAI1 = new Stockfish.NET.Stockfish(@"stockfish.exe");
            IStockfish stockfishAI2 = new Stockfish.NET.Stockfish(@"stockfish.exe");
            ChessGame chess = new ChessGame();

            stockfishAI1.Depth = random.Next(1, 20);
            stockfishAI2.Depth = random.Next(1, 20);
            int gameCount = 0;
            while (true)
            {
                string stockfishMove = "";
                string fullFen = chess.ToString();
                if (chess.Turn == PieceTeam.WHITE)
                {
                    stockfishAI1.SetFenPosition(fullFen);
                    stockfishMove = stockfishAI1.GetBestMove();
                }
                else
                {
                    stockfishAI2.SetFenPosition(fullFen);
                    stockfishMove = stockfishAI2.GetBestMove();
                }

                MoveResult result = chess.Move(stockfishMove);
                if (!result.IsSuccessful())
                {
                    throw new Exception("Invalid move");
                }

                if (chess.Promotion != null)
                {
                    chess.Promote("q");
                }

                if (chess.State != State.IN_PROGRESS)
                {
                    string stateString = GetStateString(chess);
                    Console.WriteLine(gameCount + " - " + stateString + " " + chess ) ;
                    gameCount++;
                    stockfishAI1.Depth = random.Next(1, 20);
                    stockfishAI2.Depth = random.Next(1, 20);
                    chess = new ChessGame();
                }
            }
        }

        private static string GetStateString(ChessGame chess)
        {
            State state = chess.State;
            switch (state)
            {
                case State.WIN_BLACK:
                    return "VICTORIA NEGRAS";
                case State.WIN_WHITE:
                    return "VICTORIA BLANCAS";
                case State.DRAW_FIFTY_MOVE_RULE:
                    return "EMPATE 50 MOVIMIENTOS";
                case State.DRAW_THREEFOLD_REPETITION:
                    return "EMPATE POR REPETICIÓN";
                case State.DRAW_STALEMATE:
                    return "EMPATE POR AHOGADO";
                case State.DRAW_INSUFFICIENT_MATERIAL:
                    return "EMPATE POR INSUFICIENCIA DE MATERIAL";
                default:
                    return "ERROR";
            }
        }
    }
}
