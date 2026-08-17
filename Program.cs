using Google.GenAI;

var client = new Client();

var result = await client.Models.GenerateContentAsync("gemini-3.1-flash-lite" , "Was ist 2 + 2?");

Console.WriteLine(result.Text);