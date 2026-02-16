using System.Text.Json;
using System.Text.RegularExpressions;
using Parser.Models;

namespace Parser;

public static class Parts
{
    public static ParserItemDataInterface GetItems(JsonElement[] data)
    {
        Dictionary<string, ParserPart> parts = new();
        Dictionary<string, ParserRawResource> rawResources = GetRawResources(data);

        // Scan all recipes (not parts), looking for parts that are used in recipes.
        foreach (JsonElement entry in data.Where(e => e.TryGetProperty("Classes", out _)))
        {
            JsonElement classes = entry.GetProperty("Classes");
            foreach (JsonElement classEntry in classes.EnumerateArray())
            {
                // There are two exception products we need to check for and add to the parts list
                if (classEntry.TryGetProperty("ClassName", out JsonElement className))
                {
                    string classNameStr = className.GetString() ?? "";
                    
                    if (classNameStr == "Desc_NuclearWaste_C")
                    {
                        // Note that this part id is NuclearWaste, not Uranium Waste
                        parts["NuclearWaste"] = new ParserPart
                        {
                            Name = "Uranium Waste",
                            StackSize = 500, //SS_HUGE
                            IsFluid = Common.IsFluid("NuclearWaste"),
                            IsFicsmas = classEntry.TryGetProperty("mDisplayName", out JsonElement dn) ? Common.IsFicsmas(dn.GetString() ?? "") : false,
                            EnergyGeneratedInMJ = 0
                        };
                    }
                    else if (classNameStr == "Desc_PlutoniumWaste_C")
                    {
                        parts["PlutoniumWaste"] = new ParserPart
                        {
                            Name = "Plutonium Waste",
                            StackSize = 500, //SS_HUGE
                            IsFluid = Common.IsFluid("PlutoniumWaste"),
                            IsFicsmas = classEntry.TryGetProperty("mDisplayName", out JsonElement dn) ? Common.IsFicsmas(dn.GetString() ?? "") : false,
                            EnergyGeneratedInMJ = 0
                        };
                    }
                    // These are exception products that aren't produced by mines or extractors, they are raw materials
                    else if (classNameStr == "Desc_Leaves_C")
                    {
                        parts["Leaves"] = new ParserPart
                        {
                            Name = "Leaves",
                            StackSize = 500, //SS_HUGE
                            IsFluid = false,
                            IsFicsmas = false,
                            EnergyGeneratedInMJ = 15
                        };
                    }
                    else if (classNameStr == "Desc_Wood_C")
                    {
                        parts["Wood"] = new ParserPart
                        {
                            Name = "Wood",
                            StackSize = 200, //SS_BIG
                            IsFluid = false,
                            IsFicsmas = false,
                            EnergyGeneratedInMJ = 100
                        };
                    }
                    else if (classNameStr == "Desc_Mycelia_C")
                    {
                        parts["Mycelia"] = new ParserPart
                        {
                            Name = "Mycelia",
                            StackSize = 200, //SS_BIG
                            IsFluid = false,
                            IsFicsmas = false,
                            EnergyGeneratedInMJ = 20
                        };
                    }
                    else if (classNameStr == "Desc_HogParts_C")
                    {
                        parts["HogParts"] = new ParserPart
                        {
                            Name = "Hog Remains",
                            StackSize = 50, //SS_SMALL
                            IsFluid = false,
                            IsFicsmas = false,
                            EnergyGeneratedInMJ = 0
                        };
                    }
                    else if (classNameStr == "Desc_SpitterParts_C")
                    {
                        parts["SpitterParts"] = new ParserPart
                        {
                            Name = "Spitter Remains",
                            StackSize = 50, //SS_SMALL
                            IsFluid = false,
                            IsFicsmas = false,
                            EnergyGeneratedInMJ = 0
                        };
                    }
                    else if (classNameStr == "Desc_StingerParts_C")
                    {
                        parts["StingerParts"] = new ParserPart
                        {
                            Name = "Stinger Remains",
                            StackSize = 50, //SS_SMALL
                            IsFluid = false,
                            IsFicsmas = false,
                            EnergyGeneratedInMJ = 0
                        };
                    }
                    else if (classNameStr == "Desc_HatcherParts_C")
                    {
                        parts["HatcherParts"] = new ParserPart
                        {
                            Name = "Hatcher Remains",
                            StackSize = 50, //SS_SMALL
                            IsFluid = false,
                            IsFicsmas = false,
                            EnergyGeneratedInMJ = 0
                        };
                    }
                    else if (classNameStr == "Desc_DissolvedSilica_C")
                    {
                        // This is a special intermediate alt product
                        parts["DissolvedSilica"] = new ParserPart
                        {
                            Name = "Dissolved Silica",
                            StackSize = 0, //SS_FLUID
                            IsFluid = true,
                            IsFicsmas = false,
                            EnergyGeneratedInMJ = 0
                        };
                    }
                    else if (classNameStr == "Desc_LiquidOil_C")
                    {
                        // This is a special liquid raw material
                        parts["LiquidOil"] = new ParserPart
                        {
                            Name = "Crude Oil",
                            StackSize = 0, //SS_FLUID
                            IsFluid = true,
                            IsFicsmas = false,
                            EnergyGeneratedInMJ = 320
                        };
                    }
                    else if (classNameStr == "Desc_Gift_C")
                    {
                        // this is a ficsmas collectable
                        parts["Gift"] = new ParserPart
                        {
                            Name = "FICSMAS Gift",
                            StackSize = 200, //SS_BIG
                            IsFluid = false,
                            IsFicsmas = true,
                            EnergyGeneratedInMJ = 0
                        };
                    }
                    else if (classNameStr == "Desc_Snow_C")
                    {
                        // this is a ficsmas collectable
                        parts["Snow"] = new ParserPart
                        {
                            Name = "Snow",
                            StackSize = 500, //SS_HUGE
                            IsFluid = false,
                            IsFicsmas = true,
                            EnergyGeneratedInMJ = 0
                        };
                    }
                    else if (classNameStr == "Desc_Crystal_C")
                    {
                        parts["Crystal"] = new ParserPart
                        {
                            Name = "Blue Power Slug",
                            StackSize = 50, //SS_SMALL
                            IsFluid = false,
                            IsFicsmas = false,
                            EnergyGeneratedInMJ = 0
                        };
                    }
                    else if (classNameStr == "Desc_Crystal_mk2_C")
                    {
                        parts["Crystal_mk2"] = new ParserPart
                        {
                            Name = "Yellow Power Slug",
                            StackSize = 50, //SS_SMALL
                            IsFluid = false,
                            IsFicsmas = false,
                            EnergyGeneratedInMJ = 0
                        };
                    }
                    else if (classNameStr == "Desc_Crystal_mk3_C")
                    {
                        parts["Crystal_mk3"] = new ParserPart
                        {
                            Name = "Purple Power Slug",
                            StackSize = 50, //SS_SMALL
                            IsFluid = false,
                            IsFicsmas = false,
                            EnergyGeneratedInMJ = 0
                        };
                    }
                    else if (classNameStr == "Desc_SAM_C")
                    {
                        parts["SAM"] = new ParserPart
                        {
                            Name = "SAM",
                            StackSize = 100, //SS_MEDIUM
                            IsFluid = false,
                            IsFicsmas = false,
                            EnergyGeneratedInMJ = 0
                        };
                    }
                    else if (classNameStr == "Desc_CrystalShard_C")
                    {
                        parts["CrystalShard"] = new ParserPart
                        {
                            Name = "Power Shard",
                            StackSize = 100, //SS_MEDIUM
                            IsFluid = false,
                            IsFicsmas = false,
                            EnergyGeneratedInMJ = 0
                        };
                    }
                    else if (classNameStr == "BP_ItemDescriptorPortableMiner_C")
                    {
                        parts["PortableMiner"] = new ParserPart
                        {
                            Name = "Portable Miner",
                            StackSize = 50, //SS_SMALL
                            IsFluid = false,
                            IsFicsmas = false,
                            EnergyGeneratedInMJ = 0
                        };
                    }
                }

                if (!classEntry.TryGetProperty("ClassName", out _))
                    continue;

                // Ensures it's a recipe, we only care about items that are produced within a recipe.
                if (!classEntry.TryGetProperty("mProducedIn", out JsonElement producedIn))
                    continue;

                string producedInStr = producedIn.GetString() ?? "";
                if (Common.Blacklist.Any(building => producedInStr.Contains(building)))
                    continue;

                // Check if it's an alternate recipe and skip it for parts
                if (classEntry.TryGetProperty("ClassName", out JsonElement cn) && cn.GetString()?.StartsWith("Recipe_Alternate") == true)
                    continue;

                // Check if it's an unpackage recipe and skip it for parts
                if (classEntry.TryGetProperty("mDisplayName", out JsonElement displayName) && displayName.GetString()?.Contains("Unpackage") == true)
                    continue;

                // Extract the part name
                if (!classEntry.TryGetProperty("mProduct", out JsonElement product))
                    continue;

                string productStr = product.GetString() ?? "";
                MatchCollection productMatches = Regex.Matches(productStr, @"ItemClass="".*?\/Desc_(.*?)\.Desc_.*?"",Amount=(\d+)");

                foreach (Match match in productMatches)
                {
                    string partName = Common.GetPartName(match.Groups[1].Value);  // Use the mProduct part name
                    string friendlyName = Common.GetFriendlyName(displayName.GetString() ?? "");  // Use the friendly name

                    // Extract the product's Desc_ class name so we can find it in the class descriptors to get the stack size
                    Match productClassMatch = Regex.Match(match.Value, @"Desc_(.*?)\.Desc_");
                    if (!productClassMatch.Success)
                        continue;

                    string productClass = productClassMatch.Groups[1].Value;

                    JsonElement? classDescriptor = data
                        .Where(e => e.TryGetProperty("Classes", out _))
                        .SelectMany(e => e.GetProperty("Classes").EnumerateArray())
                        .FirstOrDefault(e => e.TryGetProperty("ClassName", out JsonElement cn) && cn.GetString() == $"Desc_{productClass}_C");

                    // Extract stack size
                    int stackSize = StackSizeConvert(classDescriptor?.TryGetProperty("mStackSize", out JsonElement ss) == true ? ss.GetString() ?? "SS_UNKNOWN" : "SS_UNKNOWN");
                    
                    // Extract the energy value
                    int energyValue = 0;
                    if (classDescriptor?.TryGetProperty("mEnergyValue", out JsonElement energyVal) == true)
                    {
                        if (energyVal.ValueKind == JsonValueKind.String)
                        {
                            double.TryParse(energyVal.GetString(), out double ev);
                            energyValue = (int)ev;
                        }
                        else if (energyVal.ValueKind == JsonValueKind.Number)
                        {
                            energyValue = energyVal.GetInt32();
                        }
                    }

                    // If the part is a fluid, the energy value is multiplied by 1000, cos the game loves to store everything as 0.0001 values...
                    if (Common.IsFluid(partName))
                    {
                        energyValue *= 1000;
                    }

                    parts[partName] = new ParserPart
                    {
                        Name = friendlyName,
                        StackSize = stackSize,
                        IsFluid = Common.IsFluid(partName),
                        IsFicsmas = Common.IsFicsmas(displayName.GetString() ?? ""),
                        EnergyGeneratedInMJ = (int)Math.Round((double)energyValue) // Round to the nearest whole number (all energy numbers are whole numbers)
                    };
                }
            }
        }

        // Sort the parts by key
        return new ParserItemDataInterface
        {
            Parts = parts,
            RawResources = rawResources
        };
    }

