using Web.Models;
using Web.Models.Factory;

namespace Web.Services.FactoryManagement;

/// <summary>
/// Interface for factory calculation services.
/// Mirrors the factory management utilities from the Vue implementation:
/// factory.ts, products.ts, parts.ts, buildings.ts, exports.ts, problems.ts
/// </summary>
public interface IFactoryCalculationService
{
    // ── Factory management (factory.ts) ──────────────────────────────────────

    /// <summary>
    /// Creates a new factory with default values.
    /// </summary>
    Factory NewFactory(string name = "A new factory", int? order = null, int? id = null);

    /// <summary>
    /// Counts the number of incomplete tasks in a factory.
    /// </summary>
    int CountActiveTasks(Factory factory);

    /// <summary>
    /// Moves a factory one step up or down in the display order.
    /// </summary>
    void ReorderFactory(Factory factory, string direction, List<Factory> allFactories);

    /// <summary>
    /// Regenerates sequential display order values across all factories.
    /// </summary>
    void RegenerateSortOrders(List<Factory> factories);

    /// <summary>
    /// Runs the full calculation pipeline for a single factory.
    /// </summary>
    Factory CalculateFactory(Factory factory, List<Factory> allFactories, GameData gameData, bool loadMode = false);

    /// <summary>
    /// Runs the full calculation pipeline for all factories (two-pass).
    /// </summary>
    void CalculateFactories(List<Factory> factories, GameData gameData);

    // ── Products (products.ts) ────────────────────────────────────────────────

    /// <summary>
    /// Calculates product requirements and by-products for all products in the factory.
    /// </summary>
    void CalculateProducts(Factory factory, GameData gameData);

    /// <summary>
    /// Calculates by-products for all products in the factory.
    /// </summary>
    void CalculateByProducts(Factory factory, GameData gameData);

    // ── Parts (parts.ts) ─────────────────────────────────────────────────────

    /// <summary>
    /// Calculates part satisfaction metrics for the factory.
    /// </summary>
    void CalculateParts(Factory factory, GameData gameData);

    /// <summary>
    /// Recalculates all part metrics (clears and recomputes from scratch).
    /// </summary>
    void CalculatePartMetrics(Factory factory, GameData gameData);

    /// <summary>
    /// Accumulates the amount each part is required for production, power, and exports.
    /// </summary>
    void CalculatePartRequirements(Factory factory);

    /// <summary>
    /// Accumulates the amount each part is supplied via inputs and internal production.
    /// </summary>
    void CalculatePartSupply(Factory factory);

    /// <summary>
    /// Marks raw-resource parts as satisfied and fills factory.RawResources.
    /// </summary>
    void CalculatePartRaw(Factory factory, GameData gameData);

    /// <summary>
    /// Marks parts that are produced internally or requested via exports as exportable.
    /// </summary>
    void CalculateExportable(Factory factory);

    // ── Buildings &amp; power (buildings.ts) ───────────────────────────────────────

    /// <summary>
    /// Calculates building requirements for all products in the factory.
    /// </summary>
    void CalculateProductBuildings(Factory factory, GameData gameData);

    /// <summary>
    /// Calculates building requirements for all power producers in the factory.
    /// </summary>
    void CalculatePowerProducerBuildings(Factory factory, GameData gameData);

    /// <summary>
    /// Aggregates building requirements and net power for the factory.
    /// </summary>
    void CalculateFactoryBuildingsAndPower(Factory factory, GameData gameData);

    // ── Exports / dependency requests (exports.ts) ───────────────────────────

    /// <summary>
    /// Calculates and registers dependency requests between factories based on their inputs.
    /// Updates the provider factory's <see cref="FactoryDependency.Requests"/> and
    /// recalculates its metrics and parts.
    /// </summary>
    void CalculateFactoryDependencies(Factory factory, List<Factory> allFactories, GameData gameData, bool loadMode = false);

    /// <summary>
    /// Builds <see cref="FactoryDependency.Metrics"/> from the current Requests registered
    /// on the factory by other factories.
    /// </summary>
    void CalculateDependencyMetrics(Factory factory);

    /// <summary>
    /// Fills the supply and satisfaction fields in <see cref="FactoryDependency.Metrics"/>
    /// after parts have been calculated.
    /// </summary>
    void CalculateDependencyMetricsSupply(Factory factory);

    /// <summary>
    /// Returns all dependency requests that originate from this factory (i.e. things other
    /// factories are asking this factory to supply).
    /// </summary>
    List<FactoryDependencyRequest> GetRequestsForFactory(Factory factory);

    // ── Problems (problems.ts) ────────────────────────────────────────────────

    /// <summary>
    /// Sets the HasProblem flag on every factory in the list.
    /// </summary>
    void CalculateHasProblem(List<Factory> factories);

    // ── Sync state (syncState.ts) ─────────────────────────────────────────────

    /// <summary>
    /// Returns true if the factory has enough configured products or power producers
    /// to be eligible for game sync marking.
    /// </summary>
    bool ValidForGameSync(Factory factory);

    /// <summary>
    /// Records the current products and power producers as the sync baseline and
    /// marks the factory as in sync.
    /// </summary>
    void SetSyncState(Factory factory);

    /// <summary>
    /// Clears the sync baseline and resets InSync to null (unknown).
    /// </summary>
    void ResetSyncState(Factory factory);

    /// <summary>
    /// Checks whether a previously-synced factory has drifted out of sync.
    /// No-ops if InSync is null (never synced).
    /// </summary>
    void CalculateSyncState(Factory factory);
}
