using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Web.Models.Factory;
using Web.Services;

namespace Web.Tests.Services;

[TestClass]
public class DemoPlansServiceTests
{
    private TestAppStateService _testAppState = null!;
    private LoadingService _loadingService = null!;
    private TestHttpMessageHandler _httpHandler = null!;
    private HttpClient _httpClient = null!;

    [TestInitialize]
    public void Setup()
    {
        _testAppState = new TestAppStateService();
        _loadingService = new LoadingService();
        _httpHandler = new TestHttpMessageHandler();
        _httpClient = new HttpClient(_httpHandler) { BaseAddress = new Uri("http://localhost/") };
    }

    [TestCleanup]
    public void Cleanup()
    {
        _httpClient?.Dispose();
    }

    [TestMethod]
    public void GetSimpleDemoPlanShouldReturnFactories()
    {
        // Arrange
        DemoPlansService service = new DemoPlansService(_testAppState, _loadingService, _httpClient);

        // Act
        List<Factory> factories = service.GetSimpleDemoPlan();

        // Assert
        Assert.IsNotNull(factories);
        Assert.AreEqual(2, factories.Count);
    }

    [TestMethod]
    public void GetSimpleDemoPlanShouldHaveCorrectFactoryNames()
    {
        // Arrange
        DemoPlansService service = new DemoPlansService(_testAppState, _loadingService, _httpClient);

        // Act
        List<Factory> factories = service.GetSimpleDemoPlan();

        // Assert
        Assert.AreEqual("Iron Ingots", factories[0].Name);
        Assert.AreEqual("Iron Plates", factories[1].Name);
    }

    [TestMethod]
    public void GetSimpleDemoPlanShouldHaveCorrectFactoryIds()
    {
        // Arrange
        DemoPlansService service = new DemoPlansService(_testAppState, _loadingService, _httpClient);

        // Act
        List<Factory> factories = service.GetSimpleDemoPlan();

        // Assert
        Assert.AreEqual(0, factories[0].Id);
        Assert.AreEqual(1, factories[1].Id);
    }

    [TestMethod]
    public void GetSimpleDemoPlanFirstFactoryShouldHaveProduct()
    {
        // Arrange
        DemoPlansService service = new DemoPlansService(_testAppState, _loadingService, _httpClient);

        // Act
        List<Factory> factories = service.GetSimpleDemoPlan();

        // Assert
        Assert.IsNotNull(factories[0].Products);
        Assert.AreEqual(1, factories[0].Products.Count);
        Assert.AreEqual("IronIngot", factories[0].Products[0].Id);
        Assert.AreEqual(100, factories[0].Products[0].Amount);
    }

    [TestMethod]
    public void GetSimpleDemoPlanSecondFactoryShouldHaveInputAndProduct()
    {
        // Arrange
        DemoPlansService service = new DemoPlansService(_testAppState, _loadingService, _httpClient);

        // Act
        List<Factory> factories = service.GetSimpleDemoPlan();

        // Assert
        // Check product
        Assert.IsNotNull(factories[1].Products);
        Assert.AreEqual(1, factories[1].Products.Count);
        Assert.AreEqual("IronPlate", factories[1].Products[0].Id);
        Assert.AreEqual(100, factories[1].Products[0].Amount);

        // Check input
        Assert.IsNotNull(factories[1].Inputs);
        Assert.AreEqual(1, factories[1].Inputs.Count);
        Assert.AreEqual(0, factories[1].Inputs[0].FactoryId);
        Assert.AreEqual("IronIngot", factories[1].Inputs[0].OutputPart);
        Assert.AreEqual(100, factories[1].Inputs[0].Amount);
    }

    [TestMethod]
    public void GetAvailableTemplatesShouldReturnTemplateList()
    {
        // Arrange
        DemoPlansService service = new DemoPlansService(_testAppState, _loadingService, _httpClient);

        // Act
        List<DemoPlanTemplate> templates = service.GetAvailableTemplates();

        // Assert
        Assert.IsNotNull(templates);
        Assert.IsTrue(templates.Count > 0);
    }

