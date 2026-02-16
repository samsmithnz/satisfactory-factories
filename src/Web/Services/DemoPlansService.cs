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
    ""Id"": 0,
    ""Name"": ""Iron Ingots"",
    ""Inputs"": [],
    ""PreviousInputs"": [],
    ""Products"": [
      {
        ""Id"": ""IronIngot"",
        ""Recipe"": ""IngotIron"",
        ""Amount"": 100,
        ""DisplayOrder"": 0,
        ""Requirements"": {},
        ""BuildingRequirements"": { ""Building"": """", ""Amount"": 0 }
      }
    ],
    ""ByProducts"": [],
    ""PowerProducers"": [],
    ""Parts"": {},
    ""BuildingRequirements"": {},
    ""RequirementsSatisfied"": false,
    ""ExportCalculator"": {},
    ""Dependencies"": { ""Suppliers"": {}, ""Requests"": {} },
    ""RawResources"": {},
    ""Power"": { ""Consumed"": 0, ""Produced"": 0, ""Net"": 0 },
    ""UsingRawResourcesOnly"": false,
    ""Hidden"": false,
    ""HasProblem"": false,
    ""InSync"": null,
    ""SyncState"": {},
    ""SyncStatePower"": {},
    ""DisplayOrder"": 1,
    ""Tasks"": [],
    ""Notes"": """",
    ""DataVersion"": ""2025-01-03""
  },
  {
    ""Id"": 1,
    ""Name"": ""Iron Plates"",
    ""Inputs"": [
      {
        ""FactoryId"": 0,
        ""OutputPart"": ""IronIngot"",
        ""Amount"": 100
      }
    ],
    ""PreviousInputs"": [],
    ""Products"": [
      {
        ""Id"": ""IronPlate"",
        ""Recipe"": ""IronPlate"",
        ""Amount"": 100,
        ""DisplayOrder"": 0,
        ""Requirements"": {},
        ""BuildingRequirements"": { ""Building"": """", ""Amount"": 0 }
      }
    ],
    ""ByProducts"": [],
    ""PowerProducers"": [],
    ""Parts"": {},
    ""BuildingRequirements"": {},
    ""RequirementsSatisfied"": false,
    ""ExportCalculator"": {},
    ""Dependencies"": { ""Suppliers"": {}, ""Requests"": {} },
    ""RawResources"": {},
    ""Power"": { ""Consumed"": 0, ""Produced"": 0, ""Net"": 0 },
    ""UsingRawResourcesOnly"": false,
    ""Hidden"": false,
    ""HasProblem"": false,
    ""InSync"": null,
    ""SyncState"": {},
    ""SyncStatePower"": {},
    ""DisplayOrder"": 2,
    ""Tasks"": [],
    ""Notes"": """",
    ""DataVersion"": ""2025-01-03""
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
