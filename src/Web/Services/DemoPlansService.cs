using System.Text.Json;
using Web.Models.Factory;

namespace Web.Services;

/// <summary>
/// Service for managing demo factory plans.
/// Provides pre-configured factory setups for demonstration purposes.
/// </summary>
public class DemoPlansService
{
    /// <summary>
    /// Gets a simple demo plan with basic iron production.
    /// </summary>
    public List<Factory> GetSimpleDemoPlan()
    {
        string jsonData = @"[
  {
    ""id"": 0,
    ""name"": ""Iron Ingots"",
    ""inputs"": [],
    ""previousInputs"": [],
    ""products"": [
      {
        ""id"": ""IronIngot"",
        ""recipe"": ""IngotIron"",
        ""amount"": 100,
        ""displayOrder"": 0,
        ""requirements"": {},
        ""buildingRequirements"": { ""building"": """", ""amount"": 0 }
      }
    ],
    ""byProducts"": [],
    ""powerProducers"": [],
    ""parts"": {},
    ""buildingRequirements"": {},
    ""requirementsSatisfied"": false,
    ""exportCalculator"": {},
    ""dependencies"": { ""suppliers"": {}, ""requests"": {} },
    ""rawResources"": {},
    ""power"": { ""consumed"": 0, ""produced"": 0, ""net"": 0 },
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
    ""id"": 1,
    ""name"": ""Iron Plates"",
    ""inputs"": [
      {
        ""factoryId"": 0,
        ""outputPart"": ""IronIngot"",
        ""amount"": 100
      }
    ],
    ""previousInputs"": [],
    ""products"": [
      {
        ""id"": ""IronPlate"",
        ""recipe"": ""IronPlate"",
        ""amount"": 100,
        ""displayOrder"": 0,
        ""requirements"": {},
        ""buildingRequirements"": { ""building"": """", ""amount"": 0 }
      }
    ],
    ""byProducts"": [],
    ""powerProducers"": [],
    ""parts"": {},
    ""buildingRequirements"": {},
    ""requirementsSatisfied"": false,
    ""exportCalculator"": {},
    ""dependencies"": { ""suppliers"": {}, ""requests"": {} },
    ""rawResources"": {},
    ""power"": { ""consumed"": 0, ""produced"": 0, ""net"": 0 },
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

        List<Factory>? factories = JsonSerializer.Deserialize<List<Factory>>(jsonData);
        return factories ?? new List<Factory>();
    }

    /// <summary>
    /// Gets a list of all available demo plan templates.
    /// </summary>
    public List<DemoPlanTemplate> GetAvailableTemplates()
    {
        return new List<DemoPlanTemplate>
        {
            new DemoPlanTemplate
            {
                Name = "Simple",
                Description = "Very simple Iron Ingot and Iron Plate factory setup, with a single dependency link.",
                IsDebug = false
            }
        };
    }
}

/// <summary>
/// Represents a demo plan template.
/// </summary>
public class DemoPlanTemplate
{
    /// <summary>
    /// Name of the template.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description of what the template demonstrates.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Whether this is a debug/test template.
    /// </summary>
    public bool IsDebug { get; set; }
}
