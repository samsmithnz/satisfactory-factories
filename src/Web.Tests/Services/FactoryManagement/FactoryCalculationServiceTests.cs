using Web.Models;
using Web.Models.Factory;
using Web.Services.FactoryManagement;

namespace Web.Tests.Services.FactoryManagement;

/// <summary>
/// Tests for FactoryCalculationService.
/// Mirrors the Vue factory management test suite (factory.spec.ts, parts.spec.ts,
/// products.spec.ts, buildings.spec.ts, exports.spec.ts, problems.spec.ts).
/// </summary>
[TestClass]
public sealed class FactoryCalculationServiceTests
{
    private FactoryCalculationService _service = null!;

    [TestInitialize]
    public void Initialize()
    {
        FactoryCommonService common = new FactoryCommonService();
        _service = new FactoryCalculationService(common);
    }

    // ── NewFactory (factory.ts: newFactory) ───────────────────────────────────

    [TestMethod]
    public void NewFactory_ShouldCreateFactoryWithRandomId()
    {
        Factory fac = _service.NewFactory("My new factory");

        Assert.IsTrue(fac.Id > 0);
        Assert.AreEqual("My new factory", fac.Name);
        Assert.AreEqual(0, fac.Products.Count);
        Assert.AreEqual(0, fac.Tasks.Count);
    }

    [TestMethod]
    public void NewFactory_ShouldUseProvidedIdAndOrder()
    {
        Factory fac = _service.NewFactory("Test", order: 3, id: 42);

        Assert.AreEqual(42, fac.Id);
        Assert.AreEqual(3, fac.DisplayOrder);
    }

    [TestMethod]
    public void NewFactory_ShouldHaveCorrectDefaults()
    {
        Factory fac = _service.NewFactory();

        Assert.IsTrue(fac.RequirementsSatisfied);
        Assert.IsFalse(fac.UsingRawResourcesOnly);
        Assert.IsFalse(fac.Hidden);
        Assert.IsFalse(fac.HasProblem);
        Assert.IsNull(fac.InSync);
        Assert.AreEqual("2025-01-03", fac.DataVersion);
        Assert.AreEqual(string.Empty, fac.Notes);
    }

    // ── CountActiveTasks (factory.ts: countActiveTasks) ───────────────────────

    [TestMethod]
    public void CountActiveTasks_ShouldReturnZeroForNoTasks()
    {
        Factory fac = _service.NewFactory();
        Assert.AreEqual(0, _service.CountActiveTasks(fac));
    }

    [TestMethod]
    public void CountActiveTasks_ShouldCountIncompleteTasks()
    {
        Factory fac = _service.NewFactory();
        fac.Tasks.Add(new FactoryTask { Title = "Task 1", Completed = false });
        fac.Tasks.Add(new FactoryTask { Title = "Task 2", Completed = false });
        fac.Tasks.Add(new FactoryTask { Title = "Task 3", Completed = true });

        Assert.AreEqual(2, _service.CountActiveTasks(fac));
    }

    [TestMethod]
    public void CountActiveTasks_ShouldNotCountCompletedTasks()
    {
        Factory fac = _service.NewFactory();
        fac.Tasks.Add(new FactoryTask { Title = "Task 1", Completed = true });

        Assert.AreEqual(0, _service.CountActiveTasks(fac));
    }

    // ── ReorderFactory (factory.ts: reorderFactory) ───────────────────────────

    [TestMethod]
    public void ReorderFactory_ShouldMoveFactoryUp()
    {
        Factory fac1 = _service.NewFactory("Factory 1", order: 0, id: 1);
        Factory fac2 = _service.NewFactory("Factory 2", order: 1, id: 2);
        Factory fac3 = _service.NewFactory("Factory 3", order: 2, id: 3);
        List<Factory> factories = new List<Factory> { fac1, fac2, fac3 };

        _service.ReorderFactory(fac2, "up", factories);

        Assert.AreEqual(0, fac2.DisplayOrder);
        Assert.AreEqual(1, fac1.DisplayOrder);
        Assert.AreEqual(2, fac3.DisplayOrder);
    }

