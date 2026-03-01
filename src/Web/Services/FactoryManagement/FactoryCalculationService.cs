using Web.Models;
using Web.Models.Factory;
using FactoryPowerItem = Web.Models.Factory.PowerItem;

namespace Web.Services.FactoryManagement;

/// <summary>
/// Core factory calculation service.
/// Mirrors the factory management utilities from the Vue implementation:
/// factory.ts, products.ts, parts.ts, buildings.ts, exports.ts, problems.ts
/// </summary>
public class FactoryCalculationService : IFactoryCalculationService
{
    private readonly IFactoryCommonService _common;

    public FactoryCalculationService(IFactoryCommonService common)
    {
        _common = common;
    }

    // ── Factory management (factory.ts) ──────────────────────────────────────

    /// <inheritdoc/>
    public Factory NewFactory(string name = "A new factory", int? order = null, int? id = null)
    {
        return new Factory
        {
            Id = id ?? Random.Shared.Next(1, 10000),
            Name = name,
            Products = new List<FactoryItem>(),
            ByProducts = new List<ByProductItem>(),
            PowerProducers = new List<FactoryPowerProducer>(),
            Inputs = new List<FactoryInput>(),
            PreviousInputs = new List<FactoryInput>(),
            Parts = new Dictionary<string, PartMetrics>(),
            BuildingRequirements = new Dictionary<string, BuildingRequirement>(),
            Dependencies = new FactoryDependency(),
            ExportCalculator = new Dictionary<string, ExportCalculatorSettings>(),
            RawResources = new Dictionary<string, WorldRawResource>(),
            Power = new FactoryPower(),
            RequirementsSatisfied = true,
            UsingRawResourcesOnly = false,
            Hidden = false,
            HasProblem = false,
            InSync = null,
            SyncState = new Dictionary<string, FactorySyncState>(),
            SyncStatePower = new Dictionary<string, FactoryPowerSyncState>(),
            DisplayOrder = order ?? -1,
            Tasks = new List<FactoryTask>(),
            Notes = string.Empty,
            DataVersion = "2025-01-03",
        };
    }

    /// <inheritdoc/>
    public int CountActiveTasks(Factory factory)
    {
        return factory.Tasks.Count(t => !t.Completed);
    }

    /// <inheritdoc/>
    public void ReorderFactory(Factory factory, string direction, List<Factory> allFactories)
    {
        int currentOrder = factory.DisplayOrder;
        int targetOrder;

        if (direction == "up" && currentOrder > 0)
        {
            targetOrder = currentOrder - 1;
        }
        else if (direction == "down" && currentOrder < allFactories.Count - 1)
        {
            targetOrder = currentOrder + 1;
        }
        else
        {
            return; // Invalid move
        }

        Factory? targetFactory = allFactories.Find(f => f.DisplayOrder == targetOrder);
        if (targetFactory != null)
        {
            targetFactory.DisplayOrder = currentOrder;
            factory.DisplayOrder = targetOrder;
        }

        RegenerateSortOrders(allFactories);
    }

    /// <inheritdoc/>
    public void RegenerateSortOrders(List<Factory> factories)
    {
        factories.Sort((a, b) => a.DisplayOrder.CompareTo(b.DisplayOrder));
        for (int i = 0; i < factories.Count; i++)
        {
            factories[i].DisplayOrder = i;
        }
    }

    /// <inheritdoc/>
    public Factory CalculateFactory(Factory factory, List<Factory> allFactories, GameData gameData)
    {
        Console.WriteLine($"factory: CalculateFactory started {factory.Name}");

        factory.RawResources = new Dictionary<string, WorldRawResource>();
        factory.Parts = new Dictionary<string, PartMetrics>();

        CalculateProducts(factory, gameData);
        CalculateFactoryBuildingsAndPower(factory, gameData);
        CalculateParts(factory, gameData);
        CalculateSyncState(factory);
        CalculateHasProblem(allFactories);

        Console.WriteLine($"factory: CalculateFactory completed {factory.Name}");
        return factory;
    }

