using Parser.Models;

namespace Parser.Tests;

[TestClass]
public sealed class PowerTests
{
    private static ProcessFileResult? _results;

    [ClassInitialize]
    public static async Task Initialize(TestContext context)
    {
        // Arrange
        string inputFile = "game-docs.json";
        string outputFile = "powerGameData.json";

        // Act
        _results = await Processor.ProcessFile(inputFile, outputFile);
    }

    [TestMethod]
    public void ShouldGenerateABiomassRecipeCorrectlyWithExpectedValues()
    {
        // Assert
        Assert.IsNotNull(_results);
        ParserPowerRecipe? recipe = _results.PowerGenerationRecipes.Find(item => item.Id == "GeneratorBiomass_Automated_Wood");

        Assert.IsNotNull(recipe);
        Assert.AreEqual("Biomass Burner (Wood)", recipe.DisplayName);
        Assert.AreEqual("Wood", recipe.Ingredients[0].Part);
        Assert.AreEqual(18.0, recipe.Ingredients[0].PerMin);
        Assert.IsNull(recipe.Byproduct);
        Assert.AreEqual("generatorbiomass", recipe.Building.Name);
        Assert.AreEqual(30.0, recipe.Building.Power);
    }

    [TestMethod]
    public void ShouldGenerateABiomassLiquidRecipeCorrectlyWithExpectedValues()
    {
        // Assert
        Assert.IsNotNull(_results);
        ParserPowerRecipe? recipe = _results.PowerGenerationRecipes.Find(item => item.Id == "GeneratorBiomass_Automated_PackagedBiofuel");

        Assert.IsNotNull(recipe);
        Assert.AreEqual("Biomass Burner (Packaged Liquid Biofuel)", recipe.DisplayName);
        Assert.AreEqual("PackagedBiofuel", recipe.Ingredients[0].Part);
        Assert.AreEqual(2.4, recipe.Ingredients[0].PerMin);
        Assert.IsNull(recipe.Byproduct);
        Assert.AreEqual("generatorbiomass", recipe.Building.Name);
        Assert.AreEqual(30.0, recipe.Building.Power);
    }

    [TestMethod]
    public void ShouldGenerateACoalPoweredGenerationRecipeWithExpectedValues()
    {
        // Assert
        Assert.IsNotNull(_results);
        ParserPowerRecipe? recipe = _results.PowerGenerationRecipes.Find(item => item.Id == "GeneratorCoal_Coal");

        Assert.IsNotNull(recipe);
        Assert.AreEqual("Coal-Powered Generator (Coal)", recipe.DisplayName);
        Assert.AreEqual("Coal", recipe.Ingredients[0].Part);
        Assert.AreEqual(15.0, recipe.Ingredients[0].PerMin);
        Assert.AreEqual("Water", recipe.Ingredients[1].Part);
        Assert.AreEqual(45.0, recipe.Ingredients[1].PerMin);
        Assert.IsNull(recipe.Byproduct);
        Assert.AreEqual("generatorcoal", recipe.Building.Name);
        Assert.AreEqual(75.0, recipe.Building.Power);
    }

    [TestMethod]
    public void ShouldGenerateTheLiquidBiofuelFuelGeneratorRecipeCorrectly()
    {
        // Assert
        Assert.IsNotNull(_results);
        ParserPowerRecipe? recipe = _results.PowerGenerationRecipes.Find(item => item.Id == "GeneratorFuel_LiquidBiofuel");

        Assert.IsNotNull(recipe);
        Assert.AreEqual("Fuel-Powered Generator (Liquid Biofuel)", recipe.DisplayName);
        Assert.AreEqual("LiquidBiofuel", recipe.Ingredients[0].Part);
        Assert.AreEqual(20.0, recipe.Ingredients[0].PerMin);
        Assert.IsNull(recipe.Byproduct);
        Assert.AreEqual("generatorfuel", recipe.Building.Name);
        Assert.AreEqual(250.0, recipe.Building.Power);
    }

    [TestMethod]
    public void ShouldGenerateTheLiquidFuelFuelGeneratorRecipeCorrectly()
    {
        // Assert
        Assert.IsNotNull(_results);
        ParserPowerRecipe? recipe = _results.PowerGenerationRecipes.Find(item => item.Id == "GeneratorFuel_LiquidFuel");

        Assert.IsNotNull(recipe);
        Assert.AreEqual("Fuel-Powered Generator (Fuel)", recipe.DisplayName);
        Assert.AreEqual("LiquidFuel", recipe.Ingredients[0].Part);
        Assert.AreEqual(20.0, recipe.Ingredients[0].PerMin);
        Assert.IsNull(recipe.Byproduct);
        Assert.AreEqual("generatorfuel", recipe.Building.Name);
        Assert.AreEqual(250.0, recipe.Building.Power);
    }

    [TestMethod]
    public void ShouldGenerateTheLiquidTurbofuelFuelGeneratorRecipeCorrectly()
    {
        // Assert
        Assert.IsNotNull(_results);
        ParserPowerRecipe? recipe = _results.PowerGenerationRecipes.Find(item => item.Id == "GeneratorFuel_LiquidTurboFuel");

        Assert.IsNotNull(recipe);
        Assert.AreEqual("Fuel-Powered Generator (Turbofuel)", recipe.DisplayName);
        Assert.AreEqual("LiquidTurboFuel", recipe.Ingredients[0].Part);
        Assert.AreEqual(7.5, recipe.Ingredients[0].PerMin);
        Assert.IsNull(recipe.Byproduct);
        Assert.AreEqual("generatorfuel", recipe.Building.Name);
        Assert.AreEqual(250.0, recipe.Building.Power);
    }

