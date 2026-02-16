using Web.Models;
using Web.Models.Factory;

namespace Web.Tests.Services.FactoryManagement;

/// <summary>
/// Helper class for creating test data for factory management tests.
/// </summary>
public static class TestDataHelper
{
    /// <summary>
    /// Creates a test factory with default values.
    /// </summary>
    public static Factory CreateTestFactory(string name, int? id = null, int? displayOrder = null)
    {
        return new Factory
        {
            Id = id ?? Random.Shared.Next(1, 10000),
            Name = name,
            Products = new List<FactoryItem>(),
            ByProducts = new List<ByProductItem>(),
            PowerProducers = new List<FactoryPowerProducer>(),
            Inputs = new List<FactoryInput>(),
            PreviousInputs = new List<FactoryInput>(),
            Parts = new Dictionary<string, PartMetrics>(),
            BuildingRequirements = new Dictionary<string, BuildingRequirement>(),
            Dependencies = new FactoryDependency(),
            ExportCalculator = new Dictionary<string, ExportCalculatorSettings>(),
            RawResources = new Dictionary<string, WorldRawResource>(),
            Power = new FactoryPower(),
            RequirementsSatisfied = true,
            UsingRawResourcesOnly = false,
            Hidden = false,
            HasProblem = false,
            InSync = null,
            SyncState = new Dictionary<string, FactorySyncState>(),
            SyncStatePower = new Dictionary<string, FactoryPowerSyncState>(),
            DisplayOrder = displayOrder ?? -1,
            Tasks = new List<FactoryTask>(),
            Notes = "",
            DataVersion = "2025-01-03"
        };
    }