    [TestMethod]
    public void GetAvailableTemplatesShouldIncludeSimpleTemplate()
    {
        // Arrange
        DemoPlansService service = new DemoPlansService(_testAppState, _loadingService, _httpClient);

        // Act
        List<DemoPlanTemplate> templates = service.GetAvailableTemplates();

        // Assert
        DemoPlanTemplate? simpleTemplate = templates.Find(t => t.Name == "Simple");
        Assert.IsNotNull(simpleTemplate);
        Assert.IsFalse(simpleTemplate.IsDebug);
        Assert.IsFalse(string.IsNullOrEmpty(simpleTemplate.Description));
    }

    [TestMethod]
    public void GetAvailableTemplatesShouldIncludeDemoTemplate()
    {
        // Arrange
        DemoPlansService service = new DemoPlansService(_testAppState, _loadingService, _httpClient);

        // Act
        List<DemoPlanTemplate> templates = service.GetAvailableTemplates();

        // Assert
        DemoPlanTemplate? demoTemplate = templates.Find(t => t.Name == "Demo");
        Assert.IsNotNull(demoTemplate);
        Assert.IsFalse(demoTemplate.IsDebug);
        Assert.IsFalse(string.IsNullOrEmpty(demoTemplate.Description));
    }

    [TestMethod]
    public void GetAvailableTemplatesShouldIncludeMaelTemplate()
    {
        // Arrange
        DemoPlansService service = new DemoPlansService(_testAppState, _loadingService, _httpClient);

        // Act
        List<DemoPlanTemplate> templates = service.GetAvailableTemplates();

        // Assert
        DemoPlanTemplate? maelTemplate = templates.Find(t => t.Name == "Mael's \"MegaPlan\"");
        Assert.IsNotNull(maelTemplate);
        Assert.IsFalse(maelTemplate.IsDebug);
        Assert.IsFalse(string.IsNullOrEmpty(maelTemplate.Description));
    }

    [TestMethod]
    public void GetAvailableTemplatesShouldReturnThreeNonDebugTemplates()
    {
        // Arrange
        DemoPlansService service = new DemoPlansService(_testAppState, _loadingService, _httpClient);

        // Act
        List<DemoPlanTemplate> templates = service.GetAvailableTemplates();

        // Assert
        Assert.AreEqual(3, templates.Count);
        Assert.IsTrue(templates.All(t => !t.IsDebug));
    }

    [TestMethod]
    public void GetSimpleDemoPlanShouldReturnNewInstancesEachTime()
    {
        // Arrange
        DemoPlansService service = new DemoPlansService(_testAppState, _loadingService, _httpClient);

        // Act
        List<Factory> factories1 = service.GetSimpleDemoPlan();
        List<Factory> factories2 = service.GetSimpleDemoPlan();

        // Assert
        Assert.AreNotSame(factories1, factories2);
    }

    [TestMethod]
    public async Task LoadDemoPlanAsyncShouldLoadDemoTemplateFromFile()
    {
        // Arrange - use minimal JSON that represents the template file format (camelCase from Vue)
        string demoJson = @"[
            {""id"": 1, ""name"": ""Oil Processing"", ""products"": [], ""byProducts"": [], ""powerProducers"": [], ""inputs"": [], ""previousInputs"": [], ""parts"": {}, ""buildingRequirements"": {}, ""requirementsSatisfied"": false, ""exportCalculator"": {}, ""dependencies"": {""requests"": {}, ""metrics"": {}}, ""rawResources"": {}, ""power"": {}, ""usingRawResourcesOnly"": false, ""hidden"": false, ""hasProblem"": false, ""inSync"": null, ""syncState"": {}, ""syncStatePower"": {}, ""displayOrder"": 1, ""tasks"": [], ""notes"": """", ""dataVersion"": ""2025-01-03""},
            {""id"": 2, ""name"": ""Copper Ingots"", ""products"": [], ""byProducts"": [], ""powerProducers"": [], ""inputs"": [], ""previousInputs"": [], ""parts"": {}, ""buildingRequirements"": {}, ""requirementsSatisfied"": false, ""exportCalculator"": {}, ""dependencies"": {""requests"": {}, ""metrics"": {}}, ""rawResources"": {}, ""power"": {}, ""usingRawResourcesOnly"": false, ""hidden"": false, ""hasProblem"": false, ""inSync"": null, ""syncState"": {}, ""syncStatePower"": {}, ""displayOrder"": 2, ""tasks"": [], ""notes"": """", ""dataVersion"": ""2025-01-03""}
        ]";
        _httpHandler.Response = new System.Net.Http.HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(demoJson)
        };
        DemoPlansService service = new DemoPlansService(_testAppState, _loadingService, _httpClient);

