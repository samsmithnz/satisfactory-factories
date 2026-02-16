using System.Text.Json;
using Web.Models.Factory;

namespace Web.Services;

/// <summary>
/// Service for managing demo factory plans.
/// Provides pre-configured factory setups for demonstration purposes.
/// </summary>
public class DemoPlansService
{
    private readonly IAppStateService _appState;
    private readonly LoadingService _loadingService;

    public DemoPlansService(IAppStateService appState, LoadingService loadingService)
    {
        _appState = appState;
        _loadingService = loadingService;
    }

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

    /// <summary>
    /// Loads a demo plan template asynchronously with loading progress.
    /// </summary>
    /// <param name="templateName">Name of the template to load.</param>
    public async Task LoadDemoPlanAsync(string templateName)
    {
        // Get the demo plan data based on template name
        List<Factory> factories = GetDemoPlanByName(templateName);
        
        // Initialize loading overlay
        _loadingService.Initialize($"Loading {templateName}", factories.Count + 2);
        
        // Small delay to let UI update
        await Task.Delay(50);
        
        // Clear existing factories
        _loadingService.IncrementStep("Clearing existing factories...");
        _appState.ClearFactories();
        await Task.Delay(100);
        
        // Load factories one by one
        foreach (Factory factory in factories)
        {
            _loadingService.IncrementStep($"Loading {factory.Name}...");
            _appState.AddFactory(factory);
            await Task.Delay(75);
        }
        
        // Save to localStorage
        _loadingService.IncrementStep("Saving to local storage...", isFinalStep: true);
        await _appState.SaveFactoriesAsync();
        await Task.Delay(100);
    }

    /// <summary>
    /// Gets a demo plan by template name.
    /// </summary>
    /// <param name="templateName">Name of the template.</param>
    /// <returns>List of factories for the demo plan.</returns>
    private List<Factory> GetDemoPlanByName(string templateName)
    {
        return templateName switch
        {
            "Simple" => GetSimpleDemoPlan(),
            _ => GetSimpleDemoPlan() // Default to simple
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