    /// <summary>
    /// Creates test game data with basic recipes and parts.
    /// </summary>
    public static GameData CreateTestGameData()
    {
        return new GameData
        {
            Buildings = new Dictionary<string, double>
            {
                { "smeltermk1", 4 },
                { "constructormk1", 4 },
                { "assemblermk1", 15 }
            },
            Items = new GameDataItems
            {
                Parts = new Dictionary<string, Part>
                {
                    { "IronIngot", new Part { Name = "Iron Ingot", StackSize = 100, IsFluid = false, IsFicsmas = false, EnergyGeneratedInMJ = 0 } },
                    { "IronPlate", new Part { Name = "Iron Plate", StackSize = 100, IsFluid = false, IsFicsmas = false, EnergyGeneratedInMJ = 0 } },
                    { "IronRod", new Part { Name = "Iron Rod", StackSize = 100, IsFluid = false, IsFicsmas = false, EnergyGeneratedInMJ = 0 } },
                    { "Screw", new Part { Name = "Screw", StackSize = 500, IsFluid = false, IsFicsmas = false, EnergyGeneratedInMJ = 0 } },
                    { "ReinforcedIronPlate", new Part { Name = "Reinforced Iron Plate", StackSize = 100, IsFluid = false, IsFicsmas = false, EnergyGeneratedInMJ = 0 } },
                    { "CompactedCoal", new Part { Name = "Compacted Coal", StackSize = 100, IsFluid = false, IsFicsmas = false, EnergyGeneratedInMJ = 0 } },
                    { "Coal", new Part { Name = "Coal", StackSize = 100, IsFluid = false, IsFicsmas = false, EnergyGeneratedInMJ = 0 } }
                },
                RawResources = new Dictionary<string, RawResource>
                {
                    { "OreIron", new RawResource { Name = "Iron Ore", Limit = 70380 } },
                    { "Coal", new RawResource { Name = "Coal", Limit = 30120 } }
                }
            },
            Recipes = new List<Recipe>
            {
                // Iron Ingot recipe
                new Recipe
                {
                    Id = "IronIngot",
                    DisplayName = "Iron Ingot",
                    Ingredients = new List<RecipeItem>
                    {
                        new RecipeItem { Part = "OreIron", Amount = 1, PerMin = 30 }
                    },
                    Products = new List<RecipeItem>
                    {
                        new RecipeItem { Part = "IronIngot", Amount = 1, PerMin = 30, IsByProduct = false }
                    },
                    Building = new RecipeBuilding { Name = "smeltermk1", Power = 4 },
                    IsAlternate = false,
                    IsFicsmas = false
                },
                // Iron Plate recipe
                new Recipe
                {
                    Id = "IronPlate",
                    DisplayName = "Iron Plate",
                    Ingredients = new List<RecipeItem>
                    {
                        new RecipeItem { Part = "IronIngot", Amount = 3, PerMin = 30 }
                    },
                    Products = new List<RecipeItem>
                    {
                        new RecipeItem { Part = "IronPlate", Amount = 2, PerMin = 20, IsByProduct = false }
                    },
                    Building = new RecipeBuilding { Name = "constructormk1", Power = 4 },
                    IsAlternate = false,
                    IsFicsmas = false
                },
                // Iron Rod recipe
                new Recipe
                {
                    Id = "IronRod",
                    DisplayName = "Iron Rod",
                    Ingredients = new List<RecipeItem>
                    {
                        new RecipeItem { Part = "IronIngot", Amount = 1, PerMin = 15 }
                    },
                    Products = new List<RecipeItem>
                    {
                        new RecipeItem { Part = "IronRod", Amount = 1, PerMin = 15, IsByProduct = false }
                    },
                    Building = new RecipeBuilding { Name = "constructormk1", Power = 4 },
                    IsAlternate = false,
                    IsFicsmas = false
                },
                // Screw recipe
                new Recipe
                {
                    Id = "Screw",
                    DisplayName = "Screw",
                    Ingredients = new List<RecipeItem>
                    {
                        new RecipeItem { Part = "IronRod", Amount = 1, PerMin = 10 }
                    },
                    Products = new List<RecipeItem>
                    {
                        new RecipeItem { Part = "Screw", Amount = 4, PerMin = 40, IsByProduct = false }
                    },
                    Building = new RecipeBuilding { Name = "constructormk1", Power = 4 },
                    IsAlternate = false,
                    IsFicsmas = false
                },
                // Reinforced Iron Plate recipe
                new Recipe
                {
                    Id = "ReinforcedIronPlate",
                    DisplayName = "Reinforced Iron Plate",
                    Ingredients = new List<RecipeItem>
                    {
                        new RecipeItem { Part = "IronPlate", Amount = 6, PerMin = 30 },
                        new RecipeItem { Part = "Screw", Amount = 12, PerMin = 60 }
                    },
                    Products = new List<RecipeItem>
                    {
                        new RecipeItem { Part = "ReinforcedIronPlate", Amount = 1, PerMin = 5, IsByProduct = false }
                    },
                    Building = new RecipeBuilding { Name = "assemblermk1", Power = 15 },
                    IsAlternate = false,
                    IsFicsmas = false
                },
                // Compacted Coal recipe (has a byproduct - water)
                new Recipe
                {
                    Id = "CompactedCoal",
                    DisplayName = "Compacted Coal",
                    Ingredients = new List<RecipeItem>
                    {
                        new RecipeItem { Part = "Coal", Amount = 5, PerMin = 25 }
                    },
                    Products = new List<RecipeItem>
                    {
                        new RecipeItem { Part = "CompactedCoal", Amount = 5, PerMin = 25, IsByProduct = false },
                        new RecipeItem { Part = "Water", Amount = 5, PerMin = 25, IsByProduct = true }
                    },
                    Building = new RecipeBuilding { Name = "assemblermk1", Power = 15 },
                    IsAlternate = false,
                    IsFicsmas = false
                }
            },
            PowerGenerationRecipes = new List<PowerRecipe>
            {
                new PowerRecipe
                {
                    Id = "GeneratorCoal",
                    DisplayName = "Coal Generator",
                    Ingredients = new List<Web.Models.PowerItem>
                    {
                        new Web.Models.PowerItem { Part = "Coal", PerMin = 15 }
                    },
                    Building = new RecipeBuilding { Name = "generatorcoal", Power = 75 }
                }
            }
        };
    }

    /// <summary>
    /// Adds a product to a factory.
    /// </summary>
    public static void AddProductToFactory(Factory factory, string id, double amount, string recipe, int? displayOrder = null)
    {
        factory.Products.Add(new FactoryItem
        {
            Id = id,
            Amount = amount,
            Recipe = recipe,
            DisplayOrder = displayOrder ?? factory.Products.Count,
            Requirements = new Dictionary<string, RequirementAmount>(),
            BuildingRequirements = new BuildingRequirement(),
            ByProducts = new List<ByProductItem>()
        });
    }
}
