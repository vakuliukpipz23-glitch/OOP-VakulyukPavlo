using System.Diagnostics;
using System.Text;

class Program
{
    static async Task Main(string[] args)
    {
        string filePath = "biglog.txt";
        string filteredPath = "errors.txt";

        // 1. Генерація файлу
        Console.WriteLine("Generating file...");
        GenerateFile(filePath, 1_000_000);

        // 2. Синхронне читання
        Console.WriteLine("\nSynchronous processing...");
        var swSync = Stopwatch.StartNew();
        int syncCount = ProcessSync(filePath);
        swSync.Stop();

        Console.WriteLine($"Sync ERROR count: {syncCount}");
        Console.WriteLine($"Sync time: {swSync.ElapsedMilliseconds} ms");

        // 3. Асинхронне читання + запис
        Console.WriteLine("\nAsynchronous processing...");
        var swAsync = Stopwatch.StartNew();
        int asyncCount = await ProcessAsync(filePath, filteredPath);
        swAsync.Stop();

        Console.WriteLine($"Async ERROR count: {asyncCount}");
        Console.WriteLine($"Async time: {swAsync.ElapsedMilliseconds} ms");

        Console.WriteLine($"\nFiltered file saved to: {filteredPath}");
    }

    // -------------------------------
    // 1. Генератор великого файлу
    // -------------------------------
    static void GenerateFile(string path, int lines)
    {
        var rand = new Random();
        string[] levels = { "INFO", "WARN", "ERROR" };

        using var writer = new StreamWriter(path);

        for (int i = 0; i < lines; i++)
        {
            string level = levels[rand.Next(levels.Length)];
            writer.WriteLine($"{DateTime.Now:O} | {level} | Message {i}");
        }
    }

    // -------------------------------
    // 2. Синхронна обробка
    // -------------------------------
    static int ProcessSync(string path)
    {
        int errorCount = 0;

        using var reader = new StreamReader(path);

        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();

            if (line != null && line.Contains("ERROR"))
            {
                errorCount++;
            }
        }

        return errorCount;
    }

    // -------------------------------
    // 3. Асинхронна обробка + запис
    // -------------------------------
    static async Task<int> ProcessAsync(string inputPath, string outputPath)
    {
        int errorCount = 0;

        using var reader = new StreamReader(inputPath);
        using var writer = new StreamWriter(outputPath);

        string? line;

        while ((line = await reader.ReadLineAsync()) != null)
        {
            if (line.Contains("ERROR"))
            {
                errorCount++;
                await writer.WriteLineAsync(line);
            }
        }

        return errorCount;
    }
}