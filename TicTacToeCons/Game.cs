using System;
using System.Linq;

namespace TicTacToeCons
{
    public class Game
    {
        private Board board;
        private Player playerX;
        private Player playerO;
        private Player currentPlayer;

        public Game()
        {
            board = new Board();
            playerX = new Player('X', PlayerType.Human);
            playerO = new Player('O', PlayerType.Computer);
            currentPlayer = playerX;
        }

        public void ShowMainMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Головне меню ===");
                Console.WriteLine("1. Нова гра");

                if (DatabaseManager.HasSavedGame())
                {
                    var lastSave = DatabaseManager.GetLastSaveDate();
                    string dateStr = lastSave.HasValue ? lastSave.Value.ToString("g") : "невідомо";
                    Console.WriteLine($"2. Продовжити збережену гру (остання збереження: {dateStr})");
                }

                Console.WriteLine("0. Вийти");
                Console.Write("Виберіть опцію: ");

                string choice = Console.ReadLine();

                if (choice == "1")
                {
                    board = new Board();
                    currentPlayer = playerX;
                    StartGameLoop();
                    break;
                }
                else if (choice == "2" && DatabaseManager.HasSavedGame())
                {
                    LoadSavedGame();
                    StartGameLoop();
                    break;
                }
                else if (choice == "0")
                {
                    Console.WriteLine("Вихід з гри...");
                    Environment.Exit(0);
                }
                else
                {
                    Console.WriteLine("Невірний вибір. Спробуйте ще раз.");
                    Console.ReadKey();
                }
            }
        }

        private void StartGameLoop()
        {
            Random rnd = new Random();

            while (true)
            {
                Console.Clear();
                board.Display();
                Console.WriteLine($"Ход гравця: {currentPlayer.Symbol}");
                Console.WriteLine("Введіть номер клітинки (1-9), або команду (save / exit / help):");

                int move = -1;

                if (currentPlayer.Type == PlayerType.Human)
                {
                    Console.Write("Введіть номер клітинки або команду: ");
                    string inputStr = Console.ReadLine()?.ToLower();

                    if (inputStr == "save")
                    {
                        DatabaseManager.SaveGame(board.GetBoardState(), currentPlayer.Symbol);
                        Console.WriteLine("Гру збережено.");
                        Console.ReadKey();
                        continue;
                    }
                    else if (inputStr == "exit")
                    {
                        Console.WriteLine("Вихід з гри...");
                        return;
                    }
                    else if (inputStr == "help")
                    {
                        Console.WriteLine("Інструкція:");
                        Console.WriteLine("- Введіть номер клітинки від 1 до 9, щоб зробити хід.");
                        Console.WriteLine("- save — зберегти гру.");
                        Console.WriteLine("- exit — вийти без збереження.");
                        Console.WriteLine("- help — показати цю інструкцію.");
                        Console.ReadKey();
                        continue;
                    }

                    if (int.TryParse(inputStr, out int input))
                        move = input - 1;

                    if (move < 0 || move > 8 || !board.MakeMove(move, currentPlayer.Symbol))
                    {
                        Console.WriteLine("Невірний хід. Спробуйте ще раз.");
                        Console.ReadKey();
                        continue;
                    }
                }
                else
                {
                    do
                    {
                        move = rnd.Next(9);
                    } while (!board.MakeMove(move, currentPlayer.Symbol));

                    Console.WriteLine($"Комп'ютер обрав клітинку: {move + 1}");
                    Console.ReadKey();
                }

                DatabaseManager.SaveGame(board.GetBoardState(), currentPlayer.Symbol);

                char winner = board.CheckWinner();
                if (winner != ' ')
                {
                    Console.Clear();
                    board.Display();
                    Console.WriteLine($"Переміг {winner}!");
                    DatabaseManager.ClearSavedGame();
                    Console.ReadKey();
                    break;
                }

                if (board.IsFull())
                {
                    Console.Clear();
                    board.Display();
                    Console.WriteLine("Нічия!");
                    DatabaseManager.ClearSavedGame();
                    Console.ReadKey();
                    break;
                }

                currentPlayer = currentPlayer == playerX ? playerO : playerX;
            }
        }

        private void LoadSavedGame()
        {
            (string state, char turn) = DatabaseManager.LoadGame();
            board.LoadBoardState(state);

            currentPlayer = playerX;
        }
    }
}