using Microsoft.Data.Sqlite;
using Google.GenAI;
using System.Text.Json;


class Program {
    
    static async Task Main(string[] args) {
        Console.WriteLine("DB-Test:");
        Console.WriteLine("InitializeDatabase():");
        InitializeDatabase();
        Console.WriteLine("Erfolgreich!");
    }


    static void InitializeDatabase() {
        string connectionPath = "Data Source=data/database.db";
        SqliteConnection connection = new SqliteConnection(connectionPath);
        connection.Open();

        string[] sqlStatements = {
            "CREATE TABLE IF NOT EXISTS Person(id INTEGER PRIMARY KEY, name TEXT, age INTEGER)",
            "CREATE TABLE IF NOT EXISTS Need(id INTEGER PRIMARY KEY, person_id INTEGER REFERENCES Person(id), category TEXT, description TEXT, implicit BOOLEAN)",
            "CREATE TABLE IF NOT EXISTS Task(id INTEGER PRIMARY KEY, person_id INTEGER REFERENCES Person(id), description TEXT, date TEXT, time TEXT, notes TEXT)"
        };

        foreach (string sql in sqlStatements) {
        SqliteCommand command = new SqliteCommand(sql, connection);
        command.ExecuteNonQuery();
        }
        connection.Close();
    }

    static void ImportPerson(Person person) {

        string connectionPath = "Data Source=data/database.db";
        SqliteConnection connection = new SqliteConnection(connectionPath);
        connection.Open();

    }
}