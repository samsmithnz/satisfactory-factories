using System.Text.Json;
using Web.Config;
using Web.Models;

namespace Web.Services;

public class GameDataService
{
    private readonly HttpClient _httpClient;
    private GameData? _gameData;
    private bool _isLoaded;

    public GameDataService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<GameData> LoadGameDataAsync()
    {
        if (_isLoaded && _gameData != null)
        {
            return _gameData;
        }

        string fileName = $"gameData_v{AppConfig.DataVersion}.json";
        
        try
        {
            string jsonContent = await _httpClient.GetStringAsync(fileName);
            
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            
            _gameData = JsonSerializer.Deserialize<GameData>(jsonContent, options);
            
            if (_gameData == null)
            {
                throw new InvalidOperationException("Failed to deserialize game data");
            }
            
            _isLoaded = true;
            return _gameData;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error loading game data from {fileName}", ex);
        }
    }

    public GameData GetGameData()
    {
        if (!_isLoaded || _gameData == null)
        {
            throw new InvalidOperationException("Game data not loaded. Call LoadGameDataAsync first.");
        }
        
        return _gameData;
    }

    public Recipe? GetRecipeById(string id)
    {
        if (!_isLoaded || _gameData == null || string.IsNullOrEmpty(id))
        {
            return null;
        }

        return _gameData.Recipes.FirstOrDefault(recipe => recipe.Id == id);
    }

    public PowerRecipe? GetPowerRecipeById(string id)
    {
        if (!_isLoaded || _gameData == null || string.IsNullOrEmpty(id))
        {
            return null;
        }

        return _gameData.PowerGenerationRecipes.FirstOrDefault(recipe => recipe.Id == id);
    }

    public List<Recipe> GetRecipesForPart(string part)
    {
        if (!_isLoaded || _gameData == null || string.IsNullOrEmpty(part))
        {
            return new List<Recipe>();
        }

        return _gameData.Recipes
            .Where(recipe => recipe.Products.Any(product => product.Part == part))
            .ToList();
    }

    public List<PowerRecipe> GetRecipesForPowerProducer(string building)
    {
        if (!_isLoaded || _gameData == null || string.IsNullOrEmpty(building))
        {
            return new List<PowerRecipe>();
        }

        return _gameData.PowerGenerationRecipes
            .Where(recipe => recipe.Building.Name == building)
            .ToList();
    }

    public string GetDefaultRecipeForPart(string part)
    {
        List<Recipe> recipes = GetRecipesForPart(part);
        
        if (recipes.Count == 1)
        {
            return recipes[0].Id;
        }

        Recipe? exactRecipe = recipes.FirstOrDefault(recipe => recipe.Id == part);
        if (exactRecipe != null)
        {
            return exactRecipe.Id;
        }

        List<Recipe> defaultRecipes = recipes.Where(recipe => !recipe.IsAlternate).ToList();
        if (defaultRecipes.Count == 1)
        {
            return defaultRecipes[0].Id;
        }

        return string.Empty;
    }

    public PowerRecipe? GetDefaultRecipeForPowerProducer(string building)
    {
        List<PowerRecipe> recipes = GetRecipesForPowerProducer(building);

        if (recipes.Count == 0)
        {
            return null;
        }

        return recipes[0];
    }

    public PowerRecipe? GetGeneratorFuelRecipeByPart(string part)
    {
        if (!_isLoaded || _gameData == null || string.IsNullOrEmpty(part))
        {
            return null;
        }

        List<PowerRecipe> recipes = _gameData.PowerGenerationRecipes
            .Where(recipe => recipe.Byproduct?.Part == part)
            .ToList();

        if (recipes.Count == 0)
        {
            return null;
        }

        return recipes[0];
    }
}