    [TestMethod]
    public void ReorderFactory_ShouldMoveFactoryDown()
    {
        Factory fac1 = _service.NewFactory("Factory 1", order: 0, id: 1);
        Factory fac2 = _service.NewFactory("Factory 2", order: 1, id: 2);
        Factory fac3 = _service.NewFactory("Factory 3", order: 2, id: 3);
        List<Factory> factories = new List<Factory> { fac1, fac2, fac3 };

        _service.ReorderFactory(fac2, "down", factories);

        Assert.AreEqual(0, fac1.DisplayOrder);
        Assert.AreEqual(1, fac3.DisplayOrder);
        Assert.AreEqual(2, fac2.DisplayOrder);
    }

    [TestMethod]
    public void ReorderFactory_ShouldNotMoveFirstFactoryUp()
    {
        Factory fac1 = _service.NewFactory("Factory 1", order: 0, id: 1);
        Factory fac2 = _service.NewFactory("Factory 2", order: 1, id: 2);
        List<Factory> factories = new List<Factory> { fac1, fac2 };

        _service.ReorderFactory(fac1, "up", factories);

        Assert.AreEqual(0, fac1.DisplayOrder);
        Assert.AreEqual(1, fac2.DisplayOrder);
    }

    [TestMethod]
    public void ReorderFactory_ShouldNotMoveLastFactoryDown()
    {
        Factory fac1 = _service.NewFactory("Factory 1", order: 0, id: 1);
        Factory fac2 = _service.NewFactory("Factory 2", order: 1, id: 2);
        List<Factory> factories = new List<Factory> { fac1, fac2 };

        _service.ReorderFactory(fac2, "down", factories);

        Assert.AreEqual(0, fac1.DisplayOrder);
        Assert.AreEqual(1, fac2.DisplayOrder);
    }

    // ── RegenerateSortOrders (factory.ts: regenerateSortOrders) ───────────────

    [TestMethod]
    public void RegenerateSortOrders_ShouldSetSequentialDisplayOrders()
    {
        Factory fac1 = _service.NewFactory("Factory 1", order: 1);
        Factory fac2 = _service.NewFactory("Factory 2", order: 3);
        Factory fac3 = _service.NewFactory("Factory 3", order: 6);
        Factory fac4 = _service.NewFactory("Factory 4", order: 13);
        Factory fac5 = _service.NewFactory("Factory 5", order: 42);
        List<Factory> factories = new List<Factory> { fac1, fac2, fac3, fac4, fac5 };

        _service.RegenerateSortOrders(factories);

        Assert.AreEqual(0, fac1.DisplayOrder);
        Assert.AreEqual(1, fac2.DisplayOrder);
        Assert.AreEqual(2, fac3.DisplayOrder);
        Assert.AreEqual(3, fac4.DisplayOrder);
        Assert.AreEqual(4, fac5.DisplayOrder);
    }

    [TestMethod]
    public void RegenerateSortOrders_ShouldReorderEvenIfOutOfSync()
    {
        Factory fac1 = _service.NewFactory("Factory 1", order: 2);
        Factory fac2 = _service.NewFactory("Factory 2", order: 3);
        Factory fac3 = _service.NewFactory("Factory 3", order: 4);
        List<Factory> factories = new List<Factory> { fac1, fac2, fac3 };

        _service.RegenerateSortOrders(factories);

        Assert.AreEqual(0, fac1.DisplayOrder);
        Assert.AreEqual(1, fac2.DisplayOrder);
        Assert.AreEqual(2, fac3.DisplayOrder);
    }

    // ── CalculateProducts (products.ts: calculateProducts) ───────────────────

    [TestMethod]
    public void CalculateProducts_ShouldCalculateIngredientRequirements()
    {
        GameData gameData = TestDataHelper.CreateTestGameData();
        Factory factory = TestDataHelper.CreateTestFactory("Test Factory");
        TestDataHelper.AddProductToFactory(factory, "IronIngot", 30, "IronIngot");

        _service.CalculateProducts(factory, gameData);

        // IronIngot recipe: 30 OreIron/min => at 30/min production: ratio = 30/30 = 1
        // OreIron required = 30 * 1 = 30
        Assert.IsTrue(factory.Products[0].Requirements.ContainsKey("OreIron"));
        Assert.AreEqual(30, factory.Products[0].Requirements["OreIron"].Amount);
    }

