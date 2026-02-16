using Parser;

// Get command line arguments for input and output files
if (args.Length < 2)
{
    Console.Error.WriteLine("Usage: dotnet run <input-file> <output-file>");
    Environment.Exit(1);
    return;
}

string inputFile = args[0];
string outputFile = args[1];

// Run the recipe processing
await Processor.ProcessFile(inputFile, outputFile);
Console.WriteLine("Processing complete");

