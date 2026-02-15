# Blazor Migration Issues

This document contains 19 issues for migrating Satisfactory Factories from Vue 3 to .NET 10 Blazor.

---

## Issue 1: Add .gitignore rules for .NET artifacts

**Labels:** `infrastructure`, `dotnet`, `migration`

**Description:**
Add .NET-specific patterns to .gitignore to prevent committing build artifacts during Blazor migration.

**Details:**
Add standard .NET gitignore patterns:
- `bin/`
- `obj/`
- `*.user`
- `*.suo`
- `.vs/`
- `*.dll`
- `*.pdb`

Keep existing Node.js/Vue patterns and ensure no conflicts between Node and .NET patterns.

**Acceptance Criteria:**
- [ ] .gitignore updated with .NET patterns
- [ ] No .NET build artifacts committed to repository
- [ ] Existing Node.js/Vue patterns remain functional

**Dependencies:** None

---

## Issue 2: Set up .NET 10 Blazor Web App project structure

**Labels:** `infrastructure`, `dotnet`, `blazor`, `migration`

**Description:**
Create the initial .NET 10 Blazor Web App project structure with Blazor Client-Side (WebAssembly/WASM) hosting model in `/src/Web` directory.

**Details:**
- Create `/src/Web` directory
- Initialize Blazor Web App with .NET 10
- Configure for Client-Side (WebAssembly/WASM) hosting
- Set up project file with necessary packages
- Configure routing infrastructure
- Add basic App.razor and MainLayout.razor
- Set up wwwroot for static files
- No authentication/authorization setup required

**Acceptance Criteria:**
- [ ] `/src/Web` directory created with Blazor WASM project
- [ ] Project targets .NET 10
- [ ] Project runs successfully with `dotnet run`
- [ ] Basic routing works (home page loads)
- [ ] No authentication configured

**Dependencies:** Issue #1 (gitignore)

---

## Issue 3: Set up .NET 10 MSTest projects for Web and Parser

**Labels:** `infrastructure`, `testing`, `dotnet`, `migration`

**Description:**
Create MSTest test projects for both the Web application and Parser, following .NET testing conventions.

**Details:**
- Create `/src/Web.Tests` MSTest project targeting .NET 10
- Create `/src/Parser.Tests` MSTest project targeting .NET 10
- Add necessary MSTest NuGet packages:
  - MSTest.TestAdapter
  - MSTest.TestFramework
  - coverlet.collector
- Configure test discovery and execution
- Add sample smoke test to verify setup
- Document test running commands in README

**Current Test Context:**
- Vue app uses Vitest (413 tests)
- Parser uses Jest (23 tests with 94.7% coverage)

**Acceptance Criteria:**
- [ ] `/src/Web.Tests` project created and builds
- [ ] `/src/Parser.Tests` project created and builds
- [ ] `dotnet test` runs successfully on both projects
- [ ] Sample tests pass
- [ ] Test commands documented

**Dependencies:** Issue #2 (Blazor project structure)

---

## Issue 4: Create .NET Parser console application

**Labels:** `parser`, `dotnet`, `migration`, `core`

**Description:**
Migrate the TypeScript parser (`/parsing`) to a .NET 10 console application in `/src/Parser` that processes game-docs.json into gameData.json.

**Details:**
- Port `/parsing/src/processor.ts`, `parts.ts`, `recipes.ts`, `buildings.ts`, `common.ts` to C#
- Implement command-line argument handling for input/output files
- Port all interfaces from `/parsing/src/interfaces` to C# records/classes
- Ensure JSON output matches existing format exactly
- Accept game-docs.json as input, output gameData.json
- Maintain 90%+ test coverage requirement (currently 94.7%)
- Port all 23 existing Jest tests to MSTest
- Verify output matches byte-for-byte with TypeScript parser on sample inputs

**Acceptance Criteria:**
- [ ] Parser runs via `dotnet run` with input/output arguments
- [ ] Output gameData.json matches TypeScript parser output exactly
- [ ] All 23 tests ported to MSTest and passing
- [ ] Test coverage ≥90%
- [ ] Processes all parts, recipes, and buildings correctly

**Dependencies:** Issue #3 (MSTest projects)

---

## Issue 5: Create Blazor data models and JSON loading infrastructure

