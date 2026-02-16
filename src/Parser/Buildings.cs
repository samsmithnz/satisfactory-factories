using System.Text.Json;
using System.Text.RegularExpressions;

namespace Parser;

public static class Buildings
{
    // Function to extract all buildings that produce something
    public static List<string> GetProducingBuildings(JsonElement[] data)
    {
        HashSet<string> producingBuildingsSet = new();

        foreach (JsonElement entry in data)
        {
            if (!entry.TryGetProperty("Classes", out JsonElement classes))
                continue;

            foreach (JsonElement classEntry in classes.EnumerateArray())
            {
                if (classEntry.TryGetProperty("mProducedIn", out JsonElement producedIn))
                {
                    // Updated regex to capture building names inside quotes
                    MatchCollection matches = Regex.Matches(producedIn.GetString() ?? "", @"/(\w+)/(\w+)\.(\w+)_C");
                    
                    foreach (Match match in matches)
                    {
                        Match buildingMatch = Regex.Match(match.Value, @"/(\w+)\.(\w+)_C");
                        if (buildingMatch.Success)
                        {
                            // Remove "build_" prefix if present
                            string buildingName = buildingMatch.Groups[2].Value;
                            buildingName = buildingName.StartsWith("Build_") 
                                ? buildingName.Replace("Build_", "").ToLower() 
                                : buildingName.ToLower();
                            producingBuildingsSet.Add(buildingName);
                        }
                    }
                }
                
                // If a power generator
                if (classEntry.TryGetProperty("mFuel", out _))
                {
                    if (classEntry.TryGetProperty("ClassName", out JsonElement className))
                    {
                        string? name = Common.GetPowerProducerBuildingName(className.GetString() ?? "");
                        if (name == null)
                        {
                            throw new InvalidOperationException($"Could not extract building name for Power Recipe from {className.GetString()}");
                        }
                        producingBuildingsSet.Add(name);
                    }
                }
            }
        }

        return producingBuildingsSet.ToList();
    }

    // Function to extract the power consumption for each producing building
    public static Dictionary<string, double> GetPowerConsumptionForBuildings(JsonElement[] data, List<string> producingBuildings)
    {
        Dictionary<string, double> buildingsPowerMap = new();

        foreach (JsonElement entry in data)
        {
            if (!entry.TryGetProperty("Classes", out JsonElement classes))
                continue;

            foreach (JsonElement building in classes.EnumerateArray())
            {
                if (building.TryGetProperty("ClassName", out JsonElement className) &&
                    building.TryGetProperty("mPowerConsumption", out JsonElement powerConsumption))
                {
                    // Normalize the building name by removing "_C" and lowercasing it
                    string buildingName = className.GetString()?.Replace("_C", "").ToLower() ?? "";
                    buildingName = buildingName.Replace("build_", "");
                    buildingName = buildingName.Replace("_automated", "");

                    // Only include power data if the building is in the producingBuildings list
                    if (producingBuildings.Contains(buildingName))
                    {
                        double power = 0;
                        if (powerConsumption.ValueKind == JsonValueKind.String)
                        {
                            double.TryParse(powerConsumption.GetString(), out power);
                        }
                        else if (powerConsumption.ValueKind == JsonValueKind.Number)
                        {
                            power = powerConsumption.GetDouble();
                        }
                        buildingsPowerMap[buildingName] = power;
                    }
                }
            }
        }

        // Finally sort the map by key
        Dictionary<string, double> sortedMap = new();
        foreach (string key in buildingsPowerMap.Keys.OrderBy(k => k))
        {
            sortedMap[key] = buildingsPowerMap[key];
        }

        return sortedMap;
    }
}
