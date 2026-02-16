# Sync Component Integration Guide

## Overview

The Sync component provides optional backend synchronization for factory data. It allows users to:
- Automatically sync factory data to a backend server every 10 seconds
- Detect and resolve out-of-sync conflicts between local and server data
- Force download server data to override local changes
- Gracefully handle backend unavailability

## Components

### Models
- **BackendFactoryDataResponse** (`Web/Models/Sync/BackendFactoryDataResponse.cs`)
  - Response model from backend `/load` endpoint
  - Contains: `User`, `Data` (List<Factory>), `LastSaved`

### Services
- **ISyncService** (`Web/Services/ISyncService.cs`)
  - Interface defining sync operations
  - Properties: `DataSavePending`, `DataLastSaved`, `StopSyncing`, `IsSyncing`
  - Methods: `SetupTick()`, `StopSync()`, `DetectedChange()`, `HandleDataLoadAsync()`, `HandleSyncAsync()`

- **SyncService** (`Web/Services/SyncService.cs`)
  - Implementation of sync functionality
  - Uses Timer for 10-second interval automatic sync
  - Gracefully handles network errors (backend optional)
  - Implements out-of-sync detection via timestamp comparison

### UI Component
- **Sync.razor** (`Web/Shared/Sync.razor`)
  - Displays last sync status
  - Shows syncing indicator
  - Force download button
  - Out-of-sync conflict resolution dialog

## Usage

### Registration

The SyncService is already registered in `Program.cs`:

```csharp
builder.Services.AddScoped<ISyncService, SyncService>();
```

### Integration in Blazor Components

To use the Sync component in your Razor page:

```razor
@using Web.Shared
@inject ISyncService SyncService

<Sync />

@code {
    protected override void OnInitialized()
    {
        // Optionally start the automatic sync tick
        SyncService.SetupTick();
    }
}
```

### Backend API Endpoints

The sync service expects these backend endpoints:

**POST /save**
- Request body: `List<Factory>` (JSON)
- Response: JSON object (e.g., `{"status": "ok"}`)
- Headers: `Authorization: Bearer {token}` (if auth enabled)

**GET /load**
- Response: `BackendFactoryDataResponse` JSON
  ```json
  {
    "user": "username",
    "data": [ /* Factory objects */ ],
    "lastSaved": "2024-01-01T12:00:00Z"
  }
  ```
- Headers: `Authorization: Bearer {token}` (if auth enabled)

### Configuration

API URL is configured in `ConfigurationService.cs`:
- Development: `http://localhost:3001`
- Production: `https://api.satisfactory-factories.app`

## Features

### Automatic Sync
- Triggers every 10 seconds when `SetupTick()` is called
- Only syncs if data changes detected (`DataSavePending = true`)
- Stops on error and shows toast notification

### Out-of-Sync Detection
- Compares server's `lastSaved` with local `GetLastEdit()` timestamp
- Shows dialog if server data is newer than local
- Options:
  - Use local data (overwrite server)
  - Use server data (overwrite local)

### Error Handling
- Network errors return `null` (backend optional)
- Server 5xx errors throw exception and stop sync
- Shows toast notifications for user feedback
- Sync can be manually stopped/restarted

## Testing

Unit tests are in `Web.Tests/Services/SyncServiceTests.cs`:
- 11 comprehensive tests covering:
  - Initial state validation
  - Change detection
  - Sync operations
  - Out-of-sync detection
  - Error handling
  - Force load/sync

Run tests:
```bash
cd src
dotnet test
```

## Future Integration

The Vue implementation embeds Sync inside the Auth component (`Auth.vue` line 112). When the Auth component is ported to Blazor, the Sync component should be integrated there. For now, it can be used standalone in any Razor component.

## Dependencies

- IAppStateService - For factory data access and change detection
- IConfigurationService - For API URL configuration
- IToastService - For user notifications
- IJSRuntime - For localStorage access
- HttpClient - For API requests