**Labels:** `data`, `blazor`, `migration`, `core`

**Description:**
Port TypeScript interfaces to C# models and implement game data loading mechanism in Blazor.

**Details:**
- Port all TypeScript interfaces from `/web/src/interfaces` to C# records/classes
- Create equivalent to `/web/src/stores/game-data-store.ts` using Blazor state management
- Implement JSON deserialization for gameData_v1.x-xx.json files
- Port `/web/src/config/config.ts` for version management
- Implement HttpClient-based loading for game data JSON
- Create service/singleton for game data access across components
- Ensure data structure matches TypeScript exactly for compatibility

**Acceptance Criteria:**
- [ ] All TypeScript interfaces ported to C# models
- [ ] Game data loads from JSON file successfully
- [ ] Data accessible across Blazor components
- [ ] Version management implemented
- [ ] Data structure matches Vue implementation

**Dependencies:** Issue #2 (Blazor project structure)

---

## Issue 6: Migrate CSS/SCSS styles to Blazor

**Labels:** `styles`, `ui`, `blazor`, `migration`

**Description:**
Port existing styles from `/web/src/assets/styles/global.scss` and component-specific styles to the Blazor application.

**Details:**
- Copy `/web/src/assets/styles/global.scss` to `/src/Web/wwwroot/css`
- Evaluate Vuetify dependency - determine if migration to MudBlazor (Material Design for Blazor) or keep CSS-only approach
- Extract component-specific styles from .vue files
- Configure SCSS compilation in Blazor project if needed, or convert to CSS
- Maintain Material Design look and feel
- Keep color scheme, fonts (Roboto), and layout patterns
- Ensure responsive design breakpoints are preserved

**Acceptance Criteria:**
- [ ] Global styles ported to Blazor
- [ ] Material Design look and feel maintained
- [ ] Responsive design works on mobile and desktop
- [ ] Fonts and colors match Vue version
- [ ] Component-specific styles extracted and applied

**Dependencies:** Issue #2 (Blazor project structure)

---

## Issue 7: Migrate Navigation and Layout components to Blazor

**Labels:** `components`, `ui`, `blazor`, `migration`

**Description:**
Port navigation, footer, and layout components to Blazor shared components.

**Details:**
- Port `/web/src/components/Navigation.vue` to Blazor NavMenu
- Port `/web/src/components/AppFooter.vue` to Blazor footer component
- Port `/web/src/components/TabNavigation.vue`
- Port layouts from `/web/src/layouts` to Blazor layouts
- Ensure responsive navigation for mobile/desktop
- Maintain menu structure and routing

**Acceptance Criteria:**
- [ ] Navigation component works and routes correctly
- [ ] Footer component displays properly
- [ ] TabNavigation ported and functional
- [ ] Layouts applied correctly to pages
- [ ] Responsive navigation works on mobile

**Dependencies:** Issue #2 (Blazor project), Issue #6 (Styles)

---

## Issue 8: Port factory management utilities and calculations to C#

**Labels:** `business-logic`, `dotnet`, `migration`, `core`

**Description:**
Port all factory calculation logic from `/web/src/utils/factory-management` to C# service classes.

**Details:**
Port all TypeScript utility files to C# services:
- factory.ts, products.ts, exports.ts, imports.ts, inputs.ts, parts.ts
- power.ts, satisfaction.ts, validation.ts, problems.ts, dependencies.ts
- exportCalculator.ts, syncState.ts, common.ts

Maintain exact calculation algorithms. Port all 11+ test files to MSTest with same coverage. Ensure numerical precision matches TypeScript implementation. Create service interfaces for dependency injection.

**Current Test Context:**
- 11 test files in `/web/src/utils/factory-management`
- Critical calculation logic for products, exports, imports, satisfaction, power

**Acceptance Criteria:**
- [ ] All utility files ported to C# services
- [ ] All tests ported to MSTest and passing
- [ ] Calculations produce identical results to TypeScript
- [ ] Services use dependency injection
- [ ] Test coverage maintained

**Dependencies:** Issue #5 (Data models)

---

## Issue 9: Migrate Index/Home page to Blazor

**Labels:** `pages`, `blazor`, `migration`, `planner`

