# Satisfactory Factories
This [web tool](https://satisfactory-factories.app/) is designed to help players of the video game Satisfactory&trade; to plan a comprehensive production chain.

The tool highlights bottlenecks in the production chain, and visually tells the player that they have a problem within their designs.

The player can scale up end product factories as they see fit, and check if their production chain can handle the increased load.

## Contributing
Since this is an open source project, all PR requests will be welcomed, as long as proper intent and communication with the project maintainers is maintained.

___
## Local Development

### Technology Migration
This project is currently transitioning from **Vue 3 + TypeScript** to **.NET 10 Blazor WebAssembly**. Both technology stacks currently coexist in the repository:
- **Current (Vue 3)**: Production web application (`/web`), parser (`/parsing`), and backend API (`/backend`)
- **New (.NET 10)**: Blazor WebAssembly application (`/src/Web`), Parser console app (`/src/Parser`), and MSTest test projects

The .NET version is actively under development and will eventually replace the Vue implementation.

### Requirements

#### For .NET Development (New)
- **.NET 10 SDK or later** - [Download .NET SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
  - Verify installation: `dotnet --version` (should show 10.0.x or later)

#### For Vue Development (Current)
- **Node.js** version >20.17.0 `nvm install 20.17 && nvm use 20.17`
  - You may want to make 20.17 the default version with `nvm alias default 20.17`
  - We highly recommend you use `nvm` to manage your node versions
- **pnpm** version >9.14.4 `npm install -g pnpm`
- **Docker** (for the backend) - [Docker install docs](https://docs.docker.com/engine/install/)

### .NET Web Application (Blazor WebAssembly - New)
The new .NET 10 Blazor WebAssembly application is under active development and will eventually replace the Vue frontend.

#### Running the Blazor Web Application
```sh
cd src/Web
dotnet run
```

Visit http://localhost:5000 (or the URL shown in the console) to view the Blazor application.

#### Testing the Blazor Web Application
```sh
cd src/Web.Tests
dotnet test
```

All tests must pass for PRs to be accepted.

### Frontend (Vue 3 - Current Production)
The Vue 3 application is the current production version deployed at [satisfactory-factories.app](https://satisfactory-factories.app/).

```sh
cd web
pnpm install
pnpm dev
```

Visit http://localhost:3000 to view the project. You may need to load it twice.

#### Testing
There are tests for the frontend project, run them with `pnpm test`. Tests must pass for PRs to be accepted. Note as of writing the coverage isn't 100%.

### .NET Parser (Console Application - New)
The .NET parser is a console application that processes the `Docs.json` from the game and outputs a more readable format.

#### Running the .NET Parser
The parser requires two command-line arguments: an input file path and an output file path.

```sh
cd src/Parser
dotnet run -- path/to/Docs.json path/to/output/gameData.json
```

**Example:**
```sh
# Windows example (assuming Steam library at C:\Program Files (x86)\Steam)
cd src/Parser
dotnet run -- "C:\Program Files (x86)\Steam\steamapps\common\Satisfactory\CommunityResources\Docs\Docs.json" "../../web/public/gameData_v1.0-12.json"
```

The `Docs.json` file is located under `X\steamapps\common\Satisfactory\CommunityResources\Docs` on Windows. Replace X with where you have installed your steam library (usually `C:\Program Files (x86)\Steam`).

#### Testing the .NET Parser
```sh
cd src/Parser.Tests
dotnet test
```

All tests must pass for PRs to be accepted.

### Parsing (TypeScript - Current Production)
The TypeScript parser is responsible for processing the `Docs.json` from the game and reconstructing a more readable version for our use, since the game's docs file is overwhelmingly large and not very human-readable. The file is located under `X\steamapps\common\Satisfactory\CommunityResources\Docs` on Windows. Replace X with where you have installed your steam library (usually `C:\Program Files (x86)\Steam`)

#### Running the parser and updating the gameData
To run the parser:

```sh
cd parsing
pnpm install
pnpm dev
```

When the parser is run, it outputs to file `/parser/gameData.json`. This file needs copying to `/web/public/gameData_v1.x-xx.json`. The version must directly correlate with the minor version of the game (unless a patch messes with a recipe, unlikely). e.g. `v1.0-11.json` would increment to `v1.0-12.json`. The old version needs deleting when you commit.

Once the new file has been placed, you must also edit `/web/src/config/config.ts` and update the version there too.

This instructs web clients to re-download the game data file with the new version upon refresh and replace their locally stored version.

### Backend
Required for the login and syncing of data features, not required for local development.
1. Start Docker service on your local machine.
```sh
cd backend
pnpm install
./start.sh
```

API will be available on http://localhost:3001.

There are no tests currently available for the backend project.

### Building and Testing All .NET Projects
The repository includes a .NET solution file at `/src/SatisfactoryFactories.slnx` that contains all .NET projects (Web, Parser, and their test projects).

#### Build All .NET Projects
```sh
cd src
dotnet build
```

This will build:
- `Web` - Blazor WebAssembly application
- `Parser` - Console application for processing game data
- `Web.Tests` - MSTest tests for the Web application
- `Parser.Tests` - MSTest tests for the Parser

#### Run All .NET Tests
```sh
cd src
dotnet test
```

All tests must pass for PRs to be accepted.

#### Run Tests for a Specific Project
```sh
# Web tests
cd src/Web.Tests
dotnet test

# Parser tests
cd src/Parser.Tests
dotnet test
```

#### Run Tests with Coverage
```sh
cd src
dotnet test --collect:"XPlat Code Coverage"
```

### Deployment

#### Current Deployment (Vue 3)
New versions are trunked to `main` branch. Once `main` has been pushed, GitHub Actions will create a release then deploy the frontend to Vercel, and create a docker image of the backend to deploy to my personal server.

#### Future Deployment (.NET Blazor)
The .NET Blazor WebAssembly application is planned to be deployed to Azure Static Web Apps or Azure App Service once development is complete. This will replace the current Vercel deployment.
___

## License
This project is licensed under the GNU License - see the [LICENSE](LICENSE) file for details.

Please kindly consider opening PRs to improve the project, and make it better for everyone rather than making a clone.

## Acknowledgements
- Many thanks to [Greeny (creator of Satisfactory Tools)](https://github.com/greeny/SatisfactoryTools) for collating all the game assets required to display the various icons for items and buildings.
- Thanks to the author of [Satisfactory Logistics](https://satisfactory-logistics.xyz), who gave me the inspiration to extend what they did but even further.