    private static int StackSizeConvert(string stackSize)
    {
        // Convert e.g. SS_HUGE to 500
        return stackSize switch
        {
            "SS_HUGE" => 500,
            "SS_BIG" => 200,
            "SS_MEDIUM" => 100,
            "SS_SMALL" => 50,
            _ => 0
        };
    }

    // Function to extract raw resources from the game data
    private static Dictionary<string, ParserRawResource> GetRawResources(JsonElement[] data)
    {
        Dictionary<string, ParserRawResource> rawResources = new();
        Dictionary<string, long> limits = new()
        {
            { "Coal", 42300 },
            { "LiquidOil", 12600 },
            { "NitrogenGas", 12000 },
            { "OreBauxite", 12300 },
            { "OreCopper", 36900 },
            { "OreGold", 15000 },
            { "OreIron", 92100 },
            { "OreUranium", 2100 },
            { "RawQuartz", 13500 },
            { "SAM", 10200 },
            { "Stone", 69900 },
            { "Sulfur", 10800 },
            { "Water", 9007199254740991 }
        };

        foreach (JsonElement entry in data.Where(e => e.TryGetProperty("NativeClass", out JsonElement nc) && nc.GetString() == "/Script/CoreUObject.Class'/Script/FactoryGame.FGResourceDescriptor'"))
        {
            JsonElement classes = entry.GetProperty("Classes");
            foreach (JsonElement resource in classes.EnumerateArray())
            {
                if (resource.TryGetProperty("ClassName", out JsonElement className) &&
                    resource.TryGetProperty("mDisplayName", out JsonElement displayName))
                {
                    string classNameStr = Common.GetPartName(className.GetString() ?? "");
                    string displayNameStr = displayName.GetString() ?? "";

                    if (!string.IsNullOrEmpty(classNameStr) && !string.IsNullOrEmpty(displayNameStr))
                    {
                        rawResources[classNameStr] = new ParserRawResource
                        {
                            Name = displayNameStr,
                            Limit = limits.ContainsKey(classNameStr) ? limits[classNameStr] : 0
                        };
                    }
                }
            }
        }

        // Manually add Leaves, Wood, Mycelia to the rawResources list
        rawResources["Leaves"] = new ParserRawResource
        {
            Name = "Leaves",
            Limit = limits.ContainsKey("Leaves") ? limits["Leaves"] : 100000000
        };
        rawResources["Wood"] = new ParserRawResource
        {
            Name = "Wood",
            Limit = limits.ContainsKey("Wood") ? limits["Wood"] : 100000000
        };
        rawResources["Mycelia"] = new ParserRawResource
        {
            Name = "Mycelia",
            Limit = limits.ContainsKey("Mycelia") ? limits["Mycelia"] : 100000000
        };

        // Manually add alien parts to the rawResources list
        rawResources["HatcherParts"] = new ParserRawResource
        {
            Name = "Hatcher Remains",
            Limit = 100000000
        };
        rawResources["HogParts"] = new ParserRawResource
        {
            Name = "Hog Remains",
            Limit = 100000000
        };
        rawResources["SpitterParts"] = new ParserRawResource
        {
            Name = "Spitter Remains",
            Limit = 100000000
        };
        rawResources["StingerParts"] = new ParserRawResource
        {
            Name = "Stinger Remains",
            Limit = 100000000
        };

        // Manually add slugs. Numbers from Satisfactory Calculator map
        rawResources["Crystal"] = new ParserRawResource
        {
            Name = "Blue Power Slug",
            Limit = 596
        };
        rawResources["Crystal_mk2"] = new ParserRawResource
        {
            Name = "Yellow Power Slug",
            Limit = 389
        };
        rawResources["Crystal_mk3"] = new ParserRawResource
        {
            Name = "Purple Power Slug",
            Limit = 257
        };

        // Ficmas items
        rawResources["Gift"] = new ParserRawResource
        {
            Name = "FICSMAS Gift",
            Limit = 100000000
        };

        // Order the rawResources by key
        Dictionary<string, ParserRawResource> orderedRawResources = new();
        foreach (string key in rawResources.Keys.OrderBy(k => k))
        {
            orderedRawResources[key] = rawResources[key];
        }
        return orderedRawResources;
    }

