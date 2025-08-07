using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToeCons
{
    public enum PlayerType { Human, Computer }
    public class Player
    {
        public char Symbol { get; }
        public PlayerType Type { get; }

        public Player(char symbol, PlayerType type)
        {
            Symbol = symbol;
            Type = type;
        }
    }
}