        // Act
        await service.LoadDemoPlanAsync("Demo");

        // Assert
        Assert.AreEqual(2, _testAppState.GetFactories().Count);
        Assert.AreEqual("Oil Processing", _testAppState.GetFactories()[0].Name);
    }

    [TestMethod]
    public async Task LoadDemoPlanAsyncShouldLoadMaelTemplateFromFile()
    {
        // Arrange - use minimal JSON that represents the template file format (camelCase from Vue)
        string maelJson = @"[
            {""id"": 1, ""name"": ""Concrete MegaFac"", ""products"": [], ""byProducts"": [], ""powerProducers"": [], ""inputs"": [], ""previousInputs"": [], ""parts"": {}, ""buildingRequirements"": {}, ""requirementsSatisfied"": false, ""exportCalculator"": {}, ""dependencies"": {""requests"": {}, ""metrics"": {}}, ""rawResources"": {}, ""power"": {}, ""usingRawResourcesOnly"": false, ""hidden"": false, ""hasProblem"": false, ""inSync"": null, ""syncState"": {}, ""syncStatePower"": {}, ""displayOrder"": 1, ""tasks"": [], ""notes"": """", ""dataVersion"": ""2025-01-03""},
            {""id"": 2, ""name"": ""Stone Input"", ""products"": [], ""byProducts"": [], ""powerProducers"": [], ""inputs"": [], ""previousInputs"": [], ""parts"": {}, ""buildingRequirements"": {}, ""requirementsSatisfied"": false, ""exportCalculator"": {}, ""dependencies"": {""requests"": {}, ""metrics"": {}}, ""rawResources"": {}, ""power"": {}, ""usingRawResourcesOnly"": false, ""hidden"": false, ""hasProblem"": false, ""inSync"": null, ""syncState"": {}, ""syncStatePower"": {}, ""displayOrder"": 2, ""tasks"": [], ""notes"": """", ""dataVersion"": ""2025-01-03""}
        ]";
        _httpHandler.Response = new System.Net.Http.HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(maelJson)
        };
        DemoPlansService service = new DemoPlansService(_testAppState, _loadingService, _httpClient);

        // Act
        await service.LoadDemoPlanAsync("Mael's \"MegaPlan\"");

        // Assert
        Assert.AreEqual(2, _testAppState.GetFactories().Count);
        Assert.AreEqual("Concrete MegaFac", _testAppState.GetFactories()[0].Name);
    }

    /// <summary>
    /// Test HTTP message handler for mocking HTTP responses.
    /// </summary>
    private class TestHttpMessageHandler : HttpMessageHandler
    {
        public HttpResponseMessage? Response { get; set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (Response == null)
            {
                throw new InvalidOperationException($"No HTTP response configured for request: {request.RequestUri}");
            }
            return Task.FromResult(Response);
        }
    }

    /// <summary>
    /// Test implementation of IAppStateService for unit testing.
    /// </summary>
    private class TestAppStateService : IAppStateService
    {
        private List<Factory> _factories = new List<Factory>();
        public event Action? OnChange;

        public List<Factory> GetFactories() => _factories;
        public void SetFactories(List<Factory> factories) => _factories = factories;
        public void AddFactory(Factory factory) => _factories.Add(factory);
        public void ClearFactories() => _factories.Clear();
        public Task<bool> LoadFactoriesAsync() => Task.FromResult(true);
        public Task SaveFactoriesAsync() => Task.CompletedTask;
        public bool GetHelpTextShown() => false;
        public void SetHelpTextShown(bool shown) { }
        public List<FactoryTab> GetFactoryTabs() => new List<FactoryTab>();
        public void AddFactoryTab(FactoryTab tab) { }
        public int GetCurrentFactoryTabIndex() => 0;
        public void SetCurrentFactoryTabIndex(int index) { }
        public DateTime? GetLastEdit() => null;
    }
}