    [TestMethod]
    public void ShouldGenerateTheRocketfuelFuelGeneratorRecipeCorrectly()
    {
        // Assert
        Assert.IsNotNull(_results);
        ParserPowerRecipe? recipe = _results.PowerGenerationRecipes.Find(item => item.Id == "GeneratorFuel_RocketFuel");

        Assert.IsNotNull(recipe);
        Assert.AreEqual("Fuel-Powered Generator (Rocket Fuel)", recipe.DisplayName);
        Assert.AreEqual("RocketFuel", recipe.Ingredients[0].Part);
        Assert.AreEqual(4.16667, recipe.Ingredients[0].PerMin);
        Assert.IsNull(recipe.Byproduct);
        Assert.AreEqual("generatorfuel", recipe.Building.Name);
        Assert.AreEqual(250.0, recipe.Building.Power);
    }

    [TestMethod]
    public void ShouldGenerateTheIonizedFuelGeneratorRecipeCorrectly()
    {
        // Assert
        Assert.IsNotNull(_results);
        ParserPowerRecipe? recipe = _results.PowerGenerationRecipes.Find(item => item.Id == "GeneratorFuel_IonizedFuel");

        Assert.IsNotNull(recipe);
        Assert.AreEqual("Fuel-Powered Generator (Ionized Fuel)", recipe.DisplayName);
        Assert.AreEqual("IonizedFuel", recipe.Ingredients[0].Part);
        Assert.AreEqual(3.0, recipe.Ingredients[0].PerMin);
        Assert.IsNull(recipe.Byproduct);
        Assert.AreEqual("generatorfuel", recipe.Building.Name);
        Assert.AreEqual(250.0, recipe.Building.Power);
    }

    [TestMethod]
    public void ShouldGenerateTheNuclearFuelRodRecipeCorrectly()
    {
        // Assert
        Assert.IsNotNull(_results);
        ParserPowerRecipe? recipe = _results.PowerGenerationRecipes.Find(item => item.Id == "GeneratorNuclear_NuclearFuelRod");

        Assert.IsNotNull(recipe);
        Assert.AreEqual("Nuclear Power Plant (Uranium Fuel Rod)", recipe.DisplayName);
        Assert.AreEqual("NuclearFuelRod", recipe.Ingredients[0].Part);
        Assert.AreEqual(0.2, recipe.Ingredients[0].PerMin);
        Assert.AreEqual("Water", recipe.Ingredients[1].Part);
        Assert.AreEqual(240.0, recipe.Ingredients[1].PerMin);
        Assert.IsNotNull(recipe.Byproduct);
        Assert.AreEqual("NuclearWaste", recipe.Byproduct.Part);
        Assert.AreEqual(10.0, recipe.Byproduct.PerMin);
        Assert.AreEqual("generatornuclear", recipe.Building.Name);
        Assert.AreEqual(2500.0, recipe.Building.Power);
    }

    [TestMethod]
    public void ShouldGenerateThePlutoniumFuelRodRecipeCorrectly()
    {
        // Assert
        Assert.IsNotNull(_results);
        ParserPowerRecipe? recipe = _results.PowerGenerationRecipes.Find(item => item.Id == "GeneratorNuclear_PlutoniumFuelRod");

        Assert.IsNotNull(recipe);
        Assert.AreEqual("Nuclear Power Plant (Plutonium Fuel Rod)", recipe.DisplayName);
        Assert.AreEqual("PlutoniumFuelRod", recipe.Ingredients[0].Part);
        Assert.AreEqual(0.1, recipe.Ingredients[0].PerMin);
        Assert.AreEqual("Water", recipe.Ingredients[1].Part);
        Assert.AreEqual(240.0, recipe.Ingredients[1].PerMin);
        Assert.IsNotNull(recipe.Byproduct);
        Assert.AreEqual("PlutoniumWaste", recipe.Byproduct.Part);
        Assert.AreEqual(1.0, recipe.Byproduct.PerMin);
        Assert.AreEqual("generatornuclear", recipe.Building.Name);
        Assert.AreEqual(2500.0, recipe.Building.Power);
    }

    [TestMethod]
    public void ShouldGenerateTheFicsoniumFuelRodRecipeCorrectly()
    {
        // Assert
        Assert.IsNotNull(_results);
        ParserPowerRecipe? recipe = _results.PowerGenerationRecipes.Find(item => item.Id == "GeneratorNuclear_FicsoniumFuelRod");

        Assert.IsNotNull(recipe);
        Assert.AreEqual("Nuclear Power Plant (Ficsonium Fuel Rod)", recipe.DisplayName);
        Assert.AreEqual("FicsoniumFuelRod", recipe.Ingredients[0].Part);
        Assert.AreEqual(1.0, recipe.Ingredients[0].PerMin);
        Assert.IsNull(recipe.Byproduct);
        Assert.AreEqual("generatornuclear", recipe.Building.Name);
        Assert.AreEqual(2500.0, recipe.Building.Power);
    }
}