    [TestMethod]
    public void CalculateProducts_ShouldClearOldRequirementsBeforeRecalculating()
    {
        GameData gameData = TestDataHelper.CreateTestGameData();
        Factory factory = TestDataHelper.CreateTestFactory("Test Factory");
        TestDataHelper.AddProductToFactory(factory, "IronIngot", 30, "IronIngot");

        // First calculation
        _service.CalculateProducts(factory, gameData);
        // Second calculation - should replace, not accumulate
        _service.CalculateProducts(factory, gameData);

        Assert.AreEqual(30, factory.Products[0].Requirements["OreIron"].Amount);
    }

    [TestMethod]
    public void CalculateProducts_ShouldHandleMissingRecipeGracefully()
    {
        GameData gameData = TestDataHelper.CreateTestGameData();
        Factory factory = TestDataHelper.CreateTestFactory("Test Factory");
        TestDataHelper.AddProductToFactory(factory, "SomePart", 10, "NonExistentRecipe");

        // Should not throw
        _service.CalculateProducts(factory, gameData);

        Assert.AreEqual(0, factory.Products[0].Requirements.Count);
    }

    [TestMethod]
    public void CalculateProducts_ShouldForceAmountToOneWhenZeroOrNegative()
    {
        GameData gameData = TestDataHelper.CreateTestGameData();
        Factory factory = TestDataHelper.CreateTestFactory("Test Factory");
        TestDataHelper.AddProductToFactory(factory, "IronIngot", 0, "IronIngot");

        _service.CalculateProducts(factory, gameData);

        Assert.AreEqual(1, factory.Products[0].Amount);
    }

    // ── CalculateByProducts (products.ts: calculateByProducts) ────────────────

    [TestMethod]
    public void CalculateByProducts_ShouldComputeByProductAmounts()
    {
        GameData gameData = TestDataHelper.CreateTestGameData();
        Factory factory = TestDataHelper.CreateTestFactory("Test Factory");
        // CompactedCoal recipe produces Water as byproduct (ratio 1:1)
        TestDataHelper.AddProductToFactory(factory, "CompactedCoal", 25, "CompactedCoal");

        _service.CalculateProducts(factory, gameData);

        Assert.AreEqual(1, factory.ByProducts.Count);
        Assert.AreEqual("Water", factory.ByProducts[0].Id);
        Assert.AreEqual(25, factory.ByProducts[0].Amount);
    }

    [TestMethod]
    public void CalculateByProducts_ShouldClearByProductsBeforeRecalculating()
    {
        GameData gameData = TestDataHelper.CreateTestGameData();
        Factory factory = TestDataHelper.CreateTestFactory("Test Factory");
        TestDataHelper.AddProductToFactory(factory, "CompactedCoal", 25, "CompactedCoal");

        _service.CalculateProducts(factory, gameData);
        _service.CalculateProducts(factory, gameData);

        // Should still be 1, not accumulated
        Assert.AreEqual(1, factory.ByProducts.Count);
    }

    // ── CalculateParts / CalculatePartMetrics (parts.ts) ─────────────────────

    [TestMethod]
    public void CalculateParts_ShouldMarkSatisfiedWhenSupplyMeetsRequirement()
    {
        GameData gameData = TestDataHelper.CreateTestGameData();
        Factory factory = TestDataHelper.CreateTestFactory("Test Factory");
        TestDataHelper.AddProductToFactory(factory, "IronIngot", 30, "IronIngot");

        _service.CalculateProducts(factory, gameData);
        _service.CalculateParts(factory, gameData);

        // OreIron is a raw resource => satisfied
        Assert.IsTrue(factory.Parts["OreIron"].Satisfied);
        Assert.IsTrue(factory.Parts["OreIron"].IsRaw);
    }

    [TestMethod]
    public void CalculateParts_ShouldMarkRequirementsSatisfiedFalseWhenDeficit()
    {
        GameData gameData = TestDataHelper.CreateTestGameData();
        Factory factory = TestDataHelper.CreateTestFactory("Test Factory");
        // Produce IronPlate which requires IronIngot, but don't produce IronIngot
        TestDataHelper.AddProductToFactory(factory, "IronPlate", 20, "IronPlate");

        _service.CalculateProducts(factory, gameData);
        _service.CalculateParts(factory, gameData);

        // IronIngot is not raw and not produced => unsatisfied
        Assert.IsFalse(factory.Parts["IronIngot"].Satisfied);
        Assert.IsFalse(factory.RequirementsSatisfied);
    }

