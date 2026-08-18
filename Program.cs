using Google.GenAI;

string prompt  = File.ReadAllText("prompt_implicit5.txt");
string schema  = File.ReadAllText("extraction-schema.json");
string bericht = File.ReadAllText("bericht.txt");

string input   = prompt + "\n\nHier ist das JSON-Schema:\n\n" + schema + "\n\nHier ist der zu analysierende Bericht:\n\n" + bericht;

var client = new Client();
var result = await client.Models.GenerateContentAsync("gemini-3.1-flash-lite" , input);

string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
string filename = "logfile_" + timestamp + ".txt";

File.WriteAllText(
    filename                            ,
    "*****Gesendet an Gemini:*****\n\n" +
    input                               +
    "\n\n*****Ergebnis:*****\n\n"       +
    result.Text
);