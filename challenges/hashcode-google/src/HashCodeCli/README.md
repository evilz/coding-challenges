# Hash Code CLI

A .NET 9.0 console application to explore and run Google Hash Code challenges.

## Features

- **Browse challenges by year**: View all Hash Code challenges organized by year
- **Search challenges**: Search for specific challenges by name, description, or round
- **View challenge details**: See full problem descriptions and solution explanations
- **Run solutions**: Execute solutions on challenge input files (where available)
- **Interactive CLI**: Beautiful terminal UI powered by Spectre.Console

## Requirements

- .NET 9.0 SDK or later

## Building

```bash
cd src/HashCodeCli
dotnet build
```

## Running

```bash
cd src/HashCodeCli
dotnet run
```

Or after building:

```bash
cd src/HashCodeCli/bin/Debug/net9.0
./HashCodeCli
```

## Available Challenges

### 2014
- **Street View Routing** (Final Round) - Optimize Street View car routes to maximize city coverage

### 2015
- **Optimize a Data Center** (Qualification Round) - Optimize server placement to maximize availability
- **Loon** (Final Round) - Plan altitude adjustments for Internet-providing balloons

## Project Structure

```
HashCodeCli/
├── Models/
│   └── Challenge.cs          # Challenge data model
├── Solutions/
│   ├── StreetViewSolution.cs # 2014 Street View solution
│   └── DataCenterSolution.cs # 2015 Data Center solution
├── ChallengeRepository.cs     # Challenge data repository
└── Program.cs                 # Main CLI application
```

## Adding New Challenges

1. Add challenge data to `ChallengeRepository.cs`
2. If the challenge has a runnable solution, create a new solution class in the `Solutions/` directory
3. Link the solution runner in the Challenge record

## Technologies Used

- .NET 9.0
- C# 13
- Spectre.Console (for terminal UI)
