# Hash Code CLI - Usage Guide

## Overview

The Hash Code CLI is an interactive terminal application that allows you to explore Google Hash Code challenges from different years, view detailed problem descriptions, understand solution approaches, and run solutions on the provided input files.

## Getting Started

### Prerequisites
- .NET 9.0 SDK (or later)
- Terminal with Unicode support (for best visual experience)

### Running the Application

1. Navigate to the CLI directory:
   ```bash
   cd src/HashCodeCli
   ```

2. Run the application:
   ```bash
   dotnet run
   ```

3. Or build and run the executable:
   ```bash
   dotnet build
   cd bin/Debug/net9.0
   ./HashCodeCli
   ```

## Features

### 1. Browse Challenges by Year

From the main menu, select "View all challenges by year" to:
- See all available years (2014, 2015, etc.)
- Select a year to view all challenges from that year
- Each challenge shows its name and round (Qualification/Final)

**Example Navigation:**
```
Main Menu → View all challenges by year → 2015 → Optimize a Data Center
```

### 2. Search for Challenges

Select "Search challenges" from the main menu to:
- Enter a search term (e.g., "data center", "street", "loon")
- See all matching challenges
- Select one to view details

**Search matches against:**
- Challenge name
- Short description
- Round name

### 3. View Challenge Details

For each challenge, you can:

#### View Full Description
- Complete problem statement
- Input format details
- Output requirements
- Scoring information

#### View Solution Explanation
- Solution approach and strategy
- Key algorithms used
- Code concepts demonstrated
- Implementation highlights

#### Run Solution
- Execute the solution on the challenge's input file
- See parsed input statistics
- View solution output

## Available Challenges

### 2014 - Final Round

**Street View Routing**
- **Problem:** Optimize routes for Street View cars to maximize city coverage
- **Input:** Paris dataset with 54,000 junctions
- **Solution:** Graph algorithms with greedy route optimization
- **Status:** ✅ Runnable

### 2015 - Qualification Round

**Optimize a Data Center**
- **Problem:** Optimize server placement to maximize data center availability
- **Input:** Data center layout with rows, slots, and servers
- **Solution:** Greedy placement with pool balancing
- **Status:** ✅ Runnable

### 2015 - Final Round

**Loon**
- **Problem:** Plan altitude adjustments for Internet-providing balloons
- **Input:** Wind data and target coverage locations
- **Solution:** Not yet implemented
- **Status:** ⚠️ Description only

## Navigation Tips

- Use **arrow keys** to navigate menu options
- Press **Enter** to select an option
- Select "Back" to return to the previous menu
- Select "Exit" from the main menu to quit

## Technical Details

### Architecture

```
HashCodeCli/
├── Models/
│   └── Challenge.cs          # Data model for challenges
├── Solutions/
│   ├── StreetViewSolution.cs # 2014 solution runner
│   └── DataCenterSolution.cs # 2015 solution runner
├── ChallengeRepository.cs     # Challenge database
└── Program.cs                 # Main CLI application
```

### Technologies Used

- **.NET 9.0** - Latest .NET runtime
- **C# 13** - Latest C# language features
- **Spectre.Console** - Beautiful terminal UI library
  - Interactive prompts
  - Colored output
  - Panels and formatting
  - Spinner animations

### How Solutions Work

Each solution runner:
1. Validates the input file exists
2. Parses the input file format
3. Processes the challenge data
4. Returns formatted output with results

For demonstration purposes, the current solutions parse and display input file statistics. Full algorithmic solutions from the original F# code can be ported as needed.

## Extending the Application

### Adding a New Challenge

1. Open `ChallengeRepository.cs`
2. Add a new `Challenge` record to the `GetAllChallenges()` method:

```csharp
new Challenge(
    Year: 2016,
    Round: "Qualification Round",
    Name: "Your Challenge Name",
    ShortDescription: "Brief one-line description",
    FullDescription: @"Complete problem description...",
    SolutionExplanation: @"Solution approach...",
    InputFilePath: "path/to/input.txt",
    SolutionRunner: YourSolution.Run
)
```

3. If the challenge has a runnable solution, create a solution class in `Solutions/`:

```csharp
namespace HashCodeCli.Solutions;

public static class YourSolution
{
    public static string Run(string inputFilePath)
    {
        // Implementation
        return "Output";
    }
}
```

## Troubleshooting

### "Cannot show selection prompt since the current terminal isn't interactive"

This error occurs when running in a non-interactive environment (CI/CD, pipes, etc.). The CLI requires an interactive terminal with TTY support.

**Solution:** Run directly in a terminal, not through pipes or automation tools.

### Input file not found

If a solution runner reports "Input file not found":
1. Verify the input file exists in the repository
2. Check the path in `ChallengeRepository.cs`
3. Ensure the path is relative to the repository root

### Build errors

If you encounter build errors:
```bash
cd src/HashCodeCli
dotnet clean
dotnet restore
dotnet build
```

## Contributing

To contribute new challenges or improve solutions:

1. Add challenge data to the repository
2. Update `ChallengeRepository.cs` with challenge details
3. Implement solution runners in the `Solutions/` directory
4. Test thoroughly before submitting

## License

This project follows the repository's MIT License. See the main LICENSE file for details.