    [TestMethod]
    public void CalculateParts_ShouldSetRequirementsSatisfiedTrueForEmptyFactory()
    {
        GameData gameData = TestDataHelper.CreateTestGameData();
        Factory factory = TestDataHelper.CreateTestFactory("Test Factory");

        _service.CalculateParts(factory, gameData);

        Assert.IsTrue(factory.RequirementsSatisfied);
    }

    [TestMethod]
    public void CalculateParts_ShouldFlagUsingRawResourcesOnly()
    {
        GameData gameData = TestDataHelper.CreateTestGameData();
        Factory factory = TestDataHelper.CreateTestFactory("Test Factory");
        // IronIngot requires only OreIron (raw)
        TestDataHelper.AddProductToFactory(factory, "IronIngot", 30, "IronIngot");

        _service.CalculateProducts(factory, gameData);
        _service.CalculateParts(factory, gameData);

        Assert.IsTrue(factory.UsingRawResourcesOnly);
    }

    [TestMethod]
    public void CalculateParts_ShouldNotFlagUsingRawResourcesOnlyWhenNonRawRequired()
    {
        GameData gameData = TestDataHelper.CreateTestGameData();
        Factory factory = TestDataHelper.CreateTestFactory("Test Factory");
        // IronPlate requires IronIngot (non-raw)
        TestDataHelper.AddProductToFactory(factory, "IronPlate", 20, "IronPlate");

        _service.CalculateProducts(factory, gameData);
        _service.CalculateParts(factory, gameData);

        Assert.IsFalse(factory.UsingRawResourcesOnly);
    }

    // ── CalculatePartSupply (parts.ts: calculatePartSupply) ───────────────────

    [TestMethod]
    public void CalculatePartSupply_ShouldAccountForInputs()
    {
        GameData gameData = TestDataHelper.CreateTestGameData();
        Factory factory = TestDataHelper.CreateTestFactory("Test Factory", id: 1);
        TestDataHelper.AddProductToFactory(factory, "IronPlate", 20, "IronPlate");
        factory.Inputs.Add(new FactoryInput
        {
            FactoryId = 2,
            OutputPart = "IronIngot",
            Amount = 30,
        });

        _service.CalculateProducts(factory, gameData);
        _service.CalculateParts(factory, gameData);

        Assert.AreEqual(30, factory.Parts["IronIngot"].AmountSuppliedViaInput);
        Assert.IsTrue(factory.Parts["IronIngot"].Satisfied);
    }

    // ── CalculateExportable (parts.ts: calculateExportable) ───────────────────

    [TestMethod]
    public void CalculateExportable_ShouldMarkProducedPartsAsExportable()
    {
        GameData gameData = TestDataHelper.CreateTestGameData();
        Factory factory = TestDataHelper.CreateTestFactory("Test Factory");
        TestDataHelper.AddProductToFactory(factory, "IronIngot", 30, "IronIngot");

        _service.CalculateProducts(factory, gameData);
        _service.CalculatePartMetrics(factory, gameData);

        Assert.IsTrue(factory.Parts["IronIngot"].Exportable);
    }

    // ── CalculateProductBuildings (buildings.ts: calculateProductBuildings) ───

    [TestMethod]
    public void CalculateProductBuildings_ShouldComputeBuildingCount()
    {
        GameData gameData = TestDataHelper.CreateTestGameData();
        Factory factory = TestDataHelper.CreateTestFactory("Test Factory");
        TestDataHelper.AddProductToFactory(factory, "IronIngot", 30, "IronIngot");

        _service.CalculateProducts(factory, gameData);
        _service.CalculateFactoryBuildingsAndPower(factory, gameData);

        // IronIngot recipe: 30/min per building => building count = 30/30 = 1
        Assert.IsTrue(factory.BuildingRequirements.ContainsKey("smeltermk1"));
        Assert.AreEqual(1, factory.BuildingRequirements["smeltermk1"].Amount);
    }

