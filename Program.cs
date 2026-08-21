using Microsoft.Data.Sqlite;
using Google.GenAI;
using System.Text.Json;


class Program {

    static async Task Main(string[] args) {

        InitializeDatabase();//creates database with tables if it doesn't exist yet

        string prompt  = File.ReadAllText("prompt/prompt_implicit_final.txt");
        string schema  = File.ReadAllText("prompt/extraction-schema.json");
        string bericht = File.ReadAllText(args[0]);//requires to run the program with the filepath to the report as command parameter

        string input   = prompt + "\n\nHier ist das JSON-Schema:\n\n" + schema + "\n\nHier ist der zu analysierende Bericht:\n\n" + bericht;

        var client = new Client();
        var result = await client.Models.GenerateContentAsync("gemini-3.1-flash-lite" , input);

        //test code:
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string filename = "logs/logfile_" + timestamp + ".txt";

        File.WriteAllText(
            filename                            ,
            "*****Gesendet an Gemini:*****\n\n" +
            input                               +
            "\n\n*****Ergebnis:*****\n\n"       +
            result.Text
        );
        //end of test code

        Person person = JsonSerializer.Deserialize<Person>(result.Text);
        ImportPerson(person);
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

        string sql = "INSERT INTO Person (name, age) VALUES ($name, $age) RETURNING id";
        SqliteCommand command = new SqliteCommand(sql, connection);
        command.Parameters.AddWithValue("$name", person.Name);
        command.Parameters.AddWithValue("$age", person.Age is null ? DBNull.Value : person.Age);
        var id = command.ExecuteScalar();

        string taskSql = "INSERT INTO Task (person_id, description, date, time, notes) VALUES ($person_id, $description, $date, $time, $notes)";
        SqliteCommand taskCommand = new SqliteCommand(taskSql, connection);
        taskCommand.Parameters.AddWithValue("$person_id", id);
        SqliteParameter taskDescriptionParameter = taskCommand.Parameters.AddWithValue("$description", null);
        SqliteParameter taskDateParameter = taskCommand.Parameters.AddWithValue("$date", null);
        SqliteParameter taskTimeParameter = taskCommand.Parameters.AddWithValue("$time", null);
        SqliteParameter taskNotesParameter = taskCommand.Parameters.AddWithValue("$notes", null);

        foreach (var task in person.Tasks) {
            taskDescriptionParameter.Value = task.Description;
            taskDateParameter.Value = task.Date is null ? DBNull.Value : task.Date;
            taskTimeParameter.Value = task.Time is null ? DBNull.Value : task.Time;
            taskNotesParameter.Value = task.Notes is null ? DBNull.Value : task.Notes;
            taskCommand.ExecuteNonQuery();
        }

        string needSql = "INSERT INTO Need (person_id, category, description, implicit) VALUES ($person_id, $category, $description, $implicit)";
        SqliteCommand needCommand = new SqliteCommand(needSql, connection);
        needCommand.Parameters.AddWithValue("$person_id", id);
        SqliteParameter needCategoryParameter = needCommand.Parameters.AddWithValue("$category", null);
        SqliteParameter needDescriptionParameter = needCommand.Parameters.AddWithValue("$description", null);
        SqliteParameter needImplicitParameter = needCommand.Parameters.AddWithValue("$implicit", null);

        foreach (var needCategory in person.Needs) {
            foreach (var need in needCategory.Value) {
                needCategoryParameter.Value = needCategory.Key;
                needDescriptionParameter.Value = need.Description;
                needImplicitParameter.Value = need.Implicit;
                needCommand.ExecuteNonQuery();
            }
        }

        connection.Close();
        
    }
}