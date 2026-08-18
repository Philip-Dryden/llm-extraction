using Google.GenAI;

string prompt  = File.ReadAllText("prompt_explicit.txt");
string schema  = File.ReadAllText("extraction-schema.json");
string bericht = File.ReadAllText("bericht.txt");

string input   = prompt + "\n\nHier ist das JSON-Schema:\n\n" + schema + "\n\nHier ist der zu analysierende Bericht:\n\n" + bericht;

var client = new Client();
var result = await client.Models.GenerateContentAsync("gemini-3.1-flash-lite" , input);

File.WriteAllText(
    "logfile_implicit.txt"              ,
    "*****Gesendet an Gemini:*****\n\n" +
    input                               +
    "\n\n*****Ergebnis:*****\n\n"       +
    result
);