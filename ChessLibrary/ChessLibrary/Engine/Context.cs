using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessLibrary.Engine.Movement;
using ChessLibrary.Models;
using ChessLibrary.Models.Pieces;

namespace ChessLibrary.Engine
{
    public class Context
    {
        public Context()
        {
            Board = new Board();
            Turn = PieceTeam.WHITE;
            KingSideCastlingWhite = true;
            QueenSideCastlingWhite = true;
            KingSideCastlingBlack = true;
            QueenSideCastlingBlack = true;
            EnPassant = null;
            HalfMoveClock = 0;
            TotalMoves = 1;
            State = State.IN_PROGRESS;
            Promotion = null;
            MoveHistory = new List<(Move, string)>();
        }

        public Context(string fen)
        {
            string[] fenParts = fen.Split(' ');
            string boardFen = fenParts[0];
            string turn = fenParts[1];
            string castling = fenParts[2];
            string enPassant = fenParts[3];
            string halfMoveClock = fenParts[4];
            string totalMoves = fenParts[5];

            Board = new Board(boardFen);
            Turn = turn == "w" ? PieceTeam.WHITE : PieceTeam.BLACK;
            KingSideCastlingWhite = castling.Contains("K");
            QueenSideCastlingWhite = castling.Contains("Q");
            KingSideCastlingBlack = castling.Contains("k");
            QueenSideCastlingBlack = castling.Contains("q");
            EnPassant = enPassant == "-" ? null : Board.GetSquare(enPassant);
            HalfMoveClock = int.Parse(halfMoveClock);
            TotalMoves = int.Parse(totalMoves);

            // TODO: Implementar el estado del juego y promotion
            State = State.IN_PROGRESS;
            Promotion = null;
            MoveHistory = new List<(Move, string)>();
        }
        public Board Board { get; set; }
        public PieceTeam Turn { get; set; }
        public bool KingSideCastlingWhite { get; set; }
        public bool QueenSideCastlingWhite { get; set; }
        public bool KingSideCastlingBlack { get; set; }
        public bool QueenSideCastlingBlack { get; set; }
        public Square? EnPassant { get; set; }
        public int HalfMoveClock { get; set; }
        public int TotalMoves { get; set; }
        public State State { get; set; }
        public Square? Promotion { get; set; }
        public List<(Move, string)> MoveHistory { get; set; }

        public void ChangeTurn()
        {
            Turn = Turn == PieceTeam.WHITE ? PieceTeam.BLACK : PieceTeam.WHITE;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Board.GetFen());
            sb.Append(" ");
            string turn = Turn == PieceTeam.WHITE ? "w" : "b";
            sb.Append(turn);
            sb.Append(" ");
            if (!KingSideCastlingWhite && !QueenSideCastlingWhite && !KingSideCastlingBlack && !QueenSideCastlingBlack)
            {
                sb.Append("-");
            }
            else
            {
                if (KingSideCastlingWhite)
                    sb.Append("K");
                if (QueenSideCastlingWhite)
                    sb.Append("Q");
                if (KingSideCastlingBlack)
                    sb.Append("k");
                if (QueenSideCastlingBlack)
                    sb.Append("q");
            }
            sb.Append(" ");
            sb.Append(EnPassant?.SquarePosition ?? "-");
            sb.Append(" ");
            sb.Append(HalfMoveClock);
            sb.Append(" ");
            sb.Append(TotalMoves);

            return sb.ToString();
        }
    }
}
