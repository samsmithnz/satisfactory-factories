using Web.Models;
using Web.Models.Factory;

namespace Web.Services.FactoryManagement;

/// <summary>
/// Interface for common factory management utilities.
/// </summary>
public interface IFactoryCommonService
{
    /// <summary>
    /// Creates a new part in the factory if it doesn't already exist.
    /// </summary>
    /// <param name="factory">The factory to add the part to.</param>
    /// <param name="partId">The ID of the part to create.</param>
    void CreateNewPart(Factory factory, string partId);

    /// <summary>
    /// Gets a recipe by ID from the game data.
    /// </summary>
    /// <param name="recipeId">The recipe ID to look up.</param>
    /// <param name="gameData">The game data to search in.</param>
    /// <returns>The recipe if found, null otherwise.</returns>
    Recipe? GetRecipe(string recipeId, GameData gameData);

    /// <summary>
    /// Gets the display name for a part without using a data store.
    /// </summary>
    /// <param name="partId">The part ID to get the name for.</param>
    /// <param name="gameData">The game data to search in.</param>
    /// <returns>The display name of the part.</returns>
    string GetPartDisplayName(string partId, GameData gameData);

    /// <summary>
    /// Gets a power recipe by ID from the game data.
    /// </summary>
    /// <param name="recipeId">The power recipe ID to look up.</param>
    /// <param name="gameData">The game data to search in.</param>
    /// <returns>The power recipe if found, null otherwise.</returns>
    PowerRecipe? GetPowerRecipeById(string recipeId, GameData gameData);

    /// <summary>
    /// Gets the display name for a building.
    /// </summary>
    /// <param name="buildingId">The building ID to get the name for.</param>
    /// <returns>The friendly display name of the building.</returns>
    string GetBuildingDisplayName(string buildingId);
}
