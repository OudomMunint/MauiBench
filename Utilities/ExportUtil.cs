namespace MauiBench.Utilities
{
    class BenchmarkExporter
    {
        public string? BenchmarkVersion { get; set; }

        public static void ExportResults(string filename, string results, string benchtype, string version, string timestamp)
        {
            string appDirectory = AppContext.BaseDirectory;
            string filePath = Path.Combine(appDirectory, filename);
            string score = results.Length > 0 ? results : "No results available";
            string output = $"MauiBench v{version} Results\n" +
                           $"Timestamp: {timestamp}\n" +
                           $"{benchtype}: {score} pts\n" +
                           "----------------------------------------\n";

            File.AppendAllText(filePath, output);
            Console.WriteLine($"Results exported to {filePath}");
        }

        public static void TestExportResults()
        {
            List<string> testResults = new();
            testResults.Add("Hasing Benchmark: 1000ms.");
            testResults.Add("Encryption Benchmark: 2000ms.");
            testResults.Add("CPU Prime Computation: 3000ms.");
            testResults.Add("CPU Matrix Multiplication: 4000ms.");
            testResults.Add("Memory Bandwidth: 5000ms.");
            testResults.Add("Full run: 15000ms.");

            string results = string.Join("\n", testResults);
            string version = VersionTracking.Default.CurrentVersion;
            string timestamp = DateTime.UtcNow.ToString("o");
            string benchtype = Random.Shared.Next(0, 5) switch
            {
                0 => "Hashing",
                1 => "Encryption",
                2 => "CPU Prime Computation",
                3 => "CPU Matrix Multiplication",
                4 => "Memory Bandwidth",
                _ => "Full"
            };

            ExportResults($"MauiBench_{version}_TestResults.txt", results, benchtype, version, timestamp);
        }
    }
}