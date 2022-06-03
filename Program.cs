// See https://aka.ms/new-console-template for more information
using Microsoft.Data.Sqlite;
using System.Globalization;

namespace HabitTracker
{
    internal class Program
    {
        static string connectionString = @"Data Source=habitTracker.db";
        static void Main(string[] args)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                //create sql command to create database
                tableCmd.CommandText =
                    @"CREATE TABLE IF NOT EXISTS drinking_water(
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Date TEXT,
                    Quantity INTEGER
                    )";

                tableCmd.ExecuteNonQuery();

                connection.Close();
            }

            GetUserInput();
        }

        static void GetUserInput()
        {
            Console.Clear();
            bool closeApp = false;
            while (closeApp == false)
            {
                Console.WriteLine("\n\nMAIN MENU");
                Console.WriteLine("\nWhat would you like to do?");
                Console.WriteLine("\nType 0 to Close the Application.");
                Console.WriteLine("Type 1 to View All Records.");
                Console.WriteLine("Type 2 to Insert a Record.");
                Console.WriteLine("Type 3 to Delete a Record.");
                Console.WriteLine("Type 4 to Update a Record.");
                Console.WriteLine("-----------------------------------------\n");

                string commandInput = Console.ReadLine();

                switch (commandInput)
                {
                    case "0":
                        Console.WriteLine("\nGoodbye!\n");
                        closeApp = true;
                        break;
                    case "1":
                        GetAllRecords();
                        break;
                    case "2":
                        Insert();
                        break;
                        //case "3":
                        //    Delete();
                        //    break;
                        //case "4":
                        //    Update();
                        //    break;
                        //default:
                        //    Console.WriteLine("\nInvalid Entry.  Please Enter an integer between 0 and 4.\n");
                        //    break;
                }
            }
        }

        private static void GetAllRecords()
        {
            Console.Clear();
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText =
                    $"SELECT * FROM drinking_water";
                
                List<DrinkingWater> tableData = new();
                SqliteDataReader reader = tableCmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        tableData.Add(
                        new DrinkingWater 
                        {
                            Id = reader.GetInt32(0),
                            Date = DateTime.ParseExact(reader.GetString(1), "MM-dd-yy", new CultureInfo("en-US")),
                            Quantity = reader.GetInt32(2)
                        }); ;
                    }
                }
                else
                {
                    Console.WriteLine("No rows found!");
                }
 
                connection.Close();

                Console.WriteLine("--------------------------------------------\n");
                Console.WriteLine("Id#  -  Date  -  Quantity\n");
                foreach (var dw in tableData)
                {
                    Console.WriteLine($"{dw.Id} - {dw.Date.ToString("MM-dd-yy")} - {dw.Quantity} glasses");
                }
                Console.WriteLine("--------------------------------------------\n");
            }
        }

        private static void Insert()
        {
            string date = GetDateInput();

            int quantity = GetNumberInput("\n\nPlease insert numnber of glasses or other measure of your choice (no decimals allowed):");

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText =
                    $"INSERT INTO drinking_water(date, quantity) VALUES('{date}', {quantity})";

                tableCmd.ExecuteNonQuery();

                connection.Close();
            }

        }

        internal static int GetNumberInput(string message)
        {
            Console.WriteLine(message);
            string numberInput = Console.ReadLine();
            if (numberInput == "0") GetUserInput();
            int finalInput = Convert.ToInt32(numberInput);
            return finalInput;
        }

        internal static string GetDateInput()
        {
            Console.WriteLine("\n\nPlease insert the date: (Format: mm-dd-yy).  Type 0 to return to the main menu.");

            string dateInput = Console.ReadLine();

            if (dateInput == "0") GetUserInput();

            return dateInput;
        }

    }


}
