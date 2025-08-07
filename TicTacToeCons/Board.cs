using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToeCons
{
    public class Board
    {
        private char[] cells = new char[9];

        public Board()
        {
            for (int i = 0; i < 9; i++)
                cells[i] = ' ';
        }

        public void Display()
        {
            Console.Clear();
            Console.WriteLine();
            for (int i = 0; i < 9; i += 3)
            {
                Console.WriteLine($" {cells[i]} | {cells[i + 1]} | {cells[i + 2]} ");
                if (i < 6) Console.WriteLine("---+---+---");
            }
            Console.WriteLine();
        }

        public bool MakeMove(int index, char symbol)
        {
            if (index < 0 || index >= 9 || cells[index] != ' ') return false;
            cells[index] = symbol;
            return true;
        }

        public bool IsFull() => cells.All(c => c != ' ');

        public char CheckWinner()
        {
            int[,] wins = new int[,]
            {
            {0,1,2}, {3,4,5}, {6,7,8}, // rows
            {0,3,6}, {1,4,7}, {2,5,8}, // cols
            {0,4,8}, {2,4,6}           // diag
            };

            for (int i = 0; i < wins.GetLength(0); i++)
            {
                int a = wins[i, 0], b = wins[i, 1], c = wins[i, 2];
                if (cells[a] != ' ' && cells[a] == cells[b] && cells[b] == cells[c])
                    return cells[a];
            }

            return ' ';
        }

        public string GetBoardState() => new string(cells);

        public void LoadBoardState(string state)
        {
            for (int i = 0; i < 9; i++)
                cells[i] = state[i];
        }
    }
}
