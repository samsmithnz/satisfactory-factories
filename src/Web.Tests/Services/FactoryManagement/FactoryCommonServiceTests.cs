using Web.Models;
using Web.Models.Factory;
using Web.Services.FactoryManagement;

namespace Web.Tests.Services.FactoryManagement;

[TestClass]
public sealed class FactoryCommonServiceTests
{
    private FactoryCommonService? _commonService;

    [TestInitialize]
    public void Initialize()
    {
        _commonService = new FactoryCommonService();
    }

    [TestMethod]
    public void CreateNewPart_ShouldCreateNewPart()
    {
        // Arrange
        Factory factory = CreateTestFactory("Test Factory");
        string partId = "NewPart";

        // Act
        _commonService!.CreateNewPart(factory, partId);

        // Assert
        Assert.IsTrue(factory.Parts.ContainsKey(partId));
        PartMetrics partMetrics = factory.Parts[partId];
        Assert.AreEqual(0, partMetrics.AmountRequired);
        Assert.AreEqual(0, partMetrics.AmountSupplied);
        Assert.IsTrue(partMetrics.Satisfied);
        Assert.IsFalse(partMetrics.IsRaw);
        Assert.IsFalse(partMetrics.Exportable);
    }

    [TestMethod]
    public void CreateNewPart_ShouldNotOverwriteExistingPart()
    {
        // Arrange
        Factory factory = CreateTestFactory("Test Factory");
        string partId = "CompactedCoal";

        // Create the part first time
        _commonService!.CreateNewPart(factory, partId);
        factory.Parts[partId].AmountRequired = 1234;

        // Act - try to create it again
        _commonService.CreateNewPart(factory, partId);

        // Assert - should still have the original value
        Assert.AreEqual(1234, factory.Parts[partId].AmountRequired);
    }

    [TestMethod]
    public void GetRecipe_ShouldReturnRecipeWhenFound()
    {
        // Arrange
        GameData gameData = CreateTestGameData();
        string recipeId = "IronIngot";

        // Act
        Recipe? recipe = _commonService!.GetRecipe(recipeId, gameData);

        // Assert
        Assert.IsNotNull(recipe);
        Assert.AreEqual(recipeId, recipe.Id);
    }

    [TestMethod]
    public void GetRecipe_ShouldReturnNullWhenNotFound()
    {
        // Arrange
        GameData gameData = CreateTestGameData();
        string recipeId = "NonExistentRecipe";

        // Act
        Recipe? recipe = _commonService!.GetRecipe(recipeId, gameData);

        // Assert
        Assert.IsNull(recipe);
    }

    [TestMethod]
    public void GetPartDisplayName_ShouldReturnNameForRawResource()
    {
        // Arrange
        GameData gameData = CreateTestGameData();
        string partId = "OreIron";

        // Act
        string displayName = _commonService!.GetPartDisplayName(partId, gameData);

        // Assert
        Assert.AreEqual("Iron Ore", displayName);
    }

    [TestMethod]
    public void GetPartDisplayName_ShouldReturnNameForPart()
    {
        // Arrange
        GameData gameData = CreateTestGameData();
        string partId = "IronIngot";

        // Act
        string displayName = _commonService!.GetPartDisplayName(partId, gameData);

        // Assert
        Assert.AreEqual("Iron Ingot", displayName);
    }

    [TestMethod]
    public void GetPartDisplayName_ShouldReturnErrorMessageWhenPartIsEmpty()
    {
        // Arrange
        GameData gameData = CreateTestGameData();
        string partId = "";

        // Act
        string displayName = _commonService!.GetPartDisplayName(partId, gameData);

        // Assert
        Assert.AreEqual("NO PART!!!", displayName);
    }

    [TestMethod]
    public void GetPartDisplayName_ShouldReturnUnknownWhenPartNotFound()
    {
        // Arrange
        GameData gameData = CreateTestGameData();
        string partId = "UnknownPart";

        // Act
        string displayName = _commonService!.GetPartDisplayName(partId, gameData);

        // Assert
        Assert.AreEqual("UNKNOWN PART UnknownPart!", displayName);
    }

    [TestMethod]
    public void GetBuildingDisplayName_ShouldReturnFriendlyName()
    {
        // Arrange
        string buildingId = "smeltermk1";

        // Act
        string displayName = _commonService!.GetBuildingDisplayName(buildingId);

        // Assert
        Assert.AreEqual("Smelter", displayName);
    }

    [TestMethod]
    public void GetBuildingDisplayName_ShouldReturnUnknownForInvalidBuilding()
    {
        // Arrange
        string buildingId = "unknownbuilding";

        // Act
        string displayName = _commonService!.GetBuildingDisplayName(buildingId);

        // Assert
        Assert.AreEqual("UNKNOWN BUILDING: unknownbuilding", displayName);
    }

    [TestMethod]
    public void GetPowerRecipeById_ShouldReturnRecipeWhenFound()
    {
        // Arrange
        GameData gameData = CreateTestGameData();
        string recipeId = "GeneratorCoal";

        // Act
        PowerRecipe? recipe = _commonService!.GetPowerRecipeById(recipeId, gameData);

        // Assert
        Assert.IsNotNull(recipe);
        Assert.AreEqual(recipeId, recipe.Id);
    }

    [TestMethod]
    public void GetPowerRecipeById_ShouldReturnNullWhenNotFound()
    {
        // Arrange
        GameData gameData = CreateTestGameData();
        string recipeId = "NonExistent";

        // Act
        PowerRecipe? recipe = _commonService!.GetPowerRecipeById(recipeId, gameData);

        // Assert
        Assert.IsNull(recipe);
    }

    [TestMethod]
    public void GetPowerRecipeById_ShouldReturnNullWhenRecipeIdIsEmpty()
    {
        // Arrange
        GameData gameData = CreateTestGameData();
        string recipeId = "";

        // Act
        PowerRecipe? recipe = _commonService!.GetPowerRecipeById(recipeId, gameData);

        // Assert
        Assert.IsNull(recipe);
    }

    // Helper methods
    private static Factory CreateTestFactory(string name)
    {
        return new Factory
        {
            Id = Random.Shared.Next(1, 10000),
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
            DisplayOrder = -1,
            Tasks = new List<FactoryTask>(),
            Notes = "",
            DataVersion = "2025-01-03"
        };
    }

    private static GameData CreateTestGameData()
    {
        return new GameData
        {
            Buildings = new Dictionary<string, double>(),
            Items = new GameDataItems
            {
                Parts = new Dictionary<string, Part>
                {
                    { "IronIngot", new Part { Name = "Iron Ingot", StackSize = 100, IsFluid = false, IsFicsmas = false, EnergyGeneratedInMJ = 0 } }
                },
                RawResources = new Dictionary<string, RawResource>
                {
                    { "OreIron", new RawResource { Name = "Iron Ore", Limit = 70380 } }
                }
            },
            Recipes = new List<Recipe>
            {
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
}
