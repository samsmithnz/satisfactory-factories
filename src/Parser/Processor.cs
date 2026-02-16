using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Parser.Models;

namespace Parser;

public static class Processor
{
    // Function to detect if the file is UTF-16
    private static async Task<bool> IsUtf16(string inputFile)
    {
        byte[] buffer = await File.ReadAllBytesAsync(Path.GetFullPath(inputFile));
        bool bomLE = buffer.Length >= 2 && buffer[0] == 0xFF && buffer[1] == 0xFE;
        bool bomBE = buffer.Length >= 2 && buffer[0] == 0xFE && buffer[1] == 0xFF;
        return bomLE || bomBE;
    }

    // Function to read UTF-16 and convert to UTF-8
    private static async Task<string> ReadFileAsUtf8(string inputFile)
    {
        bool isUtf16Encoding = await IsUtf16(inputFile);
        if (isUtf16Encoding)
        {
            byte[] buffer = await File.ReadAllBytesAsync(Path.GetFullPath(inputFile));
            string content = Encoding.Unicode.GetString(buffer);
            return NormalizeLineEndings(content);
        }
        else
        {
            string content = await File.ReadAllTextAsync(Path.GetFullPath(inputFile), Encoding.UTF8);
            return NormalizeLineEndings(content);
        }
    }

    // Helper function to normalize line endings
    private static string NormalizeLineEndings(string content)
    {
        return content.Replace("\r\n", "\n");
    }

    // Function to clean up the input file to make it valid JSON
    private static string CleanInput(string input)
    {
        string cleaned = input.Replace("\r\n", "\n");
        cleaned = Regex.Replace(cleaned, @",\s*([\]}])", "$1");
        return cleaned;
    }

    private static void RemoveRubbishItems(ParserItemDataInterface items, List<ParserRecipe> recipes)
    {
        // Create a Set to store all product keys from recipes
        HashSet<string> recipeProducts = new();

        // Loop through each recipe to collect all product keys
        foreach (ParserRecipe recipe in recipes)
        {
            foreach (ParserProduct product in recipe.Products)
            {
                recipeProducts.Add(product.Part);
            }
            foreach (ParserIngredient ingredient in recipe.Ingredients)
            {
                recipeProducts.Add(ingredient.Part);
            }
        }

        // Loop through each item in items.parts and remove any entries that do not exist in recipeProducts
        List<string> keysToRemove = new();
        foreach (string part in items.Parts.Keys)
        {
            if (!recipeProducts.Contains(part))
            {
                keysToRemove.Add(part);
            }
        }

        foreach (string key in keysToRemove)
        {
            items.Parts.Remove(key);
        }
    }

    // Central function to process the file and generate the output
    public static async Task<ProcessFileResult?> ProcessFile(string inputFile, string outputFile)
    {
        try
        {
            string fileContent = await ReadFileAsUtf8(inputFile);
            string cleanedContent = CleanInput(fileContent);
            JsonDocument document = JsonDocument.Parse(cleanedContent);
            
            // Convert to array of JsonElements
            JsonElement root = document.RootElement;
            JsonElement[] data = root.EnumerateArray().ToArray();

            // Get parts
            ParserItemDataInterface items = Parts.GetItems(data);
            Parts.FixItemNames(items);

            // Get an array of all buildings that produce something
            List<string> producingBuildings = Buildings.GetProducingBuildings(data);

            // Get power consumption for the producing buildings
            Dictionary<string, double> buildings = Buildings.GetPowerConsumptionForBuildings(data, producingBuildings);

            // Pass the producing buildings with power data to getRecipes to calculate perMin and powerPerProduct
            List<ParserRecipe> recipes = Recipes.GetProductionRecipes(data, buildings);
            RemoveRubbishItems(items, recipes);
            Parts.FixTurbofuel(items, recipes);

            //IMPORTANT: The order here matters - don't run this before fixing the turbofuel.
            List<ParserPowerRecipe> powerGenerationRecipes = Recipes.GetPowerGeneratingRecipes(data, items);

            // Since we've done some manipulation of the items data, re-sort it
            Dictionary<string, ParserPart> sortedItems = new();
            foreach (string key in items.Parts.Keys.OrderBy(k => k))
            {
                sortedItems[key] = items.Parts[key];
            }
            items = items with { Parts = sortedItems };

            // Construct the final result
            ProcessFileResult finalData = new()
            {
                Buildings = buildings,
                Items = items,
                Recipes = recipes,
                PowerGenerationRecipes = powerGenerationRecipes
            };

            // Write the output to the file
            string json = JsonSerializer.Serialize(finalData, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            await File.WriteAllTextAsync(Path.GetFullPath(outputFile), json);
            
            Console.WriteLine($"Processed {items.Parts.Count} parts, {buildings.Count} buildings, and {recipes.Count} recipes have been written to {outputFile}.");

            return finalData;
        }
        catch (Exception error)
        {
            Console.Error.WriteLine($"Error processing file: {error.Message}");
            return null;
        }
    }
}

public record ProcessFileResult
{
    public required Dictionary<string, double> Buildings { get; init; }
    public required ParserItemDataInterface Items { get; init; }
    public required List<ParserRecipe> Recipes { get; init; }
    public required List<ParserPowerRecipe> PowerGenerationRecipes { get; init; }
}
