using System.Text.RegularExpressions;

namespace Parser;

public static class Common
{
    // Blacklist for excluding items produced by the Build Gun
    public static readonly List<string> Blacklist = new()
    {
        "/Game/FactoryGame/Equipment/BuildGun/BP_BuildGun.BP_BuildGun_C"
    };

    public static readonly List<string> Whitelist = new()
    {
        // Nuclear Waste is not produced by any buildings - it's a byproduct of Nuclear Power Plants
        "Desc_NuclearWaste_C",
        "Desc_PlutoniumWaste_C",
        // These are collectable items, not produced by buildings
        "Desc_Leaves_C",
        "Desc_Wood_C",
        "Desc_Mycelia_C",
        "Desc_HogParts_C",
        "Desc_SpitterParts_C",
        "Desc_StingerParts_C",
        "Desc_HatcherParts_C",
        "Desc_DissolvedSilica_C",
        "Desc_Crystal_C",
        "Desc_Crystal_mk2_C",
        "Desc_Crystal_mk3_C",
        // Liquid Oil can be produced by oil extractors and oil wells
        "Desc_LiquidOil_C",
        // FICSMAS items
        "Desc_Gift_C",
        "Desc_Snow_C",
        // SAM Ore is mined, but for some reason doesn't have a produced in property
        "Desc_SAM_C",
        // Special items
        "Desc_CrystalShard_C",
        "BP_ItemDescriptorPortableMiner_C"
    };

    // Helper function to check if a recipe is likely to be liquid based on building type and amount
    public static bool IsFluid(string productName)
    {
        List<string> liquidProducts = new() { "water", "liquidoil", "heavyoilresidue", "liquidfuel", "liquidturbofuel", "liquidbiofuel", "aluminasolution", "sulfuricacid", "nitricacid", "dissolvedsilica" };
        List<string> gasProducts = new() { "nitrogengas", "rocketfuel", "ionizedfuel", "quantumenergy", "darkenergy" };

        if (liquidProducts.Contains(productName.ToLower()))
        {
            return true;
        }

        return gasProducts.Contains(productName.ToLower());
    }

    public static bool IsFicsmas(string displayName)
    {
        return displayName.Contains("FICSMAS") ||
               displayName.Contains("Gift") ||
               displayName.Contains("Snow") ||
               displayName.Contains("Candy") ||
               displayName.Contains("Fireworks");
    }

    public static string GetRecipeName(string name)
    {
        return name.Replace("Build_", "").Replace("_C", "");
    }

    public static string GetPartName(string name)
    {
        name = name.Replace("Desc_", "").Replace("_C", "");
        return name;
    }

    public static string GetFriendlyName(string name)
    {
        // Remove any text within brackets, including the brackets themselves
        return Regex.Replace(name, @"\s*\(.*?\)", "");
    }

    // Example: Build_GeneratorBiomass_Automated_C
    // Change into "generatorbiomas"
    public static string? GetPowerProducerBuildingName(string className)
    {
        Match match = Regex.Match(className, @"Build_(\w+)_");
        if (match.Success)
        {
            string buildingName = match.Groups[1].Value.ToLower();
            // If contains _automated, remove it
            return buildingName.Replace("_automated", "");
        }
        return null;
    }
}
