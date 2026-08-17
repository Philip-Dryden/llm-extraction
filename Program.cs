using Google.GenAI;

string input = File.ReadAllText("input.txt");

var client = new Client();

var result = await client.Models.GenerateContentAsync("gemini-3.1-flash-lite" , input);

File.WriteAllText("result.txt", result.Text);

Console.WriteLine(result.Text);