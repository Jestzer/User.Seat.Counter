if (args.Length == 0 || string.IsNullOrWhiteSpace(args[0]))
{
    Console.WriteLine("To use this program, specify the full path to the log file with or without quotes.");
    return;
}
else if (args.Contains("-version"))
{
    Console.WriteLine("Version 1.0");
    Environment.Exit(0);
}

string logFilePath = args[0];

// Read the file contents.
string? fileContents;
try
{
    fileContents = File.ReadAllText(logFilePath);
}
catch (Exception ex)
{
    Console.WriteLine($"There was an error reading the license file: {ex.Message}");
    return;
}

string[] lines = fileContents.Split(new[] { '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries);

int lineNumber = 0;
Dictionary<string, int> productSeats = [];

foreach (string line in lines)
{
    lineNumber++;

    string correctedLine = line.Trim();

    if (correctedLine.Contains("OUT:") || correctedLine.Contains("IN:"))
    {
        string[] lineParts = correctedLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        string productName = lineParts[3];

        if (!productSeats.TryGetValue(productName, out int value))
        {
            value = 0;
            productSeats[productName] = value; // Initialize seat count for new products
        }

        if (lineParts[2] == "OUT:") // Determine if a seat is being checked IN or OUT.
        {
            productSeats[productName] = ++value;
        }
        else if (lineParts[2] == "IN:")
        {
            productSeats[productName] = --value;
        }
        else
        {
            Console.WriteLine($"IN: or OUT: not detected as the third line part. Line number in question: {lineNumber}. Line part: {lineParts[2]}. Exiting.");
            Environment.Exit(1);
        }
    }
}

foreach (var entry in productSeats)
{
    if (entry.Value == 1)
    {
        Console.WriteLine($"{entry.Key} has {entry.Value} seat checked out.");
    }
    else
    {
        Console.WriteLine($"{entry.Key} has {entry.Value} seats checked out.");
    }
}