using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.JSInterop;
using Web.Models.Sync;
using Web.Services;

namespace Web.Tests.Services;

[TestClass]
public class SyncServiceTests
{
    private TestHttpMessageHandler _httpHandler = null!;
    private HttpClient _httpClient = null!;
    private TestAppStateService _appState = null!;
    private TestConfigurationService _config = null!;
    private TestJSRuntime _jsRuntime = null!;
    private TestToastService _toast = null!;
    private SyncService _syncService = null!;

    [TestInitialize]
    public void Setup()
    {
        _httpHandler = new TestHttpMessageHandler();
        _httpClient = new HttpClient(_httpHandler);
        _appState = new TestAppStateService();
        _config = new TestConfigurationService();
        _jsRuntime = new TestJSRuntime();
        _toast = new TestToastService();
        
        _syncService = new SyncService(
            _httpClient,
            _appState,
            _config,
            _jsRuntime,
            _toast
        );
    }

    [TestCleanup]
    public void Cleanup()
    {
        _syncService?.Dispose();
        _httpClient?.Dispose();
    }

    [TestMethod]
    public void InitialStateShouldBeCorrect()
    {
        // Assert
        Assert.IsFalse(_syncService.DataSavePending);
        Assert.IsNull(_syncService.DataLastSaved);
        Assert.IsFalse(_syncService.StopSyncing);
        Assert.IsFalse(_syncService.IsSyncing);
    }

    [TestMethod]
    public void DetectedChangeShouldSetPendingFlag()
    {
        // Act
        _syncService.DetectedChange();

        // Assert
        Assert.IsTrue(_syncService.DataSavePending);
    }

    [TestMethod]
    public void StopSyncShouldDisableSyncing()
    {
        // Act
        _syncService.StopSync();

        // Assert
        Assert.IsTrue(_syncService.StopSyncing);
    }

    [TestMethod]
    public void GetLastSavedDisplayShouldReturnDefaultWhenNotSaved()
    {
        // Act
        string display = _syncService.GetLastSavedDisplay();

        // Assert
        Assert.AreEqual("Not saved yet, make a change!", display);
    }

