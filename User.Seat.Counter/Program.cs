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
    Console.WriteLine($"There was an error reading the log file: {ex.Message}");
    return;
}

string[] lines = fileContents.Split(new[] { '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries);

int lineNumber = 0;

// A Dictionary inside of a Dictionary to keep track of users and the products they have checked out.
Dictionary<string, Dictionary<string, int>> userProductSeats = [];
// user, product, seat count.

foreach (string line in lines)
{
    lineNumber++;

    string correctedLine = line.Trim();

    if (correctedLine.Contains("OUT:") || correctedLine.Contains("IN:"))
    {
        try
        {
            string[] lineParts = correctedLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string productName = lineParts[3];
            string[] userAndHostnameParts = lineParts[4].Split(new[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
            string user = userAndHostnameParts[0];

            // Initialize dictionary for the user if it doesn't exist.
            if (!userProductSeats.TryGetValue(user, out var productSeats))
            {
                productSeats = [];
                userProductSeats[user] = productSeats;
            }

            // Initialize seat count for the product if it doesn't exist.
            if (!productSeats.TryGetValue(productName, out int seatCount))
            {
                seatCount = 0;
                productSeats[productName] = seatCount;
            }

            if (lineParts[2] == "OUT:") // A seat is being checked out.
            {
                productSeats[productName] = ++seatCount;
            }
            else if (lineParts[2] == "IN:") // A seat is being checked in.
            {
                productSeats[productName] = --seatCount;
            }
            else
            {
                Console.WriteLine($"IN: or OUT: not detected as the third line part. Line number in question: {lineNumber}. Line part: {lineParts[2]}. Exiting.");
                Environment.Exit(1);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"There was an error attempting to calculate seat count: {ex.Message}");
            return;
        }
    }
}

// Print out the results.
foreach (var userEntry in userProductSeats)
{
    string user = userEntry.Key;
    Console.WriteLine($"User: {user}");

    foreach (var productEntry in userEntry.Value)
    {
        string product = productEntry.Key;
        int seatCount = productEntry.Value;

        if (seatCount == 1)
        {
            Console.WriteLine($"  {product}: {seatCount} seat checked out.");
        }
        else
        {
            Console.WriteLine($"  {product}: {seatCount} seats checked out.");
        }
    }
}