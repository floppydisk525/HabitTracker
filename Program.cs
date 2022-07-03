// See https://aka.ms/new-console-template for more information
using Microsoft.Data.Sqlite;
using System.Globalization;
using ConsoleValidationLibrary;

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

                //string commandInput = Console.ReadLine();
                string commandInput = RegexKeyValidate.KeyValidate(@"^[01234]*$");

                switch (commandInput)
                {
                    case "0":
                        Console.WriteLine("\nGoodbye!\n");
                        closeApp = true;
                        Environment.Exit(0);
                        break;
                    case "1":
                        GetAllRecords();
                        break;
                    case "2":
                        Insert();
                        break;
                    case "3":
                        Delete();
                        break;
                    case "4":
                        Update();
                        break;
                    default:
                        Console.WriteLine("\nInvalid Entry.  Please Enter an integer between 0 and 4.\n");
                        break;
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

        private static void Delete()
        {
            Console.Clear();
            GetAllRecords();

            var recordId = GetNumberInput("\n\nPlease provide the record ID of the record you want to delete or type 0 to go back to the main menu\n\n");

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText = $"DELETE FROM drinking_water WHERE Id = '{recordId}'";

                int rowCount = tableCmd.ExecuteNonQuery();

                if (rowCount == 0)    //issue w/ this that if we always return 0, will get in infinite loop..
                {
                    Console.WriteLine($"\n\nRecord with Id# {recordId} doesn't exist. Please choose again. \n\n");
                    Task.Delay(2000).Wait();
                    Delete();           //possible infinite loop 'here'...  
                }
            }

            Console.WriteLine($"\n\nRecord with Id# {recordId} has been deleted. \n\n");
            GetUserInput();
        }

        private static void Update()
        {
            Console.Clear();
            GetAllRecords();

            var recordId = GetNumberInput("\n\nPlease provide the record ID of the record you want to update or type 0 to go back to the main menu\n\n");

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var checkCmd = connection.CreateCommand();
                checkCmd.CommandText = $"SELECT EXISTS(SELECT 1 FROM drinking_water WHERE Id = {recordId})";
                int checkQuery = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (checkQuery == 0)
                {
                    Console.WriteLine($"\n\nRecord with Id# {recordId} doesn't exist. Please choose again. \n\n");
                    Task.Delay(2000).Wait();
                    connection.Close();
                    Update();
                }

                string date = GetDateInput();

                int quantity = GetNumberInput("\n\nPlease insert number of glasses or other measure of your choice (no decimals allowed).\n\n");

                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText = $"UPDATE drinking_water SET date = '{date}', quantity = '{quantity}' WHERE id = {recordId}";

                tableCmd.ExecuteNonQuery();
                connection.Close();
            }

            Console.WriteLine($"\n\nRecord with Id# {recordId} has been deleted. \n\n");
            GetUserInput();
        }

        internal static int GetNumberInput(string message)
        {
            Console.WriteLine(message);
            //string numberInput = Console.ReadLine();
            string numberInput = RegexKeyValidate.KeyValidate("num");
            if (numberInput == "0") GetUserInput();

            while(!Int32.TryParse(numberInput,out _) || Convert.ToInt32(numberInput) < 0)
            {
                Console.WriteLine("\n\nInvalid Number.  Try again.\n\n");
                //numberInput = Console.ReadLine();
                numberInput = RegexKeyValidate.KeyValidate("num");
            }

            int finalInput = Convert.ToInt32(numberInput);
            return finalInput;
        }

        internal static string GetDateInput()
        {
            Console.WriteLine("\n\nPlease insert the date: (Format: mm-dd-yy).  Type 0 to return to the main menu.");

            //string dateInput = Console.ReadLine();
            string dateInput = RegexKeyValidate.KeyValidate(@"^[0-9-]*$");

            if (dateInput == "0") GetUserInput();

            while (!DateTime.TryParseExact(dateInput,"MM-dd-yy",new CultureInfo("en-US"), DateTimeStyles.None,out _))
            {
                Console.WriteLine("\n\nInvalid Date.  (Format:  mm-dd-yy).  Type 0 to return to the main menu or try again:\n\n");
                //dateInput = Console.ReadLine();
                dateInput = RegexKeyValidate.KeyValidate(@"^[0-9-]*$");
            }

            return dateInput;
        }
    }

}
