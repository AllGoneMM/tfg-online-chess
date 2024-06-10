using System.Diagnostics.SymbolStore;
using ChessLibrary.Engine;
using ChessLibrary.Engine.Movement;
using ChessLibrary.Exceptions;
using ChessLibrary.Utils;

namespace ChessLibrary.UnitTests
{
    public class ChessGameTests
    {
        [Test]
        public void Instance_With_Fen_Throws_FenFormatException()
        {
            // Random string
            string fen = "Random string";
            Assert.Throws<FenFormatException>(() => new ChessGame(fen));

            // Invalid board part
            string fen2 = "rnnr/pppppppp/8/8/88/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
            Assert.Throws<FenFormatException>(() => new ChessGame(fen2));

            // Missing turn part
            string fen3 = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR KQkq - 1";
            Assert.Throws<FenFormatException>(() => new ChessGame(fen3));

            // Incorrect turn part
            string fen4 = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR m KQkq - 1";
            Assert.Throws<FenFormatException>(() => new ChessGame(fen4));

            // Missing castling part
            string fen5 = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w - 1";
            Assert.Throws<FenFormatException>(() => new ChessGame(fen5));

            // Incorrect castling part
            string fen6 = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkqz - 1";
            Assert.Throws<FenFormatException>(() => new ChessGame(fen6));

            // Missing en passant part
            string fen7 = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq 1";
            Assert.Throws<FenFormatException>(() => new ChessGame(fen7));

            // Incorrect en passant part
            string fen8 = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq z 1";
            Assert.Throws<FenFormatException>(() => new ChessGame(fen8));

            // Missing half move clock part
            string fen9 = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 1";
            Assert.Throws<FenFormatException>(() => new ChessGame(fen9));

            // Incorrect half move clock part
            string fen10 = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - z 1";
            Assert.Throws<FenFormatException>(() => new ChessGame(fen10));

            // Missing total moves part
            string fen11 = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0";
            Assert.Throws<FenFormatException>(() => new ChessGame(fen11));

            // Incorrect total moves part
            string fen12 = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 z";
            Assert.Throws<FenFormatException>(() => new ChessGame(fen12));
        }

        [Test]
        public void Instance_With_Null_Fen_Throws_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new ChessGame(null));
        }

        [Test]
        public void Instance_Without_Fen_Success()
        {
            ChessGame game = new ChessGame();
            Assert.IsNotNull(game);
            Assert.AreEqual(FenConverter.StartingFenString, game.ToString());
        }

        [Test]
        public void Instance_With_Fen_Success()
        {
            string fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
            ChessGame game = new ChessGame(fen);
            Assert.IsNotNull(game);
            Assert.AreEqual(fen, game.ToString());

            fen = "bnrbnkqr/p1p2pp1/6pK/1p1pp3/1P1N1N2/2P4R/2PPPPPP/B2B2Q1 b - - 0 1";
            game = new ChessGame(fen);
            Assert.IsNotNull(game);
            Assert.AreEqual(fen, game.ToString());

            fen = "bnrbnkqr/p4pp1/6pK/1pppp3/1P1N1N2/2P4R/2PPPPPP/B2B2Q1 w - c6 0 1";
            game = new ChessGame(fen);
            Assert.IsNotNull(game);
            Assert.AreEqual(fen, game.ToString());

            fen = "bnrbnkqr/p4pp1/6pK/1pppp3/1P1N1N2/2P4R/2PPPPPP/B2B2Q1 w - c6 20 30";
            game = new ChessGame(fen);
            Assert.IsNotNull(game);
            Assert.AreEqual(fen, game.ToString());
        }

        [Test]
        public void Chess_History_Test()
        {
            ChessGame game = new ChessGame();
            game.Move("e2e4");
            game.Move("c7c5");
            game.Move("g1f3");
            game.Move("b7b5");
            game.Move("a2a3");
            string history1 = "e2e4 rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
            string history2 = "c7c5 rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1";
            string history3 = "g1f3 rnbqkbnr/pp1ppppp/8/2p5/4P3/8/PPPP1PPP/RNBQKBNR w KQkq c6 0 2";
            string history4 = "b7b5 rnbqkbnr/pp1ppppp/8/2p5/4P3/5N2/PPPP1PPP/RNBQKB1R b KQkq - 1 2";
            string history5 = "a2a3 rnbqkbnr/p2ppppp/8/1pp5/4P3/5N2/PPPP1PPP/RNBQKB1R w KQkq b6 0 3";
            List<string> chessHistory = game.MoveHistory;

            Assert.AreEqual(history1, chessHistory[0]);
            Assert.AreEqual(history2, chessHistory[1]);
            Assert.AreEqual(history3, chessHistory[2]);
            Assert.AreEqual(history4, chessHistory[3]);
            Assert.AreEqual(history5, chessHistory[4]);


        }

        [Test]
        public void Draw_By_ThreefoldRepetition()
        {
            ChessGame game = new ChessGame("8/6p1/p1k2pP1/Pp3P2/1P6/3K4/8/8 w - - 0 1");
            game.Move("d3d4");
            game.Move("c6d6");
            game.Move("d4e4");
            game.Move("d6c6");
            game.Move("e4d4");
            game.Move("c6d6");
            game.Move("d4e4");
            game.Move("d6c6");
            Assert.AreEqual(State.IN_PROGRESS, game.State);
            game.Move("e4d4");

            Assert.AreEqual(State.DRAW_THREEFOLD_REPETITION, game.State);

            game = new ChessGame("8/6p1/p1k2pP1/Pp3P2/1P6/3K4/8/8 w - - 0 1");
            game.Move("d3d4");
            game.Move("c6d6");
            game.Move("d4e4");
            game.Move("d6c6");
            game.Move("e4d4");
            game.Move("c6d6");
            game.Move("d4d3");
            game.Move("d6d7");
            game.Move("d3d4");
            Assert.AreEqual(State.IN_PROGRESS, game.State);
            game.Move("d7d6");

            Assert.AreEqual(State.DRAW_THREEFOLD_REPETITION, game.State);

            game = new ChessGame("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
            game.Move("b1c3");
            game.Move("b8c6");
            game.Move("c3b1");
            game.Move("c6b8");
            game.Move("b1c3");
            game.Move("b8c6");
            game.Move("c3b1");
            game.Move("c6b8");
            Assert.AreEqual(State.IN_PROGRESS, game.State);
            game.Move("b1c3");
            Assert.AreEqual(State.DRAW_THREEFOLD_REPETITION, game.State);


            game = new ChessGame("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
            game.Move("b1c3");
            game.Move("b8c6");
            game.Move("c3b1");
            game.Move("c6b8");
            game.Move("b1c3");
            game.Move("b8c6");
            game.Move("c3b1");
            game.Move("c6b8");
            Assert.AreEqual(State.IN_PROGRESS, game.State);
            game.Move("b1a3");
            Assert.AreEqual(State.IN_PROGRESS, game.State);
        }

        [Test]
        public void EnPassant_Tests()
        {
            ChessGame game = new ChessGame("8/8/8/5k2/6Pp/5K1P/8/8 b - g3 0 40");
            MoveResult result = game.Move("h4g3");
            Assert.That(result.IsSuccessful());
        }

        [Test]
        public void Stalemate_Tests()
        {
            ChessGame game = new ChessGame("K7/8/8/8/1q6/8/7b/2k5 b - - 0 1");
            game.Move("b4b6");
            Assert.AreEqual(State.DRAW_STALEMATE, game.State);
        }
    }
}