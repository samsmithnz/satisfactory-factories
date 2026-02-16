# Index Page Migration Status

## Completed Work

### Core Infrastructure ✅
1. **AppStateService** - State management service replacing Pinia app-store.ts
   - Factory list management with add/remove/update operations
   - Local storage persistence for factories
   - Event-based state change notifications
   - Service registered in DI container

2. **Index Page (Home.razor)** - Basic planner page at "/" route
   - Two-pane layout (sidebar + main content)
   - Factory creation with auto-incrementing IDs
   - Factory deletion
   - Clear all factories
   - Welcome screen for empty state
   - Factory cards showing ID, products count, inputs count
   - Responsive design with desktop sidebar

3. **CSS Styling** - Updated components.css
   - Planner page layout styles
   - Factory card styles
   - Button styles (primary, secondary, danger)
   - Sidebar styles
   - Responsive utilities

### Testing ✅
- Manual testing completed
- Factory CRUD operations verified
- Local storage persistence verified
- UI rendering verified with screenshots

## Remaining Work

### High Priority - Core Planner Components

#### 1. Loading Components
- **Loading.vue** → **Loading.razor**
  - Overlay with progress bar
  - Factory loading states
  - Random loading messages
  - Event-based show/hide

- **SaveLoader.vue** → **SaveLoader.razor**
  - Save/load progress dialog
  - Step-based progress tracking
  - Loading message display

- **Splash.vue** → **Splash.razor**
  - Welcome/intro dialog
  - YouTube video embed
  - Local storage for "seen" state

#### 2. Main Planner Structure
- **Planner.vue** → **Planner.razor** (Main Container)
  - Factory list management
  - Global actions sidebar
  - Statistics display
  - World import/export functionality
  - Help text toggle
  - Navigation teleport (factory list to drawer)

#### 3. Factory List & Actions
- **PlannerFactoryList.vue** → **PlannerFactoryList.razor**
  - Draggable factory list
  - Factory visibility toggle
  - Factory navigation
  - Create factory button

- **PlannerGlobalActions.vue** → **PlannerGlobalActions.razor**
  - Clear all button
  - Show/hide all buttons
  - Import world button
  - Help text toggle

#### 4. Factory Display Components
- **PlannerFactory.vue** → **PlannerFactory.razor**
  - Main factory card
  - Products section
  - Imports section
  - Power section
  - Satisfaction section
  - Notes section
  - Tasks section
  - Factory actions (copy, delete, move, hide)

- **PlannerFactoryNotes.vue** → **PlannerFactoryNotes.razor**
  - Factory notes editor
  - Markdown support (optional)

- **PlannerFactoryTasks.vue** → **PlannerFactoryTasks.razor**
  - Task list display
  - Task CRUD operations

- **PlannerFactorySatisfaction.vue** → **PlannerFactorySatisfaction.razor**
  - Unsatisfied parts display
  - Fix suggestions
  - Export calculator

#### 5. Products & Power Components
- **Products subfolder components**
  - Product.vue → Product.razor
  - ProductRecipe.vue → ProductRecipe.razor
  - ProductAmount.vue → ProductAmount.razor
  - ProductByProducts.vue → ProductByProducts.razor

- **PowerProducer.vue** → **PowerProducer.razor**
  - Power generation configuration
  - Fuel selection
  - Building amount

#### 6. Import Components
- **Imports subfolder components**
  - Import.vue → Import.razor
  - ImportFactory.vue → ImportFactory.razor
  - ImportItem.vue → ImportItem.razor

#### 7. Satisfaction Components
- **Satisfaction subfolder components**
  - SatisfactionItem.vue → SatisfactionItem.razor
  - SatisfactionBuildings.vue → SatisfactionBuildings.razor
  - SatisfactionItems.vue → SatisfactionItems.razor

#### 8. Statistics Components
- **Statistics.vue** → **Statistics.razor** (Summary)
- **StatisticsFactorySummary.vue** → **StatisticsFactorySummary.razor**
- **StatisticsBuildings.vue** → **StatisticsBuildings.razor**
- **StatisticsItems.vue** → **StatisticsItems.razor**
- **StatisticsItemsDifference.vue** → **StatisticsItemsDifference.razor**
- **StatisticsPower.vue** → **StatisticsPower.razor**
- **StatisticsResources.vue** → **StatisticsResources.razor**

#### 9. Supporting Components
- **Introduction.vue** → **Introduction.razor**
- **Notice.vue** → **Notice.razor**
- **WorldData.vue** → **WorldData.razor**
- **WorldImport.vue** → **WorldImport.razor**
- **PlannerTooManyFactoriesOpen.vue** → **PlannerTooManyFactoriesOpen.razor**
- **Placeholder components** for empty states

### Business Logic Services (Critical Dependency)

These services from `/web/src/utils/factory-management/` need to be ported:

