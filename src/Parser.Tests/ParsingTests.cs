using Parser.Models;

namespace Parser.Tests;

[TestClass]
public sealed class ParsingTests
{
    private static ProcessFileResult? _results;

    [ClassInitialize]
    public static async Task Initialize(TestContext context)
    {
        // Arrange
        string inputFile = "game-docs.json";
        string outputFile = "gameData.json";

        // Act
        _results = await Processor.ProcessFile(inputFile, outputFile);
    }

    [TestMethod]
    public void PartsShouldBeOfExpectedLength()
    {
        // Assert
        Assert.IsNotNull(_results);
        Assert.AreEqual(168, _results.Items.Parts.Count);
    }

    [TestMethod]
    public void RawResourcesShouldBeOfExpectedLength()
    {
        // Assert
        Assert.IsNotNull(_results);
        Assert.AreEqual(24, _results.Items.RawResources.Count);
        Assert.AreEqual("Coal", _results.Items.RawResources["Coal"].Name);
        Assert.AreEqual(42300L, _results.Items.RawResources["Coal"].Limit);
        Assert.AreEqual("Blue Power Slug", _results.Items.RawResources["Crystal"].Name);
        Assert.AreEqual(596L, _results.Items.RawResources["Crystal"].Limit);
        Assert.AreEqual("Wood", _results.Items.RawResources["Wood"].Name);
        Assert.AreEqual(100000000L, _results.Items.RawResources["Wood"].Limit);
    }

    [TestMethod]
    public void IronPlatePartShouldBeCorrect()
    {
        // Assert
        Assert.IsNotNull(_results);
        ParserPart part = _results.Items.Parts["IronPlate"];

        Assert.IsNotNull(part);
        Assert.AreEqual("Iron Plate", part.Name);
        Assert.AreEqual(200, part.StackSize);
        Assert.AreEqual(false, part.IsFluid);
        Assert.AreEqual(false, part.IsFicsmas);
        Assert.AreEqual(0, part.EnergyGeneratedInMJ);
    }

    [TestMethod]
    public void LiquidFuelPartShouldBeCorrect()
    {
        // Assert
        Assert.IsNotNull(_results);
        ParserPart part = _results.Items.Parts["LiquidFuel"];

        Assert.IsNotNull(part);
        Assert.AreEqual("Fuel", part.Name);
        Assert.AreEqual(0, part.StackSize);
        Assert.AreEqual(true, part.IsFluid);
        Assert.AreEqual(false, part.IsFicsmas);
        Assert.AreEqual(750, part.EnergyGeneratedInMJ);
    }

    [TestMethod]
    public void RecipeLengthShouldBeCorrect()
    {
        // Assert
        Assert.IsNotNull(_results);
        Assert.AreEqual(291, _results.Recipes.Count);
    }

    [TestMethod]
    public void BuildingsShouldGenerateCorrectData()
    {
        // Assert
        Assert.IsNotNull(_results);
        Assert.AreEqual(15, _results.Buildings.Count);
        
        Dictionary<string, double> expectedBuildings = new()
        {
            { "assemblermk1", 15 },
            { "blender", 75 },
            { "constructormk1", 4 },
            { "converter", 0.1 },
            { "foundrymk1", 16 },
            { "generatorbiomass", 0 },
            { "generatorcoal", 0 },
            { "generatorfuel", 0 },
            { "generatornuclear", 0 },
            { "hadroncollider", 0.1 },
            { "manufacturermk1", 55 },
            { "oilrefinery", 30 },
            { "packager", 10 },
            { "quantumencoder", 0.1 },
            { "smeltermk1", 4 }
        };

        CollectionAssert.AreEqual(expectedBuildings, _results.Buildings);
    }

    [TestMethod]
    public void ShouldProperlyCalculateTheCorrectNumberOfPartsUsedAndProducedInRecipes()
    {
        // Assert
        Assert.IsNotNull(_results);
        
        HashSet<string> parts = new();

        // Scan all ingredients and products in all recipes to produce a list of parts that are used
        foreach (ParserRecipe recipe in _results.Recipes)
        {
            foreach (ParserIngredient ingredient in recipe.Ingredients)
            {
                parts.Add(ingredient.Part);
            }
            foreach (ParserProduct product in recipe.Products)
            {
                parts.Add(product.Part);
            }
            Assert.IsTrue(recipe.Products.Count > 0, $"Recipe {recipe.Id} has no products");
        }

        // Now we have our list of parts, assert that the number of parts we've generated actually match
        List<string> partsList = _results.Items.Parts.Keys.ToList();
        List<string> missingParts = partsList.Where(part => !parts.Contains(part)).ToList();
        List<string> extraParts = parts.Where(part => !partsList.Contains(part)).ToList();

        Assert.AreEqual(0, missingParts.Count, $"Missing parts: {string.Join(", ", missingParts)}");
        Assert.AreEqual(0, extraParts.Count, $"Extra parts: {string.Join(", ", extraParts)}");
        Assert.AreEqual(parts.Count, _results.Items.Parts.Count);
    }

