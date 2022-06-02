// See https://aka.ms/new-console-template for more information
using Microsoft.Data.Sqlite;

string connectionString = @"Data Source=habitTracker.db";

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
