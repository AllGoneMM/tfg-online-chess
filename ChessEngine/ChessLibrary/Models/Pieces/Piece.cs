using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessLibrary.Engine;
using ChessLibrary.Engine.Movement;

namespace ChessLibrary.Models.Pieces
{
    public abstract class Piece
    {
        private static readonly Dictionary<PieceType, char> PieceTypeToChar = new Dictionary<PieceType, char>
        {
            { PieceType.PAWN, 'P' },
            { PieceType.ROOK, 'R' },
            { PieceType.KNIGHT, 'N' },
            { PieceType.BISHOP, 'B' },
            { PieceType.QUEEN, 'Q' },
            { PieceType.KING, 'K' }
        };

        public PieceTeam Team { get; set; }

        public PieceType Type { get; set; }

        public Piece(PieceTeam team, PieceType type)
        {
            this.Team = team;
            this.Type = type;
        }


        public abstract List<Move> GetLegalMoves(Context context, int originIndex);

        // TODO: Cambiar todas estas propiedades, la manera de determinar esto es a través de la clase Context

        public string GetFenRepresentation()
        {
            string fenRepresentation = string.Empty;
            if (this.Team == PieceTeam.WHITE)
            {
                switch (this.Type)
                {
                    case PieceType.BISHOP: fenRepresentation = "B"; break;
                    case PieceType.KNIGHT: fenRepresentation = "N"; break;
                    case PieceType.PAWN: fenRepresentation = "P"; break;
                    case PieceType.KING: fenRepresentation = "K"; break;
                    case PieceType.QUEEN: fenRepresentation = "Q"; break;
                    case PieceType.ROOK: fenRepresentation = "R"; break;
                }
            }
            else
            {
                switch (this.Type)
                {
                    case PieceType.BISHOP: fenRepresentation = "b"; break;
                    case PieceType.KNIGHT: fenRepresentation = "n"; break;
                    case PieceType.PAWN: fenRepresentation = "p"; break;
                    case PieceType.KING: fenRepresentation = "k"; break;
                    case PieceType.QUEEN: fenRepresentation = "q"; break;
                    case PieceType.ROOK: fenRepresentation = "r"; break;
                }
            }

            return fenRepresentation;
        }

        public override string ToString()
        {
            return Team == PieceTeam.WHITE ? PieceTypeToChar[Type].ToString() : char.ToLower(PieceTypeToChar[Type]).ToString();
        }

        public static explicit operator char(Piece? v)
        {
            return char.Parse(v.ToString());
        }
    }
}
