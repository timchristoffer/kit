using System.Diagnostics;

public class ESLintAnalyzer
{
    public static string Analyze(string code)
    {
        // Skapa en tillfällig fil för ESLint att analysera
        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, code);

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "node",
                Arguments = $"\"C:\\path\\to\\eslint\\cli.js\" --stdin < \"{tempFile}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        string result = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        File.Delete(tempFile); // Rensa tillfällig fil
        return result;
    }
}