**Description:**
Migrate `/web/src/pages/index.vue` (home/planner page) to Blazor `Index.razor`.

**Details:**
- Create `Pages/Index.razor` in Blazor project
- Port Loading, SaveLoader, Splash, and Planner components
- Port `/web/src/components/planner/Planner.vue` and all child components
- Implement factory management logic from `/web/src/utils/factory-management`
- Port Pinia stores: app-store.ts, game-data-store.ts, sync-store.ts
- Ensure all product/import/satisfaction calculations work
- Port associated tests from .spec.ts files
- Route: `/` (default)

**Components to Port:**
- Loading
- SaveLoader  
- Splash
- Planner (and all child components in `/web/src/components/planner/`)

**Acceptance Criteria:**
- [ ] Index page accessible at `/` route
- [ ] All planner components ported and functional
- [ ] Factory calculations work correctly
- [ ] State management implemented
- [ ] Tests ported and passing

**Dependencies:** Issue #5 (Data models), Issue #7 (Navigation), Issue #8 (Business logic)

---

## Issue 10: Migrate Recipes page to Blazor

**Labels:** `pages`, `blazor`, `migration`

**Description:**
Migrate `/web/src/pages/recipes.vue` to Blazor `Recipes.razor`.

**Details:**
- Create `Pages/Recipes.razor` in Blazor project
- Port `/web/src/components/recipes/Recipes.vue` component
- Display game data recipes using data models from Issue #5
- Maintain filtering and search functionality
- Port any recipe-specific utilities
- Route: `/recipes`

**Acceptance Criteria:**
- [ ] Recipes page accessible at `/recipes` route
- [ ] Recipes display correctly from game data
- [ ] Filtering and search work
- [ ] Visually matches Vue version

**Dependencies:** Issue #5 (Data models), Issue #7 (Navigation)

---

## Issue 11: Migrate Changelog page to Blazor

**Labels:** `pages`, `blazor`, `migration`, `documentation`

**Description:**
Migrate `/web/src/pages/changelog.vue` to Blazor `Changelog.razor`.

**Details:**
- Create `Pages/Changelog.razor` in Blazor project
- Port changelog HTML content
- Embed YouTube iframe for video content
- Port Introduction component if used
- Maintain responsive layout for images
- Route: `/changelog`

**Acceptance Criteria:**
- [ ] Changelog page accessible at `/changelog` route
- [ ] All content displays correctly
- [ ] YouTube videos embedded properly
- [ ] Images display with responsive layout
- [ ] Matches Vue version visually

**Dependencies:** Issue #7 (Navigation)

---

## Issue 12: Migrate Graph/Visualization page to Blazor

**Labels:** `pages`, `blazor`, `migration`, `visualization`

**Description:**
Migrate `/web/src/pages/graph.vue` and graph visualization components to Blazor.

**Details:**
- Create `Pages/Graph.razor` in Blazor project
- Evaluate .NET graph visualization options (e.g., Blazor.Diagrams, or JavaScript interop with existing libraries)
- Port `/web/src/components/graph/Graph.vue`, `FactoryNode.vue`, `Todo.vue`
- Implement dagre layout algorithm or find .NET equivalent
- Maintain interactive graph features (zoom, pan, node dragging)
- Port graph-related utilities and state management
- Route: `/graph`

**Current Dependencies:**
- @vue-flow packages
- @dagrejs/dagre

**Acceptance Criteria:**
- [ ] Graph page accessible at `/graph` route
- [ ] Graph visualization displays factory dependencies
- [ ] Interactive features work (zoom, pan, drag)
- [ ] Layout algorithm produces similar results
- [ ] Performance is acceptable

**Dependencies:** Issue #5 (Data models), Issue #7 (Navigation), Issue #8 (Business logic)

---

## Issue 13: Migrate Error page to Blazor

**Labels:** `pages`, `blazor`, `migration`

**Description:**
Migrate `/web/src/pages/error.vue` to Blazor `Error.razor`.

**Details:**
- Create `Pages/Error.razor` in Blazor project
- Implement error display layout
- Configure error handling in Blazor router
- Route: `/error`

**Acceptance Criteria:**
- [ ] Error page accessible at `/error` route
- [ ] Error handling configured in router
- [ ] Displays errors appropriately
- [ ] Matches Vue version visually

