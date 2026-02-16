using System.Text.Json;
using Web.Models;

namespace Web.Tests;

[TestClass]
public sealed class GameDataModelTests
{
    private const string SamplePartJson = @"{
        ""name"": ""Iron Ingot"",
        ""stackSize"": 100,
        ""isFluid"": false,
        ""isFicsmas"": false,
        ""energyGeneratedInMJ"": 0
    }";

    private const string SampleRecipeJson = @"{
        ""id"": ""IronIngot"",
        ""displayName"": ""Iron Ingot"",
        ""ingredients"": [
            {
                ""part"": ""OreIron"",
                ""amount"": 1,
                ""perMin"": 30
            }
        ],
        ""products"": [
            {
                ""part"": ""IronIngot"",
                ""amount"": 1,
                ""perMin"": 30
            }
        ],
        ""building"": {
            ""name"": ""smeltermk1"",
            ""power"": 4
        },
        ""isAlternate"": false,
        ""isFicsmas"": false
    }";

    [TestMethod]
    public void Part_ShouldDeserializeFromJson()
    {
        // Arrange & Act
        Part? part = JsonSerializer.Deserialize<Part>(SamplePartJson);

        // Assert
        Assert.IsNotNull(part);
        Assert.AreEqual("Iron Ingot", part.Name);
        Assert.AreEqual(100, part.StackSize);
        Assert.IsFalse(part.IsFluid);
        Assert.IsFalse(part.IsFicsmas);
        Assert.AreEqual(0, part.EnergyGeneratedInMJ);
    }

    [TestMethod]
    public void Recipe_ShouldDeserializeFromJson()
    {
        // Arrange & Act
        Recipe? recipe = JsonSerializer.Deserialize<Recipe>(SampleRecipeJson);

        // Assert
        Assert.IsNotNull(recipe);
        Assert.AreEqual("IronIngot", recipe.Id);
        Assert.AreEqual("Iron Ingot", recipe.DisplayName);
        Assert.AreEqual(1, recipe.Ingredients.Count);
        Assert.AreEqual(1, recipe.Products.Count);
        Assert.AreEqual("OreIron", recipe.Ingredients[0].Part);
        Assert.AreEqual("IronIngot", recipe.Products[0].Part);
        Assert.IsFalse(recipe.IsAlternate);
    }

    [TestMethod]
    public void RecipeItem_ShouldDeserializeWithOptionalIsByProduct()
    {
        // Arrange
        string jsonWithByProduct = @"{""part"":""test"",""amount"":1,""perMin"":30,""isByProduct"":true}";
        string jsonWithoutByProduct = @"{""part"":""test"",""amount"":1,""perMin"":30}";

        // Act
        RecipeItem? itemWithByProduct = JsonSerializer.Deserialize<RecipeItem>(jsonWithByProduct);
        RecipeItem? itemWithoutByProduct = JsonSerializer.Deserialize<RecipeItem>(jsonWithoutByProduct);

        // Assert
        Assert.IsNotNull(itemWithByProduct);
        Assert.IsTrue(itemWithByProduct.IsByProduct.HasValue);
        Assert.IsTrue(itemWithByProduct.IsByProduct.Value);

        Assert.IsNotNull(itemWithoutByProduct);
        Assert.IsFalse(itemWithoutByProduct.IsByProduct.HasValue);
    }

    [TestMethod]
    public void GameData_ShouldInitializeWithEmptyCollections()
    {
        // Arrange & Act
        GameData gameData = new GameData();

        // Assert
        Assert.IsNotNull(gameData.Buildings);
        Assert.IsNotNull(gameData.Items);
        Assert.IsNotNull(gameData.Recipes);
        Assert.IsNotNull(gameData.PowerGenerationRecipes);
        Assert.AreEqual(0, gameData.Buildings.Count);
        Assert.AreEqual(0, gameData.Recipes.Count);
    }

    [TestMethod]
    public void PowerRecipe_ShouldAllowNullByproduct()
    {
        // Arrange
        string jsonWithoutByProduct = @"{
            ""id"": ""test"",
            ""displayName"": ""Test"",
            ""ingredients"": [],
            ""byproduct"": null,
            ""building"": {""name"":""test"",""power"":0}
        }";

        // Act
        PowerRecipe? powerRecipe = JsonSerializer.Deserialize<PowerRecipe>(jsonWithoutByProduct);

        // Assert
        Assert.IsNotNull(powerRecipe);
        Assert.IsNull(powerRecipe.Byproduct);
    }
}
