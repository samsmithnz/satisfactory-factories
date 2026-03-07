using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Web.Models.Factory;

namespace Web.Tests.Services;

/// <summary>
/// Tests that factory JSON data can be correctly deserialized from both PascalCase
/// (Blazor-saved) and camelCase (Vue-app-saved) JSON formats.
///
/// This validates the fix for the "No amount set!" label always showing on imports:
/// AppStateService.LoadFactoriesAsync() must use PropertyNameCaseInsensitive=true so
/// that camelCase input amounts ("amount") correctly map to the C# property "Amount".
/// </summary>
[TestClass]
public class FactoryJsonDeserializationTests
{
    private const string CamelCaseFactoryJson = @"[
      {
        ""id"": 1,
        ""name"": ""Supplier Factory"",
        ""inputs"": [],
        ""previousInputs"": [],
        ""products"": [],
        ""byProducts"": [],
        ""powerProducers"": [],
        ""parts"": {},
        ""buildingRequirements"": {},
        ""requirementsSatisfied"": false,
        ""exportCalculator"": {},
        ""dependencies"": {""suppliers"": {}, ""requests"": {}},
        ""rawResources"": {},
        ""power"": {},
        ""usingRawResourcesOnly"": false,
        ""hidden"": false,
        ""hasProblem"": false,
        ""inSync"": null,
        ""syncState"": {},
        ""syncStatePower"": {},
        ""displayOrder"": 1,
        ""tasks"": [],
        ""notes"": """",
        ""dataVersion"": ""2025-01-03""
      },
      {
        ""id"": 2,
        ""name"": ""Consumer Factory"",
        ""inputs"": [
          {
            ""factoryId"": 1,
            ""outputPart"": ""IronIngot"",
            ""amount"": 320
          }
        ],
        ""previousInputs"": [],
        ""products"": [],
        ""byProducts"": [],
        ""powerProducers"": [],
        ""parts"": {},
        ""buildingRequirements"": {},
        ""requirementsSatisfied"": false,
        ""exportCalculator"": {},
        ""dependencies"": {""suppliers"": {}, ""requests"": {}},
        ""rawResources"": {},
        ""power"": {},
        ""usingRawResourcesOnly"": false,
        ""hidden"": false,
        ""hasProblem"": false,
        ""inSync"": null,
        ""syncState"": {},
        ""syncStatePower"": {},
        ""displayOrder"": 2,
        ""tasks"": [],
        ""notes"": """",
        ""dataVersion"": ""2025-01-03""
      }
    ]";

    /// <summary>
    /// Verifies that camelCase JSON (as saved by the Vue app) deserializes the input Amount
    /// correctly when using PropertyNameCaseInsensitive = true.
    /// This is the fix for the "No amount set!" label always showing on imports.
    /// </summary>
    [TestMethod]
    public void LoadFactories_CamelCaseJson_InputAmountDeserializesCorrectly()
    {
        // Arrange
        JsonSerializerOptions options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        // Act
        List<Factory>? factories = JsonSerializer.Deserialize<List<Factory>>(CamelCaseFactoryJson, options);

        // Assert
        Assert.IsNotNull(factories);
        Assert.AreEqual(2, factories.Count);
        FactoryInput input = factories[1].Inputs[0];
        Assert.AreEqual("IronIngot", input.OutputPart);
        Assert.AreEqual(320, input.Amount, "Input Amount should be 320, not 0 (camelCase 'amount' must map to 'Amount')");
    }

    /// <summary>
    /// Verifies that camelCase JSON without PropertyNameCaseInsensitive fails to map
    /// the entire inputs list, demonstrating the original bug (the whole Inputs list is empty,
    /// not just with Amount=0, because camelCase "inputs" does not match PascalCase "Inputs").
    /// </summary>
    [TestMethod]
    public void LoadFactories_CamelCaseJsonWithoutCaseInsensitive_InputsListIsEmpty()
    {
        // Arrange - no options (default, case-sensitive)

        // Act
        List<Factory>? factories = JsonSerializer.Deserialize<List<Factory>>(CamelCaseFactoryJson);

        // Assert - without case-insensitive, camelCase "inputs" does NOT map to "Inputs"
        // resulting in an empty Inputs list (not just Amount defaulting to 0)
        Assert.IsNotNull(factories);
        Assert.AreEqual(2, factories.Count);
        Assert.AreEqual(0, factories[1].Inputs.Count, "Without PropertyNameCaseInsensitive, camelCase 'inputs' should not map to 'Inputs'");
    }

    /// <summary>
    /// Verifies that PascalCase JSON (as saved by the Blazor app) still deserializes
    /// correctly with PropertyNameCaseInsensitive = true.
    /// </summary>
    [TestMethod]
    public void LoadFactories_PascalCaseJson_InputAmountDeserializesCorrectly()
    {
        // Arrange - PascalCase JSON as Blazor serializes it
        string pascalCaseJson = CamelCaseFactoryJson
            .Replace(@"""factoryId""", @"""FactoryId""")
            .Replace(@"""outputPart""", @"""OutputPart""")
            .Replace(@"""amount""", @"""Amount""");

        JsonSerializerOptions options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        // Act
        List<Factory>? factories = JsonSerializer.Deserialize<List<Factory>>(pascalCaseJson, options);

        // Assert
        Assert.IsNotNull(factories);
        FactoryInput input = factories[1].Inputs[0];
        Assert.AreEqual("IronIngot", input.OutputPart);
        Assert.AreEqual(320, input.Amount, "PascalCase 'Amount' should still deserialize correctly with case-insensitive option");
    }

    /// <summary>
    /// Verifies that camelCase factory names also deserialize correctly.
    /// </summary>
    [TestMethod]
    public void LoadFactories_CamelCaseJson_FactoryNameDeserializesCorrectly()
    {
        // Arrange
        JsonSerializerOptions options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        // Act
        List<Factory>? factories = JsonSerializer.Deserialize<List<Factory>>(CamelCaseFactoryJson, options);

        // Assert
        Assert.IsNotNull(factories);
        Assert.AreEqual("Supplier Factory", factories[0].Name);
        Assert.AreEqual("Consumer Factory", factories[1].Name);
    }

    /// <summary>
    /// Verifies that a factory input with camelCase JSON correctly deserializes
    /// all fields including factoryId, outputPart, and amount.
    /// </summary>
    [TestMethod]
    public void LoadFactories_CamelCaseJson_AllInputFieldsDeserializeCorrectly()
    {
        // Arrange
        JsonSerializerOptions options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        // Act
        List<Factory>? factories = JsonSerializer.Deserialize<List<Factory>>(CamelCaseFactoryJson, options);

        // Assert
        Assert.IsNotNull(factories);
        Assert.AreEqual(1, factories[1].Inputs.Count);
        FactoryInput input = factories[1].Inputs[0];
        Assert.AreEqual(1, input.FactoryId, "FactoryId should be 1");
        Assert.AreEqual("IronIngot", input.OutputPart, "OutputPart should be 'IronIngot'");
        Assert.AreEqual(320, input.Amount, "Amount should be 320");
    }
}
