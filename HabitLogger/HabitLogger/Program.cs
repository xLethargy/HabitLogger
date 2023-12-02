using System.Data.SQLite;
using System.IO;

namespace HabitLogger
{
    internal class Program
    {
        static void Main()
        {

            string connectionString = @"Data Source=..\..\..\Files\Library.db;Version=3;";

            if (!File.Exists(@"..\..\..\Files\Library.db"))
            {
                SQLiteConnection.CreateFile(@"..\..\..\Files\Library.db");


                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    var tableCmd = connection.CreateCommand();

                    tableCmd.CommandText =
                        @"CREATE TABLE IF NOT EXISTS drinking_water (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Date TEXT,
                    Quantity INTEGER
                    )";

                    tableCmd.ExecuteNonQuery();

                    connection.Close();
                }
            }
        }
    }
}