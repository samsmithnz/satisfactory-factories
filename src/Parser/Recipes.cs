using System.Text.Json;
using System.Text.RegularExpressions;
using Parser.Models;

namespace Parser;

public static class Recipes
{
    // If you can read this, you are a wizard. ChatGPT made this, it works, so I won't question it!
    public static List<ParserRecipe> GetProductionRecipes(
        JsonElement[] data,
        Dictionary<string, double> producingBuildings)
    {
        List<ParserRecipe> recipes = new();

        IEnumerable<JsonElement> filteredRecipes = data
            .Where(entry => entry.TryGetProperty("Classes", out _))
            .SelectMany(entry => entry.GetProperty("Classes").EnumerateArray())
            .Where(recipe =>
            {
                // Filter out recipes that don't have a producing building
                if (!recipe.TryGetProperty("mProducedIn", out JsonElement producedIn))
                    return false;

                string producedInStr = producedIn.GetString() ?? "";

                // Filter out recipes that are in the blacklist (typically items produced by the Build Gun)
                if (Common.Blacklist.All(building => producedInStr.Contains(building)))
                    return false;

                // Extract all producing buildings
                MatchCollection rawBuildingKeys = Regex.Matches(producedInStr, @"/([^/]+)\.");
                if (rawBuildingKeys.Count == 0)
                {
                    return false;
                }

                // Process all buildings and check if any match the producingBuildings map
                bool validBuilding = rawBuildingKeys.Any(rawBuilding =>
                {
                    string buildingKey = rawBuilding.Value.Replace("/", "").Replace(".", "").ToLower().Replace("build_", "");
                    return producingBuildings.ContainsKey(buildingKey);
                });

                return validBuilding;
            });

        foreach (JsonElement recipe in filteredRecipes)
        {
            string ingredientsStr = recipe.TryGetProperty("mIngredients", out JsonElement ingredientsElement)
                ? ingredientsElement.GetString() ?? ""
                : "";

            List<ParserIngredient> ingredients = new();
            if (!string.IsNullOrEmpty(ingredientsStr))
            {
                MatchCollection ingredientMatches = Regex.Matches(ingredientsStr, @"ItemClass="".*?\/Desc_(.*?)\.Desc_.*?"",Amount=(\d+)");
                foreach (Match match in ingredientMatches)
                {
                    string partName = match.Groups[1].Value;
                    int amount = int.Parse(match.Groups[2].Value);
                    if (Common.IsFluid(partName))
                    {
                        amount = amount / 1000;
                    }

                    double manufacturingDuration = recipe.TryGetProperty("mManufactoringDuration", out JsonElement durationElement)
                        ? double.Parse(durationElement.GetString() ?? "0")
                        : 0;
                    double perMin = manufacturingDuration > 0 && amount > 0
                        ? (60 / manufacturingDuration) * amount
                        : 0;

                    ingredients.Add(new ParserIngredient
                    {
                        Part = partName,
                        Amount = amount,
                        PerMin = perMin
                    });
                }
            }

            // Parse mProduct to extract all products
            string productStr = recipe.TryGetProperty("mProduct", out JsonElement productElement)
                ? productElement.GetString() ?? ""
                : "";

            string className = recipe.TryGetProperty("ClassName", out JsonElement classNameElement)
                ? classNameElement.GetString() ?? ""
                : "";

            MatchCollection productMatches;
            // exception for automated miner recipes - as the product is a BP_ItemDescriptor
            if (className == "Recipe_Alternate_AutomatedMiner_C")
            {
                productMatches = Regex.Matches(productStr, @"ItemClass="".*?\/BP_ItemDescriptor(.*?)\.BP_ItemDescriptor.*?"",Amount=(\d+)");
            }
            else
            {
                productMatches = Regex.Matches(productStr, @"ItemClass="".*?\/Desc_(.*?)\.Desc_.*?"",Amount=(\d+)");
            }

            List<ParserProduct> products = new();
            foreach (Match match in productMatches)
            {
                string productName = match.Groups[1].Value;
                int amount = int.Parse(match.Groups[2].Value);
                if (Common.IsFluid(productName))
                {
                    amount = amount / 1000;  // Divide by 1000 for liquid/gas amounts
                }

                double manufacturingDuration = recipe.TryGetProperty("mManufactoringDuration", out JsonElement durationElement)
                    ? double.Parse(durationElement.GetString() ?? "0")
                    : 0;
                double perMin = manufacturingDuration > 0 && amount > 0
                    ? (60 / manufacturingDuration) * amount
                    : 0;

                products.Add(new ParserProduct
                {
                    Part = productName,
                    Amount = amount,
                    PerMin = perMin,
                    IsByProduct = products.Count > 0
                });
            }

            // Extract all producing buildings
            string producedInStr = recipe.TryGetProperty("mProducedIn", out JsonElement producedInElement)
                ? producedInElement.GetString() ?? ""
                : "";

            MatchCollection producedInMatches = Regex.Matches(producedInStr, @"/(\w+)/(\w+)\.(\w+)_C");

            // Filter and normalize building names, excluding invalid entries
            List<string> validBuildings = producedInMatches
                .Select(buildingMatch =>
                {
                    Match match = Regex.Match(buildingMatch.Value, @"/(\w+)\.(\w+)_C");
                    if (match.Success && match.Groups.Count > 2)
                    {
                        return match.Groups[2].Value.Replace("build_", "", StringComparison.OrdinalIgnoreCase).ToLower();
                    }
                    return null;
                })
                .Where(buildingName => buildingName != null && !new[] { "bp_workbenchcomponent", "bp_workshopcomponent", "factorygame" }.Contains(buildingName))
                .Cast<string>()
                .ToList();

            // Calculate power per building and choose the most relevant one
            double powerPerBuilding = 0;
            string selectedBuilding = "";

            if (validBuildings.Count > 0)
            {
                // Sum up power for all valid buildings
                foreach (string buildingName in validBuildings)
                {
                    if (producingBuildings.ContainsKey(buildingName))
                    {
                        double buildingPower = producingBuildings[buildingName];
                        if (string.IsNullOrEmpty(selectedBuilding))
                        {
                            selectedBuilding = buildingName; // Set the first valid building as selected
                        }
                        powerPerBuilding += buildingPower; // Add power for this building
                    }
                }
            }

            // Calculate variable power for recipes that need it
            double? lowPower = null;
            double? highPower = null;
            if (selectedBuilding == "hadroncollider" ||
                selectedBuilding == "converter" ||
                selectedBuilding == "quantumencoder")
            {
                // get the power from the recipe instead of the building
                if (recipe.TryGetProperty("mVariablePowerConsumptionConstant", out JsonElement lowPowerElement))
                {
                    string lowPowerStr = lowPowerElement.GetString() ?? "0";
                    lowPower = double.Parse(lowPowerStr);
                }

                if (recipe.TryGetProperty("mVariablePowerConsumptionFactor", out JsonElement highPowerElement))
                {
                    string highPowerStr = highPowerElement.GetString() ?? "0";
                    highPower = double.Parse(highPowerStr);
                }

                // calculate the average power: Note that because low power can be 0, (and often is), we can't use truthy checks to validate these values
                if (lowPower.HasValue && highPower.HasValue)
                {
                    powerPerBuilding = (lowPower.Value + highPower.Value) / 2;
                }
            }

            // Create building object with the selected building and calculated power
            ParserBuilding building = new()
            {
                Name = selectedBuilding, // Use the first valid building, or empty string if none
                Power = powerPerBuilding, // Use calculated power or 0
            };

            // keeping this in a separate conditional prevents a ton of properties with null values from being added to the building object
            if (lowPower.HasValue && highPower.HasValue)
            {
                building = building with
                {
                    MinPower = lowPower.Value,
                    MaxPower = highPower.Value
                };
            }

            string displayName = recipe.TryGetProperty("mDisplayName", out JsonElement displayNameElement)
                ? displayNameElement.GetString() ?? ""
                : "";

            recipes.Add(new ParserRecipe
            {
                Id = className.Replace("Recipe_", "").Replace("_C", ""),
                DisplayName = displayName,
                Ingredients = ingredients,
                Products = products,
                Building = building,
                IsAlternate = displayName.Contains("Alternate"),
                IsFicsmas = Common.IsFicsmas(displayName),
            });
        }

        // // Manually add Nuclear waste recipes
        // recipes.Add(new ParserRecipe
        // {
        //     Id = "NuclearWaste",
        //     DisplayName = "Uranium Waste",
        //     Ingredients = new List<ParserIngredient>
        //     {
        //         new() { Part = "NuclearFuelRod", Amount = 1, PerMin = 0.2 },
        //         new() { Part = "Water", Amount = 1200, PerMin = 240 }
        //     },
        //     Products = new List<ParserProduct>
        //     {
        //         new() { Part = "NuclearWaste", Amount = 1, PerMin = 10 }
        //     },
        //     Building = new ParserBuilding { Name = "nuclearpowerplant", Power = 2500 },
        //     IsAlternate = false,
        //     IsFicsmas = false,
        // });
        // recipes.Add(new ParserRecipe
        // {
        //     Id = "PlutoniumWaste",
        //     DisplayName = "Plutonium Waste",
        //     Ingredients = new List<ParserIngredient>
        //     {
        //         new() { Part = "PlutoniumFuelRod", Amount = 1, PerMin = 0.1 },
        //         new() { Part = "Water", Amount = 2400, PerMin = 240 }
        //     },
        //     Products = new List<ParserProduct>
        //     {
        //         new() { Part = "PlutoniumWaste", Amount = 1, PerMin = 1 }
        //     },
        //     Building = new ParserBuilding { Name = "nuclearpowerplant", Power = 2500 },
        //     IsAlternate = false,
        //     IsFicsmas = false,
        // });

        return recipes.OrderBy(r => r.DisplayName).ToList();
    }

