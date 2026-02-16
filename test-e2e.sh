#!/bin/bash

# End-to-End Acceptance Testing Script
# This script validates the Vue to Blazor migration

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Check for required dependencies
echo "Checking dependencies..."
MISSING_DEPS=0

if ! command -v pnpm &> /dev/null; then
    echo -e "${RED}ERROR: pnpm is not installed${NC}"
    echo "Install with: npm install -g pnpm"
    MISSING_DEPS=1
fi

if ! command -v dotnet &> /dev/null; then
    echo -e "${RED}ERROR: dotnet is not installed${NC}"
    echo "Install from: https://dotnet.microsoft.com/download"
    MISSING_DEPS=1
fi

if ! command -v jq &> /dev/null; then
    echo -e "${RED}ERROR: jq is not installed${NC}"
    echo "Install with: apt-get install jq (Ubuntu) or brew install jq (macOS)"
    MISSING_DEPS=1
fi

if [ $MISSING_DEPS -eq 1 ]; then
    echo -e "${RED}Missing required dependencies. Please install them and try again.${NC}"
    exit 1
fi

echo -e "${GREEN}All dependencies found${NC}"
echo ""

echo "======================================"
echo "E2E Acceptance Testing"
echo "======================================"
echo ""

# Test results
PASS_COUNT=0
FAIL_COUNT=0

print_pass() {
    echo -e "${GREEN}✅ PASS${NC}: $1"
    ((PASS_COUNT++))
}

print_fail() {
    echo -e "${RED}❌ FAIL${NC}: $1"
    ((FAIL_COUNT++))
}

print_warn() {
    echo -e "${YELLOW}⚠️  WARN${NC}: $1"
}

echo "## 1. Route Verification"
echo "Checking Blazor pages exist..."

ROUTES=(
    "Home.razor:/"
    "Recipes.razor:/recipes"
    "Changelog.razor:/changelog"
    "Graph.razor:/graph"
    "Error.razor:/error"
    "Share.razor:/share/{id}"
)

for route in "${ROUTES[@]}"; do
    IFS=':' read -r file path <<< "$route"
    if [ -f "src/Web/Pages/$file" ]; then
        if grep -q "@page \"$path\"" "src/Web/Pages/$file" 2>/dev/null || \
           grep -q "@page \"${path//\{id\}/\{id\}}\"" "src/Web/Pages/$file" 2>/dev/null; then
            print_pass "Route $path -> $file"
        else
            print_fail "Route $path not configured in $file"
        fi
    else
        print_fail "File $file not found"
    fi
done

echo ""
echo "## 2. Parser Testing"

# Test Vue Parser
echo "Testing Vue Parser..."
cd parsing
if pnpm test --silent >/dev/null 2>&1; then
    print_pass "Vue parser tests"
    
    # Get Vue parser output counts
    VUE_PARTS=$(jq '.items.parts | length' gameData.json)
    VUE_RECIPES=$(jq '.recipes | length' gameData.json)
    VUE_BUILDINGS=$(jq '.buildings | length' gameData.json)
    
    echo "  Vue Parser: $VUE_PARTS parts, $VUE_RECIPES recipes, $VUE_BUILDINGS buildings"
else
    print_fail "Vue parser tests"
fi
cd ..

# Test .NET Parser
echo "Testing .NET Parser..."
cd src
if dotnet test --filter "FullyQualifiedName~Parser.Tests" --no-build --verbosity quiet >/dev/null 2>&1; then
    print_pass ".NET parser tests"
else
    print_fail ".NET parser tests"
fi

# Get .NET parser output counts
if [ -f "Parser/dotnet-gameData.json" ]; then
    DOTNET_PARTS=$(jq '.items.parts | length' Parser/dotnet-gameData.json)
    DOTNET_RECIPES=$(jq '.recipes | length' Parser/dotnet-gameData.json)
    DOTNET_BUILDINGS=$(jq '.buildings | length' Parser/dotnet-gameData.json)
    
    echo "  .NET Parser: $DOTNET_PARTS parts, $DOTNET_RECIPES recipes, $DOTNET_BUILDINGS buildings"
    
    # Compare outputs
    if [ "$VUE_PARTS" -eq "$DOTNET_PARTS" ]; then
        print_pass "Parser part counts match"
    else
        print_fail "Parser part counts differ (Vue: $VUE_PARTS, .NET: $DOTNET_PARTS)"
    fi
    
    if [ "$VUE_RECIPES" -eq "$DOTNET_RECIPES" ]; then
        print_pass "Parser recipe counts match"
    else
        print_fail "Parser recipe counts differ (Vue: $VUE_RECIPES, .NET: $DOTNET_RECIPES)"
    fi
    
    if [ "$VUE_BUILDINGS" -eq "$DOTNET_BUILDINGS" ]; then
        print_pass "Parser building counts match"
    else
        print_fail "Parser building counts differ (Vue: $VUE_BUILDINGS, .NET: $DOTNET_BUILDINGS)"
    fi
else
    print_warn ".NET parser output file not found"
fi
cd ..

echo ""
echo "## 3. Component Testing"

# Test .NET Web
echo "Testing .NET Web..."
cd src
if dotnet test --filter "FullyQualifiedName~Web.Tests" --no-build --verbosity quiet >/dev/null 2>&1; then
    print_pass ".NET Web tests"
else
    print_fail ".NET Web tests"
fi
cd ..

# Test Vue Web
echo "Testing Vue Web..."
cd web
if pnpm test --silent >/dev/null 2>&1; then
    print_pass "Vue Web tests"
else
    print_fail "Vue Web tests"
fi
cd ..

echo ""
echo "======================================"
echo "Test Results Summary"
echo "======================================"
echo -e "${GREEN}Passed: $PASS_COUNT${NC}"
echo -e "${RED}Failed: $FAIL_COUNT${NC}"
echo ""

if [ $FAIL_COUNT -eq 0 ]; then
    echo -e "${GREEN}✅ ALL TESTS PASSED${NC}"
    exit 0
else
    echo -e "${RED}❌ SOME TESTS FAILED${NC}"
    echo ""
    echo "See test-acceptance.md for detailed report"
    exit 1
fi