    public static void FixItemNames(ParserItemDataInterface items)
    {
        // Go through the item names and do some manual fixes, e.g. renaming "Residual Plastic" to "Plastic"
        Dictionary<string, string> fixItems = new()
        {
            { "AlienProtein", "Alien Protein" },
            { "CompactedCoal", "Compacted Coal" },
            { "DarkEnergy", "Dark Matter Residue" },
            { "HeavyOilResidue", "Heavy Oil Residue" },
            { "LiquidFuel", "Fuel" },
            { "Plastic", "Plastic" },
            { "PolymerResin", "Polymer Resin" },
            { "Rubber", "Rubber" },
            { "Snow", "Snow" },
            { "Water", "Water" }
        };

        foreach (string search in fixItems.Keys)
        {
            if (items.Parts.ContainsKey(search))
            {
                ParserPart existingPart = items.Parts[search];
                items.Parts[search] = existingPart with { Name = fixItems[search] };
            }
        }
    }

    public static void FixTurbofuel(ParserItemDataInterface items, List<ParserRecipe> recipes)
    {
        // Rename the current "Turbofuel" which is actually "Packaged Turbofuel"
        if (items.Parts.ContainsKey("TurboFuel"))
        {
            items.Parts["PackagedTurboFuel"] = items.Parts["TurboFuel"];
        }

        // Add the actual "Turbofuel" as a new item
        items.Parts["LiquidTurboFuel"] = new ParserPart
        {
            Name = "Turbofuel",
            StackSize = 0,
            IsFluid = true,
            IsFicsmas = false,
            EnergyGeneratedInMJ = 2000
        };
        
        // rename the packaged item to PackagedTurboFuel
        items.Parts["PackagedTurboFuel"] = new ParserPart
        {
            Name = "Packaged Turbofuel",
            StackSize = 100, //SS_MEDIUM
            IsFluid = false,
            IsFicsmas = false,
            EnergyGeneratedInMJ = 2000
        };
        
        // remove the incorrect packaged turbofuel
        items.Parts.Remove("TurboFuel");

        // Now we need to go through the recipes and wherever "TurboFuel" is mentioned, it needs to be changed to "PackagedTurbofuel"
        foreach (ParserRecipe recipe in recipes)
        {
            for (int i = 0; i < recipe.Products.Count; i++)
            {
                if (recipe.Products[i].Part == "TurboFuel")
                {
                    recipe.Products[i] = recipe.Products[i] with { Part = "PackagedTurboFuel" };
                }
            }

            for (int i = 0; i < recipe.Ingredients.Count; i++)
            {
                if (recipe.Ingredients[i].Part == "TurboFuel")
                {
                    recipe.Ingredients[i] = recipe.Ingredients[i] with { Part = "PackagedTurboFuel" };
                }
            }
        }
    }
}