    /// <inheritdoc/>
    public void CalculateFactories(List<Factory> factories, GameData gameData)
    {
        Console.WriteLine("factory: Calculating factories");

        // First pass: generate part metrics needed for dependency evaluation
        foreach (Factory factory in factories)
        {
            CalculateFactory(factory, factories, gameData);
        }

        // Second pass: re-run after dependencies are established
        foreach (Factory factory in factories)
        {
            CalculateFactory(factory, factories, gameData);
        }

        Console.WriteLine("factory: Calculations completed");
    }

    // ── Products (products.ts) ────────────────────────────────────────────────

    /// <inheritdoc/>
    public void CalculateProducts(Factory factory, GameData gameData)
    {
        foreach (FactoryItem product in factory.Products)
        {
            product.Requirements = new Dictionary<string, RequirementAmount>(); // Prevent orphaning

            Recipe? recipe = _common.GetRecipe(product.Recipe, gameData);
            if (recipe == null)
            {
                Console.WriteLine($"calculateProductRequirements: Recipe with ID {product.Recipe} not found. It could be the user has not yet selected one.");
                continue;
            }

            if (product.Amount <= 0)
            {
                product.Amount = 1;
                Console.WriteLine("products: CalculateProducts: Product amount is <= 0, force setting to 1 to prevent calculation errors.");
            }

            foreach (RecipeItem ingredient in recipe.Ingredients)
            {
                if (double.IsNaN(ingredient.Amount))
                {
                    Console.Error.WriteLine($"Invalid ingredient amount for ingredient \"{ingredient.Part}\". Skipping.");
                    continue;
                }

                // Calculate parts required per minute to make the product
                double productIngredientRatio = product.Amount / recipe.Products[0].PerMin;
                double ingredientRequired = ingredient.PerMin * productIngredientRatio;
                ingredientRequired = Math.Round(ingredientRequired * 1000) / 1000;

                if (!product.Requirements.ContainsKey(ingredient.Part))
                {
                    product.Requirements[ingredient.Part] = new RequirementAmount { Amount = 0 };
                }

                product.Requirements[ingredient.Part].Amount += ingredientRequired;
            }
        }

        CalculateByProducts(factory, gameData);
    }

    /// <inheritdoc/>
    public void CalculateByProducts(Factory factory, GameData gameData)
    {
        factory.ByProducts = new List<ByProductItem>(); // Prevent orphaning/duplication

        foreach (FactoryItem product in factory.Products)
        {
            product.ByProducts = new List<ByProductItem>(); // Prevent orphaning/duplication

            Recipe? recipe = _common.GetRecipe(product.Recipe, gameData);
            if (recipe == null)
            {
                Console.WriteLine("CalculateByProducts: Could not get recipe, user may not have picked one yet.");
                continue;
            }

            List<RecipeItem> byProducts = recipe.Products.Where(p => p.IsByProduct == true).ToList();
            if (byProducts.Count == 0)
            {
                continue;
            }

            foreach (RecipeItem byProduct in byProducts)
            {
                double byProductRatio = byProduct.Amount / recipe.Products[0].Amount;
                double byProductAmount = product.Amount * byProductRatio;

                product.ByProducts!.Add(new ByProductItem
                {
                    Id = byProduct.Part,
                    ByProductOf = product.Id,
                    Amount = byProductAmount,
                });

                ByProductItem? existing = factory.ByProducts.Find(p => p.Id == byProduct.Part);
                if (existing == null)
                {
                    factory.ByProducts.Add(new ByProductItem
                    {
                        Id = byProduct.Part,
                        Amount = byProductAmount,
                        ByProductOf = product.Id,
                    });
                }
                else
                {
                    existing.Amount += byProductAmount;
                }
            }
        }
    }

    // ── Parts (parts.ts) ─────────────────────────────────────────────────────

    /// <inheritdoc/>
    public void CalculateParts(Factory factory, GameData gameData)
    {
        CalculatePartMetrics(factory, gameData);

        // If factory has no products there is nothing to check — mark satisfied.
        if (factory.Products.Count == 0)
        {
            factory.RequirementsSatisfied = true;
            return;
        }

        factory.RequirementsSatisfied = factory.Parts.All(kv => kv.Value.Satisfied);

        // Flag if every production part is a raw resource.
        factory.UsingRawResourcesOnly = true;
        foreach (KeyValuePair<string, PartMetrics> kv in factory.Parts)
        {
            if (!kv.Value.IsRaw && kv.Value.AmountRequiredProduction > 0)
            {
                factory.UsingRawResourcesOnly = false;
                break;
            }
        }
    }

