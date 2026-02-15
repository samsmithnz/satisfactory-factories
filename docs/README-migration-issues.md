# Blazor Migration Issue Templates

This directory contains templates and scripts for creating GitHub issues for the Vue to .NET Blazor migration.

## Files

- **`blazor-migration-issues.md`** - Detailed markdown document with all 19 issues, including descriptions, acceptance criteria, dependencies, and effort estimates.
- **`create-issues.sh`** - Bash script to automatically create all 19 issues using GitHub CLI.

## Option 1: Using the Automated Script (Recommended)

If you have GitHub CLI installed, you can create all 19 issues automatically:

### Prerequisites
1. Install GitHub CLI: https://cli.github.com/
2. Authenticate: `gh auth login`

### Run the Script
```bash
cd docs
./create-issues.sh
```

This will create all 19 issues in the repository with proper labels and formatting.

## Option 2: Manual Issue Creation

If you prefer to create issues manually or don't have GitHub CLI:

1. Open `blazor-migration-issues.md`
2. For each issue (1-19):
   - Copy the issue title
   - Copy the issue description
   - Create a new GitHub issue at: https://github.com/samsmithnz/satisfactory-factories/issues/new
   - Paste the title and description
   - Add the suggested labels
   - Submit

## Option 3: Import via GitHub API

You can also use the GitHub API or other tools to bulk-create issues from the markdown file.

## Issue Organization

The 19 issues are organized into 5 phases:

### Phase 1 - Foundation (Issues #1-3)
Infrastructure setup: .gitignore, Blazor project, MSTest projects

### Phase 2 - Core Infrastructure (Issues #4-8)
Parser, data models, styles, navigation, business logic

### Phase 3 - Pages (Issues #9-14)
All 6 page migrations (Index, Recipes, Changelog, Graph, Error, Share)

### Phase 4 - Features (Issues #15-16)
SaveLoader/localStorage and Sync functionality

### Phase 5 - Completion (Issues #17-19)
CI/CD, documentation, and acceptance testing

## Suggested Labels

Before creating issues, consider creating these labels in your repository:
- `migration`, `blazor`, `dotnet`, `infrastructure`, `core`, `pages`
- `components`, `business-logic`, `features`, `testing`, `documentation`
- `ui`, `data`, `parser`, `ci-cd`, `optional`

## Effort Estimates

- **Small (1-3 days):** 7 issues
- **Medium (3-7 days):** 6 issues
- **Large (7-14 days):** 6 issues
- **Total Timeline:** 12-16 weeks with 1-2 developers

## Dependencies

Issues are ordered with dependencies in mind. Refer to the "Dependencies" section in each issue to understand the prerequisite work.

## Questions?

For questions about the migration strategy or these issues, refer to the main issue that requested this work breakdown.
