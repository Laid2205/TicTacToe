using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace TicTacToeCons
{
    public class DatabaseManager
    {
        private static string connectionString = @"Server=localhost;Database=TicTacDB;Integrated Security=True;Encrypt=False;Trusted_Connection=True;";

        public static void Initialize()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var cmd = new SqlCommand(@"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='SavedGame' AND xtype='U')
                CREATE TABLE SavedGame (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    Board NVARCHAR(9) NOT NULL,
                    Turn CHAR(1) NOT NULL,
                    IsFinished BIT NOT NULL,
                    SaveDate DATETIME NULL
                );", connection);

                cmd.ExecuteNonQuery();
            }
        }

        public static bool HasSavedGame()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var cmd = new SqlCommand("SELECT COUNT(*) FROM SavedGame WHERE IsFinished = 0;", connection);
                int count = (int)cmd.ExecuteScalar();

                return count > 0;
            }
        }

        public static void SaveGame(string board, char turn)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var insertCmd = new SqlCommand("INSERT INTO SavedGame (Board, Turn, IsFinished) VALUES (@board, @turn, 0);", connection);
                insertCmd.Parameters.AddWithValue("@board", board);
                insertCmd.Parameters.AddWithValue("@turn", turn);
                insertCmd.ExecuteNonQuery();
            }
        }

        public static (string, char) LoadGame()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var cmd = new SqlCommand("SELECT TOP 1 Board, Turn FROM SavedGame WHERE IsFinished = 0;", connection);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string board = reader.GetString(0);
                        char turn = reader.GetString(1)[0];
                        return (board, turn);
                    }
                }
            }

            return (new string(' ', 9), 'X');
        }

        public static void ClearSavedGame()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var cmd = new SqlCommand("UPDATE SavedGame SET IsFinished = 1 WHERE IsFinished = 0;", connection);
                cmd.ExecuteNonQuery();
            }
        }
        public static DateTime? GetLastSaveDate()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var cmd = new SqlCommand("SELECT MAX(SaveDate) FROM SavedGame WHERE IsFinished = 0;", connection);
                var result = cmd.ExecuteScalar();

                if (result != DBNull.Value && result != null)
                    return (DateTime)result;
                else
                    return null;
            }
        }
    }
}