    /// <inheritdoc/>
    public void CalculatePartMetrics(Factory factory, GameData gameData)
    {
        factory.Parts = new Dictionary<string, PartMetrics>();
        factory.RawResources = new Dictionary<string, WorldRawResource>();

        CalculatePartRequirements(factory);
        CalculatePartSupply(factory);
        CalculatePartRaw(factory, gameData);
        CalculateExportable(factory);

        foreach (string part in factory.Parts.Keys.ToList())
        {
            // Remove empty-string part keys
            if (part == string.Empty)
            {
                Console.Error.WriteLine("CalculatePartMetrics: Part key is an empty string! Flushing part data.");
                factory.Parts.Remove(part);
                continue;
            }

            PartMetrics partData = factory.Parts[part];
            partData.AmountRemaining = partData.AmountSupplied - partData.AmountRequired;
            partData.Satisfied = partData.AmountRemaining >= 0;
        }
    }

    /// <inheritdoc/>
    public void CalculatePartRequirements(Factory factory)
    {
        // Requirements from production
        foreach (FactoryItem product in factory.Products)
        {
            _common.CreateNewPart(factory, product.Id);

            if (product.Requirements == null)
            {
                Console.Error.WriteLine($"CalculatePartRequirements: Requirements is null for product '{product.Id}'. Skipping.");
                continue;
            }

            foreach (KeyValuePair<string, RequirementAmount> reqKv in product.Requirements)
            {
                if (reqKv.Value == null || reqKv.Value.Amount == 0)
                {
                    Console.Error.WriteLine($"CalculatePartRequirements - products: Amount is missing from product!");
                    continue;
                }

                _common.CreateNewPart(factory, reqKv.Key);
                factory.Parts[reqKv.Key].AmountRequiredProduction += reqKv.Value.Amount;
            }
        }

        // Requirements from power production
        foreach (FactoryPowerProducer producer in factory.PowerProducers)
        {
            if (producer.Ingredients.Count == 0)
            {
                Console.Error.WriteLine("CalculatePartRequirements - powerProducers: Ingredients are missing from producer!");
                continue;
            }

            foreach (FactoryPowerItem ingredient in producer.Ingredients)
            {
                _common.CreateNewPart(factory, ingredient.Part);
                if (ingredient.PerMin == 0)
                {
                    // PerMin is 0 when the user has not yet specified fuel amounts - this is expected
                    continue;
                }
                factory.Parts[ingredient.Part].AmountRequiredPower += ingredient.PerMin;
            }
        }

        // Requirements from export dependencies
        List<FactoryDependencyRequest> requests = GetRequestsForFactory(factory);
        foreach (FactoryDependencyRequest request in requests)
        {
            _common.CreateNewPart(factory, request.Part);
            factory.Parts[request.Part].AmountRequiredExports += request.Amount;
        }

        // Sum up requirements
        foreach (KeyValuePair<string, PartMetrics> kv in factory.Parts)
        {
            kv.Value.AmountRequired =
                kv.Value.AmountRequiredProduction +
                kv.Value.AmountRequiredPower +
                kv.Value.AmountRequiredExports;
        }
    }

