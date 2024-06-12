namespace ChessLibrary.Utils;

public class SquareConverter
{
    public static int ToTileNumber(String tileString)
    {
        if (tileString.Length != 2) throw new ArgumentException();

        char rowChar = tileString[0];
        char columnChar = char.ToLower(tileString[1]);

        if (!"12345678".Contains(rowChar)) throw new ArgumentException();
        if (!"abcdefgh".Contains(columnChar)) throw new ArgumentException();

        int row = 0;
        switch (rowChar)
        {
            case '8': row = 1; break;
            case '7': row = 2; break;
            case '6': row = 3; break;
            case '5': row = 4; break;
            case '4': row = 5; break;
            case '3': row = 6; break;
            case '2': row = 7; break;
            case '1': row = 8; break;
        }

        int column = 0;
        switch (columnChar)
        {
            case 'a': column = 8; break;
            case 'b': column = 7; break;
            case 'c': column = 6; break;
            case 'd': column = 5; break;
            case 'e': column = 4; break;
            case 'f': column = 3; break;
            case 'g': column = 2; break;
            case 'h': column = 1; break;
        }
        return (row * 8) - column;
    }
}