**Dependencies:** Issue #7 (Navigation)

---

## Issue 14: Migrate Share page to Blazor

**Labels:** `pages`, `blazor`, `migration`, `sharing`

**Description:**
Migrate `/web/src/pages/share/[id].vue` (dynamic route for sharing) to Blazor.

**Details:**
- Create `Pages/Share.razor` with route parameter for ID
- Port share functionality and ShareButton component
- Implement URL parameter handling in Blazor
- Maintain share link generation and loading
- Route: `/share/{id}`

**Acceptance Criteria:**
- [ ] Share page accessible at `/share/{id}` route
- [ ] URL parameter parsed correctly
- [ ] Share functionality works (generate and load)
- [ ] ShareButton component ported
- [ ] Shared plans load correctly

**Dependencies:** Issue #5 (Data models), Issue #7 (Navigation), Issue #8 (Business logic)

---

## Issue 15: Migrate SaveLoader and local storage functionality to Blazor

**Labels:** `features`, `blazor`, `migration`, `storage`

**Description:**
Port save/load functionality from `/web/src/components/SaveLoader.vue` to Blazor using localStorage interop.

**Details:**
- Port SaveLoader component to Blazor
- Implement JavaScript interop for localStorage access
- Port Templates component for demo plan loading
- Maintain JSON serialization/deserialization of factory data
- Ensure backward compatibility with existing saved plans
- Add tests for save/load functionality

**Acceptance Criteria:**
- [ ] SaveLoader component ported
- [ ] localStorage interop works
- [ ] Can save factory plans
- [ ] Can load factory plans
- [ ] Demo plan loads successfully
- [ ] Backward compatible with existing saves
- [ ] Tests passing

**Dependencies:** Issue #9 (Index page)

---

## Issue 16: Implement Sync functionality for backend integration

**Labels:** `features`, `blazor`, `migration`, `backend`, `optional`

**Description:**
Port `/web/src/components/Sync.vue` and sync-store to Blazor for backend data synchronization (optional feature).

**Details:**
- Port Sync.vue component to Blazor
- Port sync-store.ts logic to C# service
- Implement HttpClient calls to backend API
- Handle authentication state (though auth not required per requirements)
- Make sync opt-in/optional
- Can work without backend for local development
- Port sync-related tests

**Note:** Per README: "Required for the login and syncing of data features, not required for local development."

**Acceptance Criteria:**
- [ ] Sync component ported
- [ ] Sync service implemented
- [ ] Works with backend API
- [ ] Optional/opt-in functionality
- [ ] Works without backend for local dev
- [ ] Tests passing

**Dependencies:** Issue #9 (Index page)

---

## Issue 17: Create CI/CD GitHub Actions workflow for .NET projects

**Labels:** `infrastructure`, `ci-cd`, `github-actions`, `dotnet`

**Description:**
Add GitHub Actions workflows to build and test the new .NET Parser and Web projects.

**Details:**

Create `.github/workflows/build-dotnet-parser.yml`:
- Build Parser project
- Run Parser MSTest tests
- Verify gameData.json output

Create `.github/workflows/build-dotnet-web.yml`:
- Build Blazor Web project
- Run Web MSTest tests
- Publish WASM output

Configure .NET 10 SDK in workflows. Maintain existing Vue workflows during transition. Add status badges to README.

**Acceptance Criteria:**
- [ ] Parser workflow created and passing
- [ ] Web workflow created and passing
- [ ] Both workflows run on PR and push
- [ ] Existing Vue workflows still functional
- [ ] Status badges added to README

**Dependencies:** Issue #4 (Parser), Issue #9 (Index page - represents functional Web project)

---

## Issue 18: Update README with .NET development instructions

**Labels:** `documentation`, `readme`, `dotnet`

**Description:**
Update README.md with .NET 10 development, build, and test instructions while keeping Vue documentation for transition period.

**Details:**
- Add .NET 10 SDK requirement (replaces Node.js requirement for new code)
- Document `dotnet run` commands for Parser and Web
- Document `dotnet test` for running MSTest tests
- Add section explaining migration strategy (Vue code coexists)
- Update parser section with .NET console app instructions
- Document how to run both Vue (current) and Blazor (new) versions
- Keep existing Vue documentation during transition
- Update deployment section for future Azure Web App hosting

