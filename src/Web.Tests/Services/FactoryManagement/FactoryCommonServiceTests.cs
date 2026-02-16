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
        Factory factory = TestDataHelper.CreateTestFactory("Test Factory");
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
        Factory factory = TestDataHelper.CreateTestFactory("Test Factory");
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
        GameData gameData = TestDataHelper.CreateTestGameData();
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
        GameData gameData = TestDataHelper.CreateTestGameData();
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
        GameData gameData = TestDataHelper.CreateTestGameData();
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
        GameData gameData = TestDataHelper.CreateTestGameData();
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
        GameData gameData = TestDataHelper.CreateTestGameData();
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
        GameData gameData = TestDataHelper.CreateTestGameData();
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
        GameData gameData = TestDataHelper.CreateTestGameData();
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
        GameData gameData = TestDataHelper.CreateTestGameData();
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
        GameData gameData = TestDataHelper.CreateTestGameData();
        string recipeId = "";

        // Act
        PowerRecipe? recipe = _commonService!.GetPowerRecipeById(recipeId, gameData);

        // Assert
        Assert.IsNull(recipe);
    }

}
