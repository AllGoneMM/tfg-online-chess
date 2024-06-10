using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLibrary.Engine.Movement
{
    public class MoveDirection
    {
        public const int Down = +8;

        public const int Up = -8;

        public const int Right = +1;

        public const int Left = -1;

        public const int DownRight = Down + Right;

        public const int DownLeft = Down + Left;

        public const int UpRight = Up + Right;

        public const int UpLeft = Up + Left;
    }
}