    [TestMethod]
    public void CalculateFactoryBuildingsAndPower_ShouldComputePowerConsumed()
    {
        GameData gameData = TestDataHelper.CreateTestGameData();
        Factory factory = TestDataHelper.CreateTestFactory("Test Factory");
        TestDataHelper.AddProductToFactory(factory, "IronIngot", 30, "IronIngot");

        _service.CalculateProducts(factory, gameData);
        _service.CalculateFactoryBuildingsAndPower(factory, gameData);

        // Smelter: 4 MW per building, 1 building => 4 MW consumed
        Assert.AreEqual(4, factory.Power.Consumed, 0.001);
        Assert.AreEqual(0, factory.Power.Produced, 0.001);
        Assert.AreEqual(-4, factory.Power.Difference, 0.001);
    }

    [TestMethod]
    public void CalculateFactoryBuildingsAndPower_ShouldClearPreviousBuildingData()
    {
        GameData gameData = TestDataHelper.CreateTestGameData();
        Factory factory = TestDataHelper.CreateTestFactory("Test Factory");
        TestDataHelper.AddProductToFactory(factory, "IronIngot", 30, "IronIngot");

        _service.CalculateProducts(factory, gameData);
        _service.CalculateFactoryBuildingsAndPower(factory, gameData);
        _service.CalculateFactoryBuildingsAndPower(factory, gameData);

        // Should not double-count
        Assert.AreEqual(1, factory.BuildingRequirements["smeltermk1"].Amount);
    }

    // ── GetRequestsForFactory (exports.ts: getRequestsForFactory) ─────────────

    [TestMethod]
    public void GetRequestsForFactory_ShouldReturnEmptyListWhenNoDependencies()
    {
        Factory factory = TestDataHelper.CreateTestFactory("Test Factory");

        List<FactoryDependencyRequest> requests = _service.GetRequestsForFactory(factory);

        Assert.AreEqual(0, requests.Count);
    }

    [TestMethod]
    public void GetRequestsForFactory_ShouldReturnAllRequestsFlattened()
    {
        Factory factory = TestDataHelper.CreateTestFactory("Test Factory");
        factory.Dependencies.Requests["2"] = new List<FactoryDependencyRequest>
        {
            new FactoryDependencyRequest { RequestingFactoryId = 2, Part = "IronIngot", Amount = 30 },
        };
        factory.Dependencies.Requests["3"] = new List<FactoryDependencyRequest>
        {
            new FactoryDependencyRequest { RequestingFactoryId = 3, Part = "IronPlate", Amount = 10 },
            new FactoryDependencyRequest { RequestingFactoryId = 3, Part = "IronIngot", Amount = 20 },
        };

        List<FactoryDependencyRequest> requests = _service.GetRequestsForFactory(factory);

        Assert.AreEqual(3, requests.Count);
    }

    // ── CalculateHasProblem (problems.ts: calculateHasProblem) ────────────────

    [TestMethod]
    public void CalculateHasProblem_ShouldSetHasProblemFalseWhenSatisfied()
    {
        Factory factory = TestDataHelper.CreateTestFactory("Test Factory");
        factory.RequirementsSatisfied = true;
        List<Factory> factories = new List<Factory> { factory };

        _service.CalculateHasProblem(factories);

        Assert.IsFalse(factory.HasProblem);
    }

    [TestMethod]
    public void CalculateHasProblem_ShouldSetHasProblemTrueWhenRequirementsUnsatisfied()
    {
        Factory factory = TestDataHelper.CreateTestFactory("Test Factory");
        factory.RequirementsSatisfied = false;
        List<Factory> factories = new List<Factory> { factory };

        _service.CalculateHasProblem(factories);

        Assert.IsTrue(factory.HasProblem);
    }

    [TestMethod]
    public void CalculateHasProblem_ShouldHandleNullDependenciesGracefully()
    {
        Factory factory = TestDataHelper.CreateTestFactory("Test Factory");
        factory.RequirementsSatisfied = true;
        factory.Dependencies = null!;
        List<Factory> factories = new List<Factory> { factory };

        // Should not throw even with null Dependencies
        _service.CalculateHasProblem(factories);

        Assert.IsFalse(factory.HasProblem);
    }