    [TestMethod]
    public async Task HandleSyncAsyncShouldReturnFalseWhenNoDataPending()
    {
        // Act
        bool result = await _syncService.HandleSyncAsync();

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task HandleSyncAsyncShouldReturnFalseWhenSyncingStopped()
    {
        // Arrange
        _syncService.DetectedChange();
        _syncService.StopSync();

        // Act
        bool result = await _syncService.HandleSyncAsync();

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task HandleSyncAsyncShouldReturnFalseWhenNoFactories()
    {
        // Arrange
        _syncService.DetectedChange();

        // Act
        bool result = await _syncService.HandleSyncAsync();

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task HandleSyncAsyncShouldSendDataToServer()
    {
        // Arrange
        Web.Models.Factory.Factory factory = new Web.Models.Factory.Factory { Id = 1, Name = "Test Factory" };
        _appState.Factories.Add(factory);
        _syncService.DetectedChange();

        JsonDocument responseDoc = JsonDocument.Parse("{\"status\":\"ok\"}");
        _httpHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(responseDoc)
        };

        // Act
        bool result = await _syncService.HandleSyncAsync(force: true);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task HandleDataLoadAsyncShouldReturnNullWhenNoServerData()
    {
        // Arrange - simulate network error
        _httpHandler.ThrowException = new HttpRequestException("Network error");

        // Act
        string? result = await _syncService.HandleDataLoadAsync();

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task HandleDataLoadAsyncShouldForceLoadData()
    {
        // Arrange
        Web.Models.Factory.Factory serverFactory = new Web.Models.Factory.Factory { Id = 1, Name = "Server Factory" };
        BackendFactoryDataResponse serverResponse = new BackendFactoryDataResponse
        {
            User = "testuser",
            Data = new List<Web.Models.Factory.Factory> { serverFactory },
            LastSaved = DateTime.Now
        };

        _httpHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(serverResponse)
        };

        // Act
        string? result = await _syncService.HandleDataLoadAsync(forceLoad: true);

        // Assert
        Assert.AreEqual("true", result);
        Assert.AreEqual(1, _appState.Factories.Count);
    }

    [TestMethod]
    public async Task HandleDataLoadAsyncShouldDetectOutOfSync()
    {
        // Arrange - server data is newer than local
        DateTime serverTime = DateTime.Now;
        DateTime localTime = serverTime.AddMinutes(-5);
        _appState.LastEdit = localTime;

        Web.Models.Factory.Factory serverFactory = new Web.Models.Factory.Factory { Id = 1, Name = "Server Factory" };
        BackendFactoryDataResponse serverResponse = new BackendFactoryDataResponse
        {
            User = "testuser",
            Data = new List<Web.Models.Factory.Factory> { serverFactory },
            LastSaved = serverTime
        };

        _httpHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(serverResponse)
        };

        // Act
        string? result = await _syncService.HandleDataLoadAsync(forceLoad: false);

        // Assert
        Assert.AreEqual("oos", result);
    }

    [TestMethod]
    public async Task HandleDataLoadAsyncShouldNotDetectOOSWhenLocalIsNewer()
    {
        // Arrange - local data is newer than server
        DateTime serverTime = DateTime.Now.AddMinutes(-5);
        DateTime localTime = DateTime.Now;
        _appState.LastEdit = localTime;

        Web.Models.Factory.Factory serverFactory = new Web.Models.Factory.Factory { Id = 1, Name = "Server Factory" };
        BackendFactoryDataResponse serverResponse = new BackendFactoryDataResponse
        {
            User = "testuser",
            Data = new List<Web.Models.Factory.Factory> { serverFactory },
            LastSaved = serverTime
        };

        _httpHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(serverResponse)
        };

        // Act
        string? result = await _syncService.HandleDataLoadAsync(forceLoad: false);

        // Assert
        Assert.IsNull(result);
    }

    // Test helper classes
    private class TestHttpMessageHandler : HttpMessageHandler
    {
        public HttpResponseMessage? Response { get; set; }
        public Exception? ThrowException { get; set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (ThrowException != null)
            {
                throw ThrowException;
            }
            return Task.FromResult(Response ?? new HttpResponseMessage(HttpStatusCode.OK));
        }
    }

    private class TestAppStateService : IAppStateService
    {
        public List<Web.Models.Factory.Factory> Factories { get; set; } = new List<Web.Models.Factory.Factory>();
        public DateTime? LastEdit { get; set; }
        public event Action? OnChange;

        public List<Web.Models.Factory.Factory> GetFactories() => Factories;
        public void SetFactories(List<Web.Models.Factory.Factory> factories) { Factories = factories; }
        public void AddFactory(Web.Models.Factory.Factory factory) { Factories.Add(factory); }
        public void ClearFactories() { Factories.Clear(); }
        public Task<bool> LoadFactoriesAsync() => Task.FromResult(true);
        public Task SaveFactoriesAsync() => Task.CompletedTask;
        public bool GetHelpTextShown() => false;
        public void SetHelpTextShown(bool shown) { }
        public List<Web.Models.Factory.FactoryTab> GetFactoryTabs() => new List<Web.Models.Factory.FactoryTab>();
        public void AddFactoryTab(Web.Models.Factory.FactoryTab tab) { }
        public int GetCurrentFactoryTabIndex() => 0;
        public void SetCurrentFactoryTabIndex(int index) { }
        public DateTime? GetLastEdit() => LastEdit;
    }

    private class TestConfigurationService : IConfigurationService
    {
        public string GetApiUrl() => "http://test-api.com";
        public string GetDataVersion() => "1.0-29";
    }

    private class TestJSRuntime : IJSRuntime
    {
        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
        {
            return ValueTask.FromResult(default(TValue)!);
        }

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
        {
            return ValueTask.FromResult(default(TValue)!);
        }
    }

    private class TestToastService : IToastService
    {
        public event Action<string, ToastType>? OnShowToast;
        public void ShowToast(string message, ToastType type = ToastType.Info)
        {
            OnShowToast?.Invoke(message, type);
        }
    }
}
