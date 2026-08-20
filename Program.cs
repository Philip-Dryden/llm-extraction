using Google.GenAI;

class Program {
    static async Task Main(string[] args) {

        string prompt  = File.ReadAllText("prompt/prompt_implicit_final.txt");
        string schema  = File.ReadAllText("prompt/extraction-schema.json");
        string bericht = File.ReadAllText("reports/bericht2.txt");

        string input   = prompt + "\n\nHier ist das JSON-Schema:\n\n" + schema + "\n\nHier ist der zu analysierende Bericht:\n\n" + bericht;

        var client = new Client();
        var result = await client.Models.GenerateContentAsync("gemini-3.1-flash-lite" , input);

        string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string filename = "logs/logfile_" + timestamp + ".txt";

        File.WriteAllText(
            filename                            ,
            "*****Gesendet an Gemini:*****\n\n" +
            input                               +
            "\n\n*****Ergebnis:*****\n\n"       +
            result.Text
        );
    }
}