    [TestMethod]
    public void CalculatePartRequirements_ShouldHandleNullRequirementsGracefully()
    {
        Factory factory = TestDataHelper.CreateTestFactory("Test Factory");
        factory.Products.Add(new FactoryItem
        {
            Id = "IronIngot",
            Recipe = "IronIngot",
            Amount = 30,
            DisplayOrder = 0,
            Requirements = null!,
            BuildingRequirements = new BuildingRequirement(),
        });

        // Should not throw even with null Requirements
        FactoryCommonService common = new FactoryCommonService();
        FactoryCalculationService service = new FactoryCalculationService(common);
        service.CalculatePartRequirements(factory);
    }

    [TestMethod]
    public void CalculateHasProblem_ShouldSetHasProblemTrueWhenDependencyUnsatisfied()
    {
        Factory factory = TestDataHelper.CreateTestFactory("Test Factory");
        factory.RequirementsSatisfied = true;
        factory.Dependencies.Metrics["IronIngot"] = new FactoryDependencyMetrics
        {
            Part = "IronIngot",
            Request = 30,
            Supply = 20,
            IsRequestSatisfied = false,
            Difference = -10,
        };
        List<Factory> factories = new List<Factory> { factory };

        _service.CalculateHasProblem(factories);

        Assert.IsTrue(factory.HasProblem);
    }

    // ── CalculateFactory (full pipeline integration test) ─────────────────────

    [TestMethod]
    public void CalculateFactory_ShouldComputeFullPipelineForIronIngotFactory()
    {
        GameData gameData = TestDataHelper.CreateTestGameData();
        Factory factory = TestDataHelper.CreateTestFactory("Iron Ingot Factory", id: 1);
        TestDataHelper.AddProductToFactory(factory, "IronIngot", 30, "IronIngot");

        _service.CalculateFactory(factory, new List<Factory> { factory }, gameData);

        // Ingredients
        Assert.AreEqual(30, factory.Products[0].Requirements["OreIron"].Amount, 0.001);

        // Part metrics: OreIron is raw => satisfied
        Assert.IsTrue(factory.Parts["OreIron"].IsRaw);
        Assert.IsTrue(factory.Parts["OreIron"].Satisfied);

        // IronIngot produced at 30/min
        Assert.AreEqual(30, factory.Parts["IronIngot"].AmountSuppliedViaProduction, 0.001);
        Assert.IsTrue(factory.Parts["IronIngot"].Exportable);

        // Building requirements
        Assert.IsTrue(factory.BuildingRequirements.ContainsKey("smeltermk1"));

        // Requirements satisfied
        Assert.IsTrue(factory.RequirementsSatisfied);
        Assert.IsFalse(factory.HasProblem);
    }

    [TestMethod]
    public void CalculateFactory_ShouldDetectUnsatisfiedRequirementsForIronPlateWithoutIronIngot()
    {
        GameData gameData = TestDataHelper.CreateTestGameData();
        Factory factory = TestDataHelper.CreateTestFactory("Iron Plate Factory", id: 1);
        TestDataHelper.AddProductToFactory(factory, "IronPlate", 20, "IronPlate");

        _service.CalculateFactory(factory, new List<Factory> { factory }, gameData);

        // IronIngot is required but not produced or imported
        Assert.IsFalse(factory.Parts["IronIngot"].Satisfied);
        Assert.IsFalse(factory.RequirementsSatisfied);
        Assert.IsTrue(factory.HasProblem);
    }

    [TestMethod]
    public void CalculateFactories_ShouldCalculateMultipleFactories()
    {
        GameData gameData = TestDataHelper.CreateTestGameData();
        Factory fac1 = TestDataHelper.CreateTestFactory("Factory 1", id: 1, displayOrder: 0);
        Factory fac2 = TestDataHelper.CreateTestFactory("Factory 2", id: 2, displayOrder: 1);
        TestDataHelper.AddProductToFactory(fac1, "IronIngot", 30, "IronIngot");
        TestDataHelper.AddProductToFactory(fac2, "IronPlate", 20, "IronPlate");
        List<Factory> factories = new List<Factory> { fac1, fac2 };

        _service.CalculateFactories(factories, gameData);

        // fac1 should be satisfied (only needs raw OreIron)
        Assert.IsTrue(fac1.RequirementsSatisfied);
        // fac2 needs IronIngot which is not imported => unsatisfied
        Assert.IsFalse(fac2.RequirementsSatisfied);
    }

    // ── Sync state (syncState.ts) ─────────────────────────────────────────────

