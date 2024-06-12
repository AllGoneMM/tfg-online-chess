using System.Text;
using System.Text.RegularExpressions;
using ChessLibrary.Engine.Movement;
using ChessLibrary.Exceptions;
using ChessLibrary.Models;
using ChessLibrary.Models.Pieces;

namespace ChessLibrary.Utils
{
    public static class FenConverter
    {
        public const string StartingFenString = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        public static Square[] ToSquareArray(string fenString)
        {

            Square[] layout = new Square[64];
            for (int i = 0; i < layout.Length; i++)
            {
                layout[i] = new Square(i);
            }

            string[] fenParts = fenString.Split(' ');
            string[] boardParts = fenParts[0].Split('/');
            int squareIndex = 0;
            PieceFactory factory = new PieceFactory();
            foreach (string part in boardParts)
            {
                foreach (char partComponent in part)
                {
                    if (char.IsDigit(partComponent))
                    {
                        squareIndex += int.Parse(partComponent.ToString());
                    }
                    else
                    {
                        layout[squareIndex].SetPiece(factory.CreatePiece(partComponent));
                        squareIndex++;
                    }
                }
            }
            return layout;
        }

        public static string ToFenString(Square[] layout)
        {
            StringBuilder fenStringBuilder = new StringBuilder();
            int emptyCount = 0;

            for (int row = 0; row < 8; row++)
            {
                for (int column = 0; column <= 7; column++)
                {
                    int squareIndex = row * 8 + column;
                    if (layout[squareIndex].IsEmpty()) emptyCount++;
                    else
                    {
                        if (emptyCount > 0)
                        {
                            fenStringBuilder.Append(emptyCount.ToString());
                            emptyCount = 0;
                        }
                        Piece? piece = layout[squareIndex].Piece;
                        if (piece != null) fenStringBuilder.Append(piece);
                    }
                }
                if (emptyCount > 0)
                {
                    fenStringBuilder.Append(emptyCount.ToString());
                    emptyCount = 0;
                }
                if (row < 7)
                {
                    fenStringBuilder.Append("/");
                }
            }
            return fenStringBuilder.ToString();
        }

        public static bool IsFenValid(string fenString)
        {
            bool isFenValid = false;
            string fenRegEx = @"^([rnbqkpRNBQKP1-8]{1,8}/){7}[rnbqkpRNBQKP1-8]{1,8}\s[w|b]\s([-]|[K|Q|k|q]{1,4})\s([-]|[a-h][1-8])\s\d+\s\d+$";
            if (Regex.IsMatch(fenString, fenRegEx))
            {
                isFenValid = true;
            }
            return isFenValid;
        }
    }
}
