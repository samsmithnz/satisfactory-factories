using Web.Models;
using Web.Models.Factory;

namespace Web.Services.FactoryManagement;

/// <summary>
/// Service for common factory management utilities.
/// </summary>
public class FactoryCommonService : IFactoryCommonService
{
    /// <inheritdoc/>
    public void CreateNewPart(Factory factory, string partId)
    {
        if (!factory.Parts.ContainsKey(partId))
        {
            factory.Parts[partId] = new PartMetrics
            {
                AmountRequired = 0,
                AmountRequiredExports = 0,
                AmountRequiredProduction = 0,
                AmountRequiredPower = 0,
                AmountSupplied = 0,
                AmountSuppliedViaInput = 0,
                AmountSuppliedViaRaw = 0,
                AmountSuppliedViaProduction = 0,
                AmountRemaining = 0,
                Satisfied = true,
                IsRaw = false,
                Exportable = false
            };
        }
    }

    /// <inheritdoc/>
    public Recipe? GetRecipe(string recipeId, GameData gameData)
    {
        Recipe? recipe = gameData.Recipes.FirstOrDefault(r => r.Id == recipeId);

        if (recipe == null)
        {
            Console.Error.WriteLine($"Recipe with ID {recipeId} not found.");
            return null;
        }

        return recipe;
    }

    /// <inheritdoc/>
    public string GetPartDisplayName(string partId, GameData gameData)
    {
        if (string.IsNullOrEmpty(partId))
        {
            return "NO PART!!!";
        }

        if (gameData == null)
        {
            Console.Error.WriteLine("getPartDisplayName: No game data!!");
            return "NO DATA!!!";
        }

        if (gameData.Items.RawResources.TryGetValue(partId, out RawResource? rawResource))
        {
            return rawResource.Name;
        }

        if (gameData.Items.Parts.TryGetValue(partId, out Part? part))
        {
            return part.Name;
        }

        return $"UNKNOWN PART {partId}!";
    }

    /// <inheritdoc/>
    public PowerRecipe? GetPowerRecipeById(string recipeId, GameData gameData)
    {
        if (gameData == null || string.IsNullOrEmpty(recipeId))
        {
            return null;
        }

        return gameData.PowerGenerationRecipes.FirstOrDefault(recipe => recipe.Id == recipeId);
    }

    /// <inheritdoc/>
    public string GetBuildingDisplayName(string buildingId)
    {
        Dictionary<string, string> buildingFriendly = new Dictionary<string, string>
        {
            { "assemblermk1", "Assembler" },
            { "blender", "Blender" },
            { "constructormk1", "Constructor" },
            { "converter", "Converter" },
            { "foundrymk1", "Foundry" },
            { "hadroncollider", "Particle Accelerator" },
            { "generatorbiomass", "Biomass Burner" },
            { "generatorcoal", "Coal-Powered Generator" },
            { "generatorfuel", "Fuel-Powered Generator" },
            { "generatornuclear", "Nuclear Power Plant" },
            { "manufacturermk1", "Manufacturer" },
            { "oilrefinery", "Oil Refinery" },
            { "packager", "Packager" },
            { "quantumencoder", "Quantum Encoder" },
            { "smeltermk1", "Smelter" },
            { "waterExtractor", "Water Extractor" }
        };

        if (buildingFriendly.TryGetValue(buildingId, out string? friendlyName))
        {
            return friendlyName;
        }

        return $"UNKNOWN BUILDING: {buildingId}";
    }
}