1. **FactoryService** (factory.ts)
   - `calculateFactory()` - Calculate single factory
   - `calculateFactories()` - Calculate all factories
   - `newFactory()` - Create new factory
   - `findFac()` - Find factory by ID
   - `reorderFactory()` - Change factory order
   - `regenerateSortOrders()` - Fix display order

2. **ProductService** (products.ts) - 369 lines, COMPLEX
   - Product calculations
   - Byproduct calculations
   - Amount adjustments based on demand
   - Recipe-based calculations

3. **PartService** (parts.ts)
   - Part metrics aggregation
   - Satisfaction calculations
   - Supply/demand tracking
   - Raw resource tracking

4. **BuildingService** (buildings.ts)
   - Building count calculations
   - Power consumption per recipe
   - Clock speed adjustments

5. **PowerService** (power.ts)
   - Power generation calculations
   - Fuel consumption
   - Power producer calculations

6. **DependencyService** (dependencies.ts)
   - Inter-factory dependencies
   - Dependency metrics
   - Supply/request tracking
   - Dependency removal

7. **ValidationService** (validation.ts)
   - Data integrity checks
   - Invalid input removal
   - Recipe validation

8. **InputService** (inputs.ts)
   - Factory input management
   - Inter-factory connections
   - Input removal

9. **ExportService** (exports.ts)
   - Export request management
   - Exportable factory identification

10. **SatisfactionService** (satisfaction.ts)
    - Unsatisfied part detection
    - Fix suggestions

11. **ProblemService** (problems.ts)
    - Problem detection
    - Factory flagging

12. **ExportCalculatorService** (exportCalculator.ts)
    - Export value calculations
    - Transportation metrics

13. **SyncStateService** (syncState.ts)
    - State synchronization
    - Change detection

### State Management

Currently implemented:
- ✅ AppStateService (replaces app-store.ts)
- ✅ GameDataService (replaces game-data-store.ts)

Still needed:
- [ ] SyncStateService (replaces sync-store.ts) - requires backend integration
- [ ] AuthStateService (replaces auth-store.ts) - for future auth features

### Testing

#### Component Tests Needed
- AppStateService unit tests
- Index page component tests
- All ported Razor component tests

#### Integration Tests Needed
- Factory CRUD flow
- Calculation accuracy tests
- Local storage persistence tests
- State synchronization tests

### Additional Work

#### Event Bus Replacement
The Vue implementation uses an event bus (`eventBus.ts`) for component communication:
- `prepareForLoad` - Loading initialization
- `incrementLoad` - Loading progress
- `loadingCompleted` - Loading done
- `plannerShow` - Show/hide planner
- `navigationReady` - Navigation ready
- `worldDataShow` - World data dialog
- `readyForData` - Ready to load data

**Blazor approach:** Use state management services and EventCallback parameters instead.

#### Teleport Functionality
Vue's `<Teleport>` is used to move factory list to navigation drawer on mobile.

**Blazor approach:** Use conditional rendering and CSS for responsive layouts.

#### Local Storage
Currently using IJSInterop for localStorage operations.
Consider creating a LocalStorageService abstraction.

## Implementation Strategy

### Phase 1: Foundation (COMPLETED ✅)
- AppStateService
- Basic Index page
- CSS styling

### Phase 2: Core Services (NEXT - HIGH PRIORITY)
These are blockers for most components:
1. FactoryService (factory CRUD and orchestration)
2. ProductService (complex calculations)
3. PartService (satisfaction tracking)
4. BuildingService (building requirements)
5. PowerService (power calculations)

**Estimate:** 2-3 work sessions

### Phase 3: Core Components
After services are ready:
1. Loading, SaveLoader, Splash
2. Planner main container
3. PlannerFactory (main card)
4. Product components
5. Statistics components

**Estimate:** 2-3 work sessions

### Phase 4: Advanced Components
1. Satisfaction components
2. Import/Export components
3. World data components
4. Tasks and Notes

**Estimate:** 1-2 work sessions

### Phase 5: Polish & Testing
1. Component tests
2. Integration tests
3. UI polish
4. Bug fixes
5. Performance optimization

**Estimate:** 1-2 work sessions

## Total Estimate
**7-11 work sessions** to complete full Index page migration with all features.

## Current Status Summary
- **Completed:** ~10% of total work
- **Foundation:** Solid (AppStateService, basic Index page)
- **Blockers:** Business logic services (ProductService, PartService, etc.)
- **Next Steps:** Implement core calculation services or create minimal stub services

## Recommendation

Given the complexity, consider one of these approaches:

### Option A: Incremental (Recommended)
1. Implement calculation services one at a time
2. Add components as services become available
3. Test thoroughly at each step
4. Ship incrementally

### Option B: Stub Services
1. Create minimal stub services for calculations
2. Implement all UI components with placeholders
3. Back-fill real calculations later
4. Get UI working faster but calculations won't be accurate

### Option C: Parallel Tracks
1. One developer on services
2. Another on components
3. Integrate as pieces complete
4. Requires coordination

**Chosen approach for this PR:** Option A - Start with minimal implementation, propose remaining work as separate issues.