    /// <inheritdoc/>
    public void CalculatePartSupply(Factory factory)
    {
        // Supply from inputs
        foreach (FactoryInput input in factory.Inputs)
        {
            if (string.IsNullOrEmpty(input.OutputPart))
            {
                Console.Error.WriteLine("CalculatePartSupply - inputs: Output part is missing from input!");
                continue;
            }
            _common.CreateNewPart(factory, input.OutputPart);
            if (input.Amount == 0)
            {
                Console.Error.WriteLine("CalculatePartSupply - inputs: Amount is missing from input!");
                continue;
            }
            factory.Parts[input.OutputPart].AmountSuppliedViaInput += input.Amount;
        }

        // Supply from products
        foreach (FactoryItem product in factory.Products)
        {
            if (product.Amount == 0)
            {
                Console.Error.WriteLine("CalculatePartSupply - products: Amount is missing from product!");
                continue;
            }

            _common.CreateNewPart(factory, product.Id);
            factory.Parts[product.Id].AmountSuppliedViaProduction += product.Amount;

            if (product.ByProducts != null)
            {
                foreach (ByProductItem byProduct in product.ByProducts)
                {
                    _common.CreateNewPart(factory, byProduct.Id);
                    factory.Parts[byProduct.Id].AmountSuppliedViaProduction += byProduct.Amount;
                }
            }
        }

        // Supply from power producer byproducts (e.g. nuclear waste)
        foreach (FactoryPowerProducer producer in factory.PowerProducers)
        {
            if (producer.Byproduct == null)
            {
                continue;
            }

            _common.CreateNewPart(factory, producer.Byproduct.Part);
            factory.Parts[producer.Byproduct.Part].AmountSuppliedViaProduction += producer.Byproduct.Amount;
        }

        // Sum up supply
        foreach (KeyValuePair<string, PartMetrics> kv in factory.Parts)
        {
            kv.Value.AmountSupplied =
                kv.Value.AmountSuppliedViaInput +
                kv.Value.AmountSuppliedViaProduction;
        }
    }

    /// <inheritdoc/>
    public void CalculatePartRaw(Factory factory, GameData gameData)
    {
        foreach (KeyValuePair<string, PartMetrics> kv in factory.Parts)
        {
            string part = kv.Key;
            PartMetrics partData = kv.Value;

            bool isRaw = gameData.Items.RawResources.ContainsKey(part);
            partData.IsRaw = isRaw;

            if (!isRaw)
            {
                continue;
            }

            partData.AmountSuppliedViaRaw = partData.AmountRequired;

            partData.AmountSupplied =
                partData.AmountSuppliedViaInput +
                partData.AmountSuppliedViaProduction +
                partData.AmountSuppliedViaRaw;

            if (!factory.RawResources.ContainsKey(part))
            {
                factory.RawResources[part] = new WorldRawResource
                {
                    Id = part,
                    Name = gameData.Items.RawResources[part].Name,
                    Amount = 0,
                };
            }
            factory.RawResources[part].Amount += partData.AmountRequired;
        }
    }

    /// <inheritdoc/>
    public void CalculateExportable(Factory factory)
    {
        foreach (KeyValuePair<string, PartMetrics> kv in factory.Parts)
        {
            PartMetrics partData = kv.Value;

            if (partData.AmountRequiredExports > 0)
            {
                partData.Exportable = true;
            }

            if (partData.AmountSuppliedViaProduction > 0)
            {
                partData.Exportable = true;
            }
        }
    }

    // ── Buildings &amp; power (buildings.ts) ───────────────────────────────────────

    /// <inheritdoc/>
    public void CalculateProductBuildings(Factory factory, GameData gameData)
    {
        foreach (FactoryItem product in factory.Products)
        {
            if (string.IsNullOrEmpty(product.Recipe))
            {
                product.BuildingRequirements = new BuildingRequirement();
                continue;
            }

            Recipe? recipe = _common.GetRecipe(product.Recipe, gameData);
            if (recipe == null)
            {
                Console.WriteLine($"CalculateProductBuildings: Recipe with ID {product.Recipe} not found. It could be the user has not yet selected one.");
                continue;
            }

            RecipeItem? productInRecipe = recipe.Products.FirstOrDefault(p => p.Part == product.Id);
            if (productInRecipe == null)
            {
                product.BuildingRequirements = new BuildingRequirement();
                continue;
            }

            RecipeBuilding buildingData = recipe.Building;
            double buildingCount = product.Amount / productInRecipe.PerMin;

            // Apply underclocking formula for fractional buildings
            // See: https://satisfactory.wiki.gg/wiki/Clock_speed
            double wholeBuildingCount = Math.Floor(buildingCount);
            double fractionalBuildingCount = buildingCount - wholeBuildingCount;
            double powerConsumed = (buildingData.Power * wholeBuildingCount) +
                                   (buildingData.Power * Math.Pow(fractionalBuildingCount, 1.321928));

            product.BuildingRequirements = new BuildingRequirement
            {
                Name = buildingData.Name,
                Amount = buildingCount,
                PowerConsumed = powerConsumed,
            };

            if (!factory.BuildingRequirements.ContainsKey(buildingData.Name))
            {
                factory.BuildingRequirements[buildingData.Name] = new BuildingRequirement
                {
                    Name = buildingData.Name,
                    Amount = 0,
                    PowerConsumed = 0,
                };
            }

            BuildingRequirement facBuilding = factory.BuildingRequirements[buildingData.Name];
            double newPowerConsumed = (facBuilding.PowerConsumed ?? 0) + powerConsumed;
            facBuilding.Amount += Math.Ceiling(buildingCount);
            facBuilding.PowerConsumed = Math.Round(newPowerConsumed, 3);
        }
    }