    public static List<ParserPowerRecipe> GetPowerGeneratingRecipes(
        JsonElement[] data,
        ParserItemDataInterface parts)
    {
        List<ParserPowerRecipe> recipes = new();

        IEnumerable<JsonElement> filteredRecipes = data
            .Where(entry => entry.TryGetProperty("Classes", out _))
            .SelectMany(entry => entry.GetProperty("Classes").EnumerateArray())
            .Where(recipe => recipe.TryGetProperty("mFuel", out _));

        foreach (JsonElement recipe in filteredRecipes)
        {
            string className = recipe.TryGetProperty("ClassName", out JsonElement classNameElement)
                ? classNameElement.GetString() ?? ""
                : "";

            string displayName = recipe.TryGetProperty("mDisplayName", out JsonElement displayNameElement)
                ? displayNameElement.GetString() ?? ""
                : "";

            double powerProduction = recipe.TryGetProperty("mPowerProduction", out JsonElement powerProductionElement)
                ? (powerProductionElement.ValueKind == JsonValueKind.String
                    ? double.Parse(powerProductionElement.GetString() ?? "0")
                    : powerProductionElement.GetDouble())
                : 0;

            ParserBuilding building = new()
            {
                Name = Common.GetPowerProducerBuildingName(className) ?? "UNKNOWN",
                Power = Math.Round(powerProduction), // generated power - can be rounded to the nearest whole number (all energy numbers are whole numbers)
            };

            double supplementalRatio = recipe.TryGetProperty("mSupplementalToPowerRatio", out JsonElement supplementalRatioElement)
                ? (supplementalRatioElement.ValueKind == JsonValueKind.String
                    ? double.Parse(supplementalRatioElement.GetString() ?? "0")
                    : supplementalRatioElement.GetDouble())
                : 0;

            // 1. Generator MW generated. This is an hourly value.
            // 2. Divide by 60, to get the minute value
            // 3. Now calculate the MJ, using the MJ->MW constant (1/3600), (https://en.wikipedia.org/wiki/Joule#Conversions) 
            // 4. Now divide this number by the part energy to calculate how many MJ we burn in 1 minute. e.g. For nuclear reactors this is 150,000MJ / minute.
            double burnRateMJ = (powerProduction / 60) / (1.0 / 3600);

            List<JsonElement> fuels = new();
            if (recipe.TryGetProperty("mFuel", out JsonElement fuelElement))
            {
                if (fuelElement.ValueKind == JsonValueKind.Array)
                {
                    fuels = fuelElement.EnumerateArray().ToList();
                }
            }

            // The game data does not seem to contain the duration of the burning of the fuel. So we have to calculate it from the megajuoles of the fuel.
            // We know that the burn rate is 150,000MJ / minute, so we can figure out the durations from that.

            foreach (JsonElement fuel in fuels)
            {
                string mFuelClass = fuel.TryGetProperty("mFuelClass", out JsonElement fuelClassElement)
                    ? fuelClassElement.GetString() ?? ""
                    : "";

                string primaryFuel = Common.GetPartName(mFuelClass);
                ParserPart primaryFuelPart = parts.Parts[primaryFuel];

                double burnDurationInMins = primaryFuelPart.EnergyGeneratedInMJ / burnRateMJ;
                double burnDurationInS = burnDurationInMins * 60; // Convert to seconds

                string supplementalResourceClass = fuel.TryGetProperty("mSupplementalResourceClass", out JsonElement supplementalResourceElement)
                    ? supplementalResourceElement.GetString() ?? ""
                    : "";

                string byproductClass = fuel.TryGetProperty("mByproduct", out JsonElement byproductElement)
                    ? byproductElement.GetString() ?? ""
                    : "";

                double byproductAmount = fuel.TryGetProperty("mByproductAmount", out JsonElement byproductAmountElement)
                    ? (byproductAmountElement.ValueKind == JsonValueKind.String
                        ? double.Parse(byproductAmountElement.GetString() ?? "0")
                        : byproductAmountElement.GetDouble())
                    : 0;

                ParserFuel fuelItem = new()
                {
                    PrimaryFuel = primaryFuel,
                    SupplementalResource = !string.IsNullOrEmpty(supplementalResourceClass) ? Common.GetPartName(supplementalResourceClass) : "",
                    ByProduct = !string.IsNullOrEmpty(byproductClass) ? Common.GetPartName(byproductClass) : "",
                    ByProductAmount = byproductAmount,
                    ByProductAmountPerMin = byproductAmount / burnDurationInMins,
                    BurnDurationInS = burnDurationInS
                };

                //Find the part for the primary fuel
                double primaryPerMin = 0;
                if (primaryFuelPart.EnergyGeneratedInMJ > 0)
                {
                    // The rounding here is important to remove floating point errors that appear with some types 
                    // (this is step 4 from above)
                    primaryPerMin = Math.Round(burnRateMJ / primaryFuelPart.EnergyGeneratedInMJ, 5);
                }

                List<ParserPowerItem> ingredients = new();
                ingredients.Add(new ParserPowerItem
                {
                    Part = fuelItem.PrimaryFuel,
                    PerMin = primaryPerMin,
                    MwPerItem = building.Power / primaryPerMin,
                });

                if (!string.IsNullOrEmpty(fuelItem.SupplementalResource) && supplementalRatio > 0)
                {
                    double perMin = (3.0 / 50) * supplementalRatio * building.Power;
                    double supplementalFuelRatio = (3.0 / 50) * supplementalRatio;
                    ingredients.Add(new ParserPowerItem
                    {
                        Part = fuelItem.SupplementalResource,
                        PerMin = perMin, // Calculate the ratio of the supplemental resource to the primary fuel
                        SupplementalRatio = supplementalFuelRatio,
                    });
                }

                ParserPowerItem? byproduct = null;
                if (!string.IsNullOrEmpty(fuelItem.ByProduct))
                {
                    byproduct = new ParserPowerItem
                    {
                        Part = fuelItem.ByProduct,
                        PerMin = fuelItem.ByProductAmountPerMin,
                    };
                }

                recipes.Add(new ParserPowerRecipe
                {
                    Id = Common.GetRecipeName(className) + '_' + fuelItem.PrimaryFuel,
                    DisplayName = displayName + " (" + primaryFuelPart.Name + ")",
                    Ingredients = ingredients,
                    Byproduct = byproduct,
                    Building = new ParserPowerRecipeBuilding
                    {
                        Name = building.Name,
                        Power = building.Power
                    }
                });
            }
        }

        return recipes.OrderBy(r => r.DisplayName).ToList();
    }
}