    [TestMethod]
    public void ValidateARecipeWithASingleIngredientAndProduct_IronPlates()
    {
        // Assert
        Assert.IsNotNull(_results);
        ParserRecipe? recipe = _results.Recipes.Find(item => item.Id == "IronPlate");

        Assert.IsNotNull(recipe);
        Assert.AreEqual("Iron Plate", recipe.DisplayName);
        Assert.AreEqual(1, recipe.Ingredients.Count);
        Assert.AreEqual("IronIngot", recipe.Ingredients[0].Part);
        Assert.AreEqual(3.0, recipe.Ingredients[0].Amount);
        Assert.AreEqual(30.0, recipe.Ingredients[0].PerMin);
        Assert.AreEqual(1, recipe.Products.Count);
        Assert.AreEqual("IronPlate", recipe.Products[0].Part);
        Assert.AreEqual(2.0, recipe.Products[0].Amount);
        Assert.AreEqual(20.0, recipe.Products[0].PerMin);
        Assert.AreEqual(false, recipe.Products[0].IsByProduct);
        Assert.AreEqual("constructormk1", recipe.Building.Name);
        Assert.AreEqual(4.0, recipe.Building.Power);
    }

    [TestMethod]
    public void ValidateARecipeWithMultipleIngredients_ModularFrames()
    {
        // Assert
        Assert.IsNotNull(_results);
        ParserRecipe? recipe = _results.Recipes.Find(item => item.Id == "ModularFrame");

        Assert.IsNotNull(recipe);
        Assert.AreEqual("Modular Frame", recipe.DisplayName);
        Assert.AreEqual(2, recipe.Ingredients.Count);
        Assert.AreEqual("IronPlateReinforced", recipe.Ingredients[0].Part);
        Assert.AreEqual(3.0, recipe.Ingredients[0].Amount);
        Assert.AreEqual(3.0, recipe.Ingredients[0].PerMin);
        Assert.AreEqual("IronRod", recipe.Ingredients[1].Part);
        Assert.AreEqual(12.0, recipe.Ingredients[1].Amount);
        Assert.AreEqual(12.0, recipe.Ingredients[1].PerMin);
        Assert.AreEqual(1, recipe.Products.Count);
        Assert.AreEqual("ModularFrame", recipe.Products[0].Part);
        Assert.AreEqual(2.0, recipe.Products[0].Amount);
        Assert.AreEqual(2.0, recipe.Products[0].PerMin);
        Assert.AreEqual(false, recipe.Products[0].IsByProduct);
        Assert.AreEqual("assemblermk1", recipe.Building.Name);
        Assert.AreEqual(15.0, recipe.Building.Power);
        Assert.AreEqual(false, recipe.IsAlternate);
    }

    [TestMethod]
    public void ValidateARecipeWithMultipleProducts_Plastic()
    {
        // Assert
        Assert.IsNotNull(_results);
        ParserRecipe? recipe = _results.Recipes.Find(item => item.Id == "Plastic");

        Assert.IsNotNull(recipe);
        Assert.AreEqual("Plastic", recipe.DisplayName);
        Assert.AreEqual(1, recipe.Ingredients.Count);
        Assert.AreEqual("LiquidOil", recipe.Ingredients[0].Part);
        Assert.AreEqual(3.0, recipe.Ingredients[0].Amount);
        Assert.AreEqual(30.0, recipe.Ingredients[0].PerMin);
        Assert.AreEqual(2, recipe.Products.Count);
        Assert.AreEqual("Plastic", recipe.Products[0].Part);
        Assert.AreEqual(2.0, recipe.Products[0].Amount);
        Assert.AreEqual(20.0, recipe.Products[0].PerMin);
        Assert.AreEqual(false, recipe.Products[0].IsByProduct);
        Assert.AreEqual("HeavyOilResidue", recipe.Products[1].Part);
        Assert.AreEqual(1.0, recipe.Products[1].Amount);
        Assert.AreEqual(10.0, recipe.Products[1].PerMin);
        Assert.AreEqual(true, recipe.Products[1].IsByProduct);
        Assert.AreEqual("oilrefinery", recipe.Building.Name);
        Assert.AreEqual(30.0, recipe.Building.Power);
        Assert.AreEqual(false, recipe.IsAlternate);
    }