    /// <inheritdoc/>
    public void CalculatePowerProducerBuildings(Factory factory, GameData gameData)
    {
        foreach (FactoryPowerProducer producer in factory.PowerProducers)
        {
            PowerRecipe? recipe = _common.GetPowerRecipeById(producer.Recipe, gameData);
            if (recipe == null)
            {
                Console.WriteLine($"CalculatePowerProducerBuildings: Recipe with ID {producer.Recipe} not found. It could be the user has not yet selected one.");
                continue;
            }

            if (!factory.BuildingRequirements.ContainsKey(producer.Building))
            {
                factory.BuildingRequirements[producer.Building] = new BuildingRequirement
                {
                    Name = producer.Building,
                    Amount = 0,
                    PowerProduced = 0,
                };
            }

            BuildingRequirement buildingData = factory.BuildingRequirements[producer.Building];
            buildingData.Amount += Math.Ceiling(producer.BuildingAmount);

            if (buildingData.PowerProduced == null)
            {
                buildingData.PowerProduced = 0;
            }

            double wholeBuildingCount = Math.Floor(producer.BuildingAmount);
            double fractionalBuildingCount = producer.BuildingAmount - wholeBuildingCount;
            double powerProduced = ((recipe.Building.Power) * wholeBuildingCount) +
                                   (recipe.Building.Power * Math.Pow(fractionalBuildingCount, 1.321928));

            buildingData.PowerProduced += Math.Round(powerProduced, 3);
        }
    }

    /// <inheritdoc/>
    public void CalculateFactoryBuildingsAndPower(Factory factory, GameData gameData)
    {
        factory.BuildingRequirements = new Dictionary<string, BuildingRequirement>();

        CalculateProductBuildings(factory, gameData);
        CalculatePowerProducerBuildings(factory, gameData);

        factory.Power = new FactoryPower
        {
            Consumed = 0,
            Produced = 0,
            Difference = 0,
        };

        foreach (KeyValuePair<string, BuildingRequirement> kv in factory.BuildingRequirements)
        {
            factory.Power.Consumed += kv.Value.PowerConsumed ?? 0;
            factory.Power.Produced += kv.Value.PowerProduced ?? 0;
        }

        factory.Power.Difference = factory.Power.Produced - factory.Power.Consumed;
    }

    // ── Exports / dependency requests (exports.ts) ───────────────────────────

    /// <inheritdoc/>
    public List<FactoryDependencyRequest> GetRequestsForFactory(Factory factory)
    {
        if (factory.Dependencies?.Requests == null ||
            factory.Dependencies.Requests.Count == 0)
        {
            return new List<FactoryDependencyRequest>();
        }

        return factory.Dependencies.Requests
            .Values
            .SelectMany(requests => requests)
            .ToList();
    }

    // ── Problems (problems.ts) ────────────────────────────────────────────────

    /// <inheritdoc/>
    public void CalculateHasProblem(List<Factory> factories)
    {
        foreach (Factory factory in factories)
        {
            factory.HasProblem = false;

            if (!factory.RequirementsSatisfied)
            {
                factory.HasProblem = true;
                continue;
            }

            if (factory.Dependencies?.Metrics == null)
            {
                continue;
            }

            foreach (KeyValuePair<string, FactoryDependencyMetrics> kv in factory.Dependencies.Metrics)
            {
                if (!kv.Value.IsRequestSatisfied)
                {
                    factory.HasProblem = true;
                    break;
                }
            }
        }
    }

