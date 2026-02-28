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
    private readonly HttpClient _httpClient;

    public DemoPlansService(IAppStateService appState, LoadingService loadingService, HttpClient httpClient)
    {
        _appState = appState;
        _loadingService = loadingService;
        _httpClient = httpClient;
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

        JsonSerializerOptions options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        List<Factory>? factories = JsonSerializer.Deserialize<List<Factory>>(jsonData, options);
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
            },
            new DemoPlanTemplate
            {
                Name = "Demo",
                Description = "Contains 7 factories with a mix of fluids, solids and multiple dependencies, along with power generation. Has a purposeful bottleneck on Copper Basics to demonstrate the bottleneck feature, and multiple missing resources for the Uranium Power.",
                IsDebug = false
            },
            new DemoPlanTemplate
            {
                Name = "Mael's \"MegaPlan\"",
                Description = "A real-life plan created by Maelstrome. This is considered a very large plan, and makes use of all features of the planner.",
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
        List<Factory> factories = await GetDemoPlanByNameAsync(templateName);
        
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
    /// Gets a demo plan by template name asynchronously.
    /// </summary>
    /// <param name="templateName">Name of the template.</param>
    /// <returns>List of factories for the demo plan.</returns>
    private async Task<List<Factory>> GetDemoPlanByNameAsync(string templateName)
    {
        return templateName switch
        {
            "Simple" => GetSimpleDemoPlan(),
            "Demo" => await GetDemoPlanFromFileAsync("sample-data/templates/demo-template.json"),
            "Mael's \"MegaPlan\"" => await GetDemoPlanFromFileAsync("sample-data/templates/mael-template.json"),
            _ => GetSimpleDemoPlan() // Default to simple
        };
    }

    /// <summary>
    /// Loads a demo plan from a JSON file in wwwroot.
    /// </summary>
    /// <param name="fileName">Relative path to the JSON file.</param>
    /// <returns>List of factories loaded from the file.</returns>
    private async Task<List<Factory>> GetDemoPlanFromFileAsync(string fileName)
    {
        string jsonContent = await _httpClient.GetStringAsync(fileName);
        JsonSerializerOptions options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        List<Factory>? factories = JsonSerializer.Deserialize<List<Factory>>(jsonContent, options);
        return factories ?? new List<Factory>();
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
