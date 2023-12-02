using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Reflection.PortableExecutable;

namespace HabitLogger
{
    internal class Program
    {
        public static string connectionString = @"Data Source=..\..\..\Files\Library.db;Version=3;";
        static void Main()
        {
            CreateDatabase();
            GetUserInput();
        }

        static void CreateDatabase()
        {
            if (!File.Exists(@"..\..\..\Files\Library.db"))
            {
                SQLiteConnection.CreateFile(@"..\..\..\Files\Library.db");


                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    SQLiteCommand tableCmd = connection.CreateCommand();

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

        static void GetUserInput()
        {
            
            bool closeApp = false;
            do
            {
                Console.Clear();
                Console.WriteLine("\nMAIN MENU");
                Console.WriteLine("\nWhat would you like to do?");
                Console.WriteLine("\nType 0 to Close Application");
                Console.WriteLine("Type 1 to View All Records");
                Console.WriteLine("Type 2 to Insert Record");
                Console.WriteLine("Type 3 to Delete Record");
                Console.WriteLine("Type 4 to Update Record");
                Console.WriteLine("------------------------------------------\n");

                string userInput = Console.ReadLine();

                switch(userInput)
                {
                    case "0":
                        closeApp = true;
                        break;
                    case "1":
                        GetAllRecords();
                        Console.ReadLine();
                        break;
                    case "2":
                        InsertRecord();
                        Console.ReadLine();
                        break;
                    case "3":
                        DeleteRecord();
                        Console.ReadLine();
                        break;
                    case "4":
                        UpdateRecord();
                        Console.ReadLine();
                        break;

                }
            } while (!closeApp);
            
                
        }

        static void UpdateRecord()
        {
            GetAllRecords();

            int updateId = GetNumberInput("\nPlease type the Id of the record you want to update or type 0 to go back to Main Menu");
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                SQLiteCommand checkCmd = connection.CreateCommand();
                checkCmd.CommandText = $"SELECT EXISTS(SELECT 1 FROM drinking_water WHERE Id = {updateId})";

                int checkQuery = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (checkQuery == 0)
                {
                    Console.WriteLine($"\nRecord with Id {updateId} does not exist\n");

                    connection.Close();
                    UpdateRecord();
                }

                Console.Write("\nPlease enter the date: ");
                string date = GetDateInput();

                int quantity = GetNumberInput("\nPlease enter the quantity");

                SQLiteCommand tableCmd = connection.CreateCommand();
                tableCmd.CommandText = $"UPDATE drinking_water SET date = '{date}', quantity = {quantity} WHERE Id = {updateId}";

                tableCmd.ExecuteNonQuery();
                connection.Close();
            }
        }

        static void DeleteRecord()
        {
            GetAllRecords();

            int deleteId = GetNumberInput("\nPlease type the Id of the record you want to delete or type 0 to go back to Main Menu");

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                SQLiteCommand tableCmd = connection.CreateCommand();
                tableCmd.CommandText = $"DELETE from drinking_water WHERE Id = '{deleteId}'";

                int rowCount = tableCmd.ExecuteNonQuery();

                if (rowCount == 0)
                {
                    Console.WriteLine($"\nRecord with Id {deleteId} does not exist\n");
                    
                    DeleteRecord();
                }
                else
                {
                    GetAllRecords();
                }
            }
        }

        static void GetAllRecords()
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                SQLiteCommand tableCmd = connection.CreateCommand();
                tableCmd.CommandText =
                    $"SELECT * FROM drinking_water ";

                List<DrinkingWater> tableData = new List<DrinkingWater>();

                SQLiteDataReader reader = tableCmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        tableData.Add(new DrinkingWater
                        {
                            Id = reader.GetInt32(0),
                            Date = DateTime.ParseExact(reader.GetString(1), "dd/MM/yyyy", new CultureInfo("en-UK")),
                            Quantity = reader.GetInt32(2)
                        });
                    }
                }
                else
                {
                    Console.WriteLine("No rows found");
                }

                connection.Close();

                foreach (DrinkingWater row in tableData)
                {
                    Console.WriteLine($"ID: {row.Id} - Date: {row.Date.ToString("dd/MM/yyyy")} - Quantity: {row.Quantity}");
                }
                Console.WriteLine("------------------------------------------\n");
            }
        }

        static void InsertRecord()
        {
            string date = GetDateInput();

            int quantity = GetNumberInput("\nPlease insert number of glasses (no decimals)");

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                SQLiteCommand tableCmd = connection.CreateCommand();
                tableCmd.CommandText = 
                    $"INSERT INTO drinking_water(date, quantity) VALUES('{date}', {quantity})";

                tableCmd.ExecuteNonQuery();

                connection.Close();
            }
        }

        static int GetNumberInput(string message)
        {
            Console.WriteLine(message);

            string numberInput = Console.ReadLine();

            if (numberInput == "0") GetUserInput();

            bool inputCheck = int.TryParse(numberInput, out int finalInput);

            if (inputCheck == false)
            {
                GetNumberInput("\nInvalid input. Please a valid number (no decimal)");
                return 0;
            }

            return finalInput;
        }

        static string GetDateInput()
        {
            Console.WriteLine("\nWould you like to enter a custom date (Y) Custom Date (N) Current Date. Type 0 to return to main menu");
            string yesOrNo = Console.ReadLine().ToLower();

            if (yesOrNo == "0") GetUserInput();

            if (yesOrNo == "n")
            {
                DateTime dateTime = DateTime.Now;

                return(dateTime.ToString("dd/MM/yyyy"));
            }
            else
            {
                Console.WriteLine("\nPlease insert the date: (Format: dd/mm/yyyy)");

                string dateInput = Console.ReadLine();

                if (dateInput == "0") GetUserInput();

                return dateInput;
            }

            
        }
    }
    public class DrinkingWater
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int Quantity { get; set; }

    }
}