    // ── Sync state (syncState.ts) ─────────────────────────────────────────────

    /// <inheritdoc/>
    public bool ValidForGameSync(Factory factory)
    {
        bool hasProduct = factory.Products.Count > 0 && !string.IsNullOrEmpty(factory.Products[0].Recipe);
        bool hasPowerProducer = factory.PowerProducers.Count > 0 && !string.IsNullOrEmpty(factory.PowerProducers[0].Building);
        return hasProduct || hasPowerProducer;
    }

    /// <inheritdoc/>
    public void SetSyncState(Factory factory)
    {
        factory.SyncState = new Dictionary<string, FactorySyncState>();
        factory.SyncStatePower = new Dictionary<string, FactoryPowerSyncState>();

        foreach (FactoryItem product in factory.Products)
        {
            factory.SyncState[product.Id] = new FactorySyncState
            {
                Amount = product.Amount,
                Recipe = product.Recipe,
            };
        }

        foreach (FactoryPowerProducer powerProducer in factory.PowerProducers)
        {
            factory.SyncStatePower[powerProducer.Building] = new FactoryPowerSyncState
            {
                PowerAmount = powerProducer.PowerAmount,
                BuildingAmount = powerProducer.BuildingAmount,
                Recipe = powerProducer.Recipe,
                IngredientAmount = powerProducer.IngredientAmount,
            };
        }

        factory.InSync = true;
    }

    /// <inheritdoc/>
    public void ResetSyncState(Factory factory)
    {
        factory.InSync = null;
        factory.SyncState = new Dictionary<string, FactorySyncState>();
        factory.SyncStatePower = new Dictionary<string, FactoryPowerSyncState>();
    }

    /// <inheritdoc/>
    public void CalculateSyncState(Factory factory)
    {
        // If factory has never been synced, nothing to check.
        if (factory.InSync == null)
        {
            return;
        }

        // Only check factories currently marked as in sync.
        if (factory.InSync != true)
        {
            return;
        }

        // Empty factory — drop out of sync.
        if (factory.Products.Count == 0 && factory.PowerProducers.Count == 0)
        {
            factory.InSync = false;
            return;
        }

        // Check product count mismatch (fuel-only factories legitimately have no products).
        if (factory.Products.Count != factory.SyncState.Count)
        {
            bool isFuelOnly = factory.Products.Count == 0 && factory.PowerProducers.Count > 0 && factory.SyncState.Count == 0;
            if (!isFuelOnly)
            {
                factory.InSync = false;
                return;
            }
        }

        // Check all power producers deleted.
        if (factory.PowerProducers.Count == 0 && factory.SyncStatePower.Count > 0)
        {
            factory.InSync = false;
            return;
        }

        // Check power producer count mismatch.
        if (factory.PowerProducers.Count != factory.SyncStatePower.Count)
        {
            factory.InSync = false;
            return;
        }

        // Check individual products.
        foreach (FactoryItem product in factory.Products)
        {
            if (!factory.SyncState.TryGetValue(product.Id, out FactorySyncState? syncState))
            {
                factory.InSync = false;
                return;
            }

            if (syncState.Amount != product.Amount || syncState.Recipe != product.Recipe)
            {
                factory.InSync = false;
                return;
            }
        }

        // Check individual power producers.
        foreach (FactoryPowerProducer powerProducer in factory.PowerProducers)
        {
            if (!factory.SyncStatePower.TryGetValue(powerProducer.Building, out FactoryPowerSyncState? syncState))
            {
                factory.InSync = false;
                return;
            }

            if (syncState.BuildingAmount != powerProducer.BuildingAmount
                || syncState.Recipe != powerProducer.Recipe
                || syncState.PowerAmount != powerProducer.PowerAmount
                || syncState.IngredientAmount != powerProducer.IngredientAmount)
            {
                factory.InSync = false;
                return;
            }
        }
    }
}