    [TestMethod]
    public void ValidateARecipeWithVariablePower_NuclearPasta()
    {
        // Assert
        Assert.IsNotNull(_results);
        ParserRecipe? recipe = _results.Recipes.Find(item => item.Id == "SpaceElevatorPart_9");

        Assert.IsNotNull(recipe);
        Assert.AreEqual("Nuclear Pasta", recipe.DisplayName);
        Assert.AreEqual(2, recipe.Ingredients.Count);
        Assert.AreEqual("CopperDust", recipe.Ingredients[0].Part);
        Assert.AreEqual(200.0, recipe.Ingredients[0].Amount);
        Assert.AreEqual(100.0, recipe.Ingredients[0].PerMin);
        Assert.AreEqual("PressureConversionCube", recipe.Ingredients[1].Part);
        Assert.AreEqual(1.0, recipe.Ingredients[1].Amount);
        Assert.AreEqual(0.5, recipe.Ingredients[1].PerMin);
        Assert.AreEqual(1, recipe.Products.Count);
        Assert.AreEqual("SpaceElevatorPart_9", recipe.Products[0].Part);
        Assert.AreEqual(1.0, recipe.Products[0].Amount);
        Assert.AreEqual(0.5, recipe.Products[0].PerMin);
        Assert.AreEqual(false, recipe.Products[0].IsByProduct);
        Assert.AreEqual("hadroncollider", recipe.Building.Name);
        Assert.AreEqual(750.0, recipe.Building.Power);
        Assert.AreEqual(500.0, recipe.Building.MinPower);
        Assert.AreEqual(1000.0, recipe.Building.MaxPower);
        Assert.AreEqual(false, recipe.IsAlternate);
    }

    [TestMethod]
    public void ValidateARecipeWithVariablePowerWithAQuantumEncoder_NeuralQuantumProcessor()
    {
        // Assert
        Assert.IsNotNull(_results);
        ParserRecipe? recipe = _results.Recipes.Find(item => item.Id == "TemporalProcessor");

        Assert.IsNotNull(recipe);
        Assert.AreEqual("Neural-Quantum Processor", recipe.DisplayName);
        Assert.AreEqual(4, recipe.Ingredients.Count);
        Assert.AreEqual("TimeCrystal", recipe.Ingredients[0].Part);
        Assert.AreEqual(5.0, recipe.Ingredients[0].Amount);
        Assert.AreEqual(15.0, recipe.Ingredients[0].PerMin);
        Assert.AreEqual("ComputerSuper", recipe.Ingredients[1].Part);
        Assert.AreEqual(1.0, recipe.Ingredients[1].Amount);
        Assert.AreEqual(3.0, recipe.Ingredients[1].PerMin);
        Assert.AreEqual("FicsiteMesh", recipe.Ingredients[2].Part);
        Assert.AreEqual(15.0, recipe.Ingredients[2].Amount);
        Assert.AreEqual(45.0, recipe.Ingredients[2].PerMin);
        Assert.AreEqual("QuantumEnergy", recipe.Ingredients[3].Part);
        Assert.AreEqual(25.0, recipe.Ingredients[3].Amount);
        Assert.AreEqual(75.0, recipe.Ingredients[3].PerMin);
        Assert.AreEqual(2, recipe.Products.Count);
        Assert.AreEqual("TemporalProcessor", recipe.Products[0].Part);
        Assert.AreEqual(1.0, recipe.Products[0].Amount);
        Assert.AreEqual(3.0, recipe.Products[0].PerMin);
        Assert.AreEqual(false, recipe.Products[0].IsByProduct);
        Assert.AreEqual("DarkEnergy", recipe.Products[1].Part);
        Assert.AreEqual(25.0, recipe.Products[1].Amount);
        Assert.AreEqual(75.0, recipe.Products[1].PerMin);
        Assert.AreEqual(true, recipe.Products[1].IsByProduct);
        Assert.AreEqual("quantumencoder", recipe.Building.Name);
        Assert.AreEqual(1000.0, recipe.Building.Power);
        Assert.AreEqual(0.0, recipe.Building.MinPower);
        Assert.AreEqual(2000.0, recipe.Building.MaxPower);
        Assert.AreEqual(false, recipe.IsAlternate);
    }
}
