using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.JSInterop;
using Web.Models.Factory;
using Web.Models.Sync;

namespace Web.Services;

/// <summary>
/// Backend synchronization service for optional cloud data sync.
/// Port of sync-store.ts and SyncActions from Vue implementation.
/// </summary>
public class SyncService : ISyncService, IDisposable
{
    private readonly HttpClient _client;
    private readonly IAppStateService _state;
    private readonly IConfigurationService _config;
    private readonly IJSRuntime _js;
    private readonly IToastService _toast;
    
    private bool _needsSync;
    private DateTime? _lastSync;
    private bool _disabled;
    private bool _busy;
    private Timer? _ticker;

    public event Action? OnChange;
    public bool DataSavePending => _needsSync;
    public DateTime? DataLastSaved => _lastSync;
    public bool StopSyncing => _disabled;
    public bool IsSyncing => _busy;

    public SyncService(
        HttpClient client,
        IAppStateService state,
        IConfigurationService config,
        IJSRuntime js,
        IToastService toast)
    {
        _client = client;
        _state = state;
        _config = config;
        _js = js;
        _toast = toast;
        _state.OnChange += DetectedChange;
        Console.WriteLine("SyncService ready");
    }

    public void SetupTick()
    {
        _ticker?.Dispose();
        _ticker = new Timer(async _ => await ProcessTickAsync(), null, 10000, 10000);
        Console.WriteLine("SyncService tick enabled");
    }

    public void StopSync()
    {
        _ticker?.Dispose();
        _ticker = null;
        _disabled = true;
    }

    public void DetectedChange()
    {
        _needsSync = true;
    }

    public async Task<string?> HandleDataLoadAsync(bool forceLoad = false)
    {
        Console.WriteLine("SyncService loading from server");
        _busy = true;
        NotifyUpdate();

        try
        {
            BackendFactoryDataResponse? serverData = await FetchFromServerAsync();
            if (serverData == null)
            {
                Console.WriteLine("No server data available");
                return null;
            }

            if (forceLoad)
            {
                Console.WriteLine("Force loading server data");
                _state.SetFactories(serverData.Data);
                return "true";
            }

            bool outOfSync = DetectOutOfSync(serverData);
            Console.WriteLine($"Out of sync check: {outOfSync}");
            return outOfSync ? "oos" : null;
        }
        catch (Exception err)
        {
            Console.Error.WriteLine($"Load error: {err.Message}");
            _toast.ShowToast($"Load failed: {err.Message}", ToastType.Error);
            return null;
        }
        finally
        {
            _busy = false;
            NotifyUpdate();
        }
    }

    public async Task<bool> HandleSyncAsync(bool force = false)
    {
        Console.WriteLine($"HandleSync force={force}");
        return force 
            ? await PushToServerAsync(false, true)
            : await PushToServerAsync(_disabled, _needsSync);
    }

    public string GetLastSavedDisplay()
    {
        return _lastSync == null 
            ? "Not saved yet, make a change!" 
            : BuildDateDisplay(_lastSync.Value);
    }

    private async Task ProcessTickAsync()
    {
        if (_disabled || !_needsSync)
        {
            return;
        }

        try
        {
            bool success = await PushToServerAsync(_disabled, _needsSync);
            if (success)
            {
                _needsSync = false;
                _lastSync = DateTime.Now;
                await _js.InvokeVoidAsync("localStorage.setItem", "lastEdit", _lastSync.Value.ToString("o"));
                NotifyUpdate();
                Console.WriteLine("Tick sync complete");
            }
            else
            {
                ReportError(new Exception("Sync returned no result"));
            }
        }
        catch (Exception err)
        {
            ReportError(err);
        }
    }

    private async Task<bool> PushToServerAsync(bool isDisabled, bool hasPending)
    {
        if (isDisabled || !hasPending)
        {
            return false;
        }

        List<Factory> localData = _state.GetFactories();
        if (localData.Count == 0)
        {
            Console.WriteLine("No data to push");
            return false;
        }

        try
        {
            string endpoint = _config.GetApiUrl();
            HttpResponseMessage reply = await _client.PostAsJsonAsync($"{endpoint}/save", localData);

            if (reply.IsSuccessStatusCode)
            {
                JsonDocument? content = await reply.Content.ReadFromJsonAsync<JsonDocument>();
                Console.WriteLine($"Push successful: {content}");
                return true;
            }
            
            if (reply.StatusCode == System.Net.HttpStatusCode.InternalServerError ||
                reply.StatusCode == System.Net.HttpStatusCode.BadGateway)
            {
                throw new Exception("Server error 5xx");
            }

            Console.Error.WriteLine($"Push failed: {reply.StatusCode}");
            return false;
        }
        catch (Exception err)
        {
            Console.Error.WriteLine($"Push error: {err.Message}");
            throw;
        }
    }

    private async Task<BackendFactoryDataResponse?> FetchFromServerAsync()
    {
        try
        {
            string endpoint = _config.GetApiUrl();
            HttpResponseMessage reply = await _client.GetAsync($"{endpoint}/load");

            if (reply.IsSuccessStatusCode)
            {
                BackendFactoryDataResponse? parsed = 
                    await reply.Content.ReadFromJsonAsync<BackendFactoryDataResponse>();
                
                if (parsed?.Data == null)
                {
                    throw new Exception("Invalid server response");
                }
                return parsed;
            }

            Console.Error.WriteLine($"Fetch failed: {reply.StatusCode}");
            throw new Exception("Server unreachable");
        }
        catch (HttpRequestException err)
        {
            Console.Error.WriteLine($"Network error: {err.Message}");
            return null; // Backend optional - return null on network failure
        }
    }

    private bool DetectOutOfSync(BackendFactoryDataResponse serverData)
    {
        DateTime serverTime = serverData.LastSaved;
        DateTime? localTime = _state.GetLastEdit();
        
        if (localTime == null || localTime < serverTime)
        {
            Console.WriteLine("Server ahead - out of sync");
            return true;
        }
        
        Console.WriteLine("Local ahead - in sync");
        return false;
    }

    private void ReportError(Exception err)
    {
        Console.Error.WriteLine($"Sync error: {err.Message}");
        StopSync();
        _toast.ShowToast($"Sync error - disabled until refresh: {err.Message}", ToastType.Error);
    }

    private string BuildDateDisplay(DateTime dt)
    {
        string[] shortMonths = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", 
                                 "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
        return $"{dt.Day:D2}/{shortMonths[dt.Month - 1]}/{dt.Year} " +
               $"{dt.Hour:D2}:{dt.Minute:D2}:{dt.Second:D2}";
    }

    private void NotifyUpdate()
    {
        OnChange?.Invoke();
    }

    public void Dispose()
    {
        _ticker?.Dispose();
        _state.OnChange -= DetectedChange;
    }
}