**Acceptance Criteria:**
- [ ] .NET 10 SDK requirement documented
- [ ] `dotnet run` commands for Parser and Web documented
- [ ] `dotnet test` commands documented
- [ ] Migration strategy explained
- [ ] Both Vue and Blazor instructions present
- [ ] Clear and easy to follow

**Dependencies:** Issue #4 (Parser), Issue #9 (Index page - represents functional Web project)

---

## Issue 19: End-to-end acceptance testing for migration completion

**Labels:** `testing`, `acceptance`, `migration`, `validation`

**Description:**
Perform end-to-end acceptance testing to verify all Vue routes have equivalent Blazor pages and parser produces identical output.

**Details:**

**Verify Routes:**
- `/` → Index.razor (Planner with all components)
- `/recipes` → Recipes.razor (Recipe browser)
- `/changelog` → Changelog.razor (Release notes)
- `/graph` → Graph.razor (Dependency visualization)
- `/error` → Error.razor (Error handling)
- `/share/{id}` → Share.razor (Shared factory plans)

**Verify Parser:**
- Run Vue parser: `cd parsing && pnpm dev`
- Run .NET parser: `cd src/Parser && dotnet run game-docs.json gameData.json`
- Compare output files byte-for-byte or structurally
- Test with multiple sample game-docs.json inputs
- Verify all items, recipes, buildings parsed correctly

**Manual Testing:**
- Load demo plan in Blazor version
- Create new factory
- Add products and imports
- Verify calculations match Vue version
- Test save/load functionality
- Test all navigation links
- Verify responsive design on mobile/desktop
- Test graph visualization
- Compare visual appearance to Vue version

**Acceptance Criteria:**
- [ ] All 6 routes functional in Blazor
- [ ] Parser output matches Vue parser exactly
- [ ] Demo plan loads and works
- [ ] All calculations produce same results
- [ ] Save/load functionality works
- [ ] Navigation works correctly
- [ ] Responsive design verified
- [ ] Graph visualization works
- [ ] Visual appearance matches Vue

**Dependencies:** Issues #4-16 (all implementation issues)

---

## Migration Order Recommendation

1. **Phase 1 - Foundation (Issues #1-3)**
   - Issue #1: .gitignore
   - Issue #2: Blazor project structure
   - Issue #3: MSTest projects

2. **Phase 2 - Core Infrastructure (Issues #4-8)**
   - Issue #4: Parser console app
   - Issue #5: Data models
   - Issue #6: Styles
   - Issue #7: Navigation/Layouts
   - Issue #8: Business logic utilities

3. **Phase 3 - Pages (Issues #9-14)**
   - Issue #9: Index/Home page (most complex, core functionality)
   - Issue #10: Recipes page
   - Issue #11: Changelog page
   - Issue #12: Graph page
   - Issue #13: Error page
   - Issue #14: Share page

4. **Phase 4 - Features (Issues #15-16)**
   - Issue #15: SaveLoader/localStorage
   - Issue #16: Sync functionality

5. **Phase 5 - Completion (Issues #17-19)**
   - Issue #17: CI/CD workflows
   - Issue #18: README updates
   - Issue #19: E2E acceptance testing

## Labels to Create

Suggested labels for organizing these issues:
- `migration` - All migration-related issues
- `blazor` - Blazor-specific work
- `dotnet` - .NET-specific work
- `infrastructure` - Project setup and tooling
- `core` - Core functionality
- `pages` - Page migration work
- `components` - Component migration
- `business-logic` - Calculation and utility logic
- `features` - Feature implementation
- `testing` - Testing-related work
- `documentation` - Documentation updates
- `ui` - UI/styling work
- `data` - Data models and loading
- `parser` - Parser-specific work
- `ci-cd` - CI/CD workflows
- `optional` - Optional features

## Estimated Effort

- **Small (1-3 days):** Issues #1, #3, #6, #7, #10, #11, #13
- **Medium (3-7 days):** Issues #2, #5, #14, #15, #17, #18
- **Large (7-14 days):** Issues #4, #8, #9, #12, #16, #19

**Total Estimated Timeline:** 12-16 weeks with 1-2 developers working concurrently on independent issues.
