# Download Problems Script (C# Version)

This C# program downloads all Project Euler problem information from projecteuler.net and creates a consolidated `problems.md` file.

## Requirements

- .NET 10 SDK

## Usage

### Navigate to the DownloadProblems directory and run:

```bash
cd DownloadProblems
dotnet run
```

This will create a `problems.md` file in the DownloadProblems directory with all 969 problems.

### Download a Range of Problems

```bash
cd DownloadProblems
dotnet run -- --start 1 --end 100
```

### Test Mode (First 5 Problems Only)

```bash
cd DownloadProblems
dotnet run -- --test
```

### Custom Output File

```bash
cd DownloadProblems
dotnet run -- --output my_problems.md
```

### Custom Delay Between Requests

```bash
cd DownloadProblems
dotnet run -- --delay 1.0
```

### Command Line Arguments

- `--start N`: Starting problem number (default: 1)
- `--end N`: Ending problem number (default: 969)
- `--output FILE`: Output file name (default: problems.md)
- `--delay SECONDS`: Delay between requests in seconds (default: 0.5)
- `--test`: Test mode - only downloads problems 1-5
- `--help` or `-h`: Show help message

## How It Works

The program:

1. Fetches each problem page from `https://projecteuler.net/problem={ref}`
2. Extracts the title from the h2 tag
3. Fetches the minimal page from `https://projecteuler.net/minimal={ref}`
4. Extracts the raw HTML description from the pre tag
5. Writes everything to a markdown file

The program includes a 0.5 second delay between requests (configurable) to be respectful to the Project Euler server.

## Output Format

The generated `problems.md` file contains:

```markdown
# Project Euler Problems

Problems 1 to 969

---

## Problem 1: Multiples of 3 or 5

[Problem description in raw HTML]

---

## Problem 2: Even Fibonacci numbers

[Problem description in raw HTML]

---

...
```

## Example

```bash
# Download first 10 problems to a custom file
cd DownloadProblems
dotnet run -- --start 1 --end 10 --output first_10_problems.md
```

## Building

To build the project without running:

```bash
cd DownloadProblems
dotnet build
```

## Notes

- The program requires internet access to projecteuler.net
- Large ranges (all 969 problems) will take time due to the delay between requests
- If a problem cannot be fetched, it will be noted in the console output
- Error messages are printed to stderr for debugging