    [TestMethod]
    public void NewFactory_ShouldHaveNullInSync()
    {
        Factory factory = _service.NewFactory();
        Assert.IsNull(factory.InSync);
    }

    [TestMethod]
    public void ValidForGameSync_ReturnsFalseForEmptyFactory()
    {
        Factory factory = _service.NewFactory();
        Assert.IsFalse(_service.ValidForGameSync(factory));
    }

    [TestMethod]
    public void ValidForGameSync_ReturnsTrueWhenProductHasRecipe()
    {
        Factory factory = _service.NewFactory();
        TestDataHelper.AddProductToFactory(factory, "IronIngot", 30, "IronIngot");
        Assert.IsTrue(_service.ValidForGameSync(factory));
    }

    [TestMethod]
    public void SetSyncState_MarksFactoryAsInSync()
    {
        Factory factory = _service.NewFactory();
        TestDataHelper.AddProductToFactory(factory, "IronIngot", 30, "IronIngot");

        _service.SetSyncState(factory);

        Assert.IsTrue(factory.InSync);
        Assert.IsTrue(factory.SyncState.ContainsKey("IronIngot"));
        Assert.AreEqual(30, factory.SyncState["IronIngot"].Amount, 0.001);
        Assert.AreEqual("IronIngot", factory.SyncState["IronIngot"].Recipe);
    }

    [TestMethod]
    public void ResetSyncState_SetsInSyncToNull()
    {
        Factory factory = _service.NewFactory();
        TestDataHelper.AddProductToFactory(factory, "IronIngot", 30, "IronIngot");
        _service.SetSyncState(factory);

        _service.ResetSyncState(factory);

        Assert.IsNull(factory.InSync);
        Assert.AreEqual(0, factory.SyncState.Count);
        Assert.AreEqual(0, factory.SyncStatePower.Count);
    }

    [TestMethod]
    public void CalculateSyncState_DoesNothingWhenInSyncIsNull()
    {
        Factory factory = _service.NewFactory();
        TestDataHelper.AddProductToFactory(factory, "IronIngot", 30, "IronIngot");

        _service.CalculateSyncState(factory);

        Assert.IsNull(factory.InSync);
    }

    [TestMethod]
    public void CalculateSyncState_RemainsInSyncWhenNothingChanged()
    {
        Factory factory = _service.NewFactory();
        TestDataHelper.AddProductToFactory(factory, "IronIngot", 30, "IronIngot");
        _service.SetSyncState(factory);

        _service.CalculateSyncState(factory);

        Assert.IsTrue(factory.InSync);
    }

    [TestMethod]
    public void CalculateSyncState_DropsOutOfSyncWhenAmountChanges()
    {
        Factory factory = _service.NewFactory();
        TestDataHelper.AddProductToFactory(factory, "IronIngot", 30, "IronIngot");
        _service.SetSyncState(factory);

        factory.Products[0].Amount = 60;
        _service.CalculateSyncState(factory);

        Assert.IsFalse(factory.InSync);
    }

    [TestMethod]
    public void CalculateSyncState_DropsOutOfSyncWhenRecipeChanges()
    {
        Factory factory = _service.NewFactory();
        TestDataHelper.AddProductToFactory(factory, "IronIngot", 30, "IronIngot");
        _service.SetSyncState(factory);

        factory.Products[0].Recipe = "Alternate_IronIngot";
        _service.CalculateSyncState(factory);

        Assert.IsFalse(factory.InSync);
    }

    [TestMethod]
    public void CalculateSyncState_DropsOutOfSyncWhenProductAdded()
    {
        Factory factory = _service.NewFactory();
        TestDataHelper.AddProductToFactory(factory, "IronIngot", 30, "IronIngot");
        _service.SetSyncState(factory);

        TestDataHelper.AddProductToFactory(factory, "IronPlate", 20, "IronPlate");
        _service.CalculateSyncState(factory);

        Assert.IsFalse(factory.InSync);
    }

    [TestMethod]
    public void CalculateSyncState_DropsOutOfSyncWhenAllProductsRemoved()
    {
        Factory factory = _service.NewFactory();
        TestDataHelper.AddProductToFactory(factory, "IronIngot", 30, "IronIngot");
        _service.SetSyncState(factory);

        factory.Products.Clear();
        _service.CalculateSyncState(factory);

        Assert.IsFalse(factory.InSync);
    }
}
