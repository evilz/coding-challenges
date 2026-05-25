# Hash Code CLI - Visual Demo

## Application Flow

```
┌─────────────────────────────────────────────────────────────┐
│                                                             │
│    _   _                 _          ____               _    │
│   | | | |   __ _   ___  | |__      / ___|   ___     __| |   │
│   | |_| |  / _` | / __| | '_ \    | |      / _ \   / _` |   │
│   |  _  | | (_| | \__ \ | | | |   | |___  | (_) | | (_| |   │
│   |_| |_|  \__,_| |___/ |_| |_|    \____|  \___/   \__,_|   │
│                                                             │
│            Google Hash Code Challenge Viewer                │
│       Interactive CLI to explore and run challenges         │
│                                                             │
└─────────────────────────────────────────────────────────────┘

           ┌──────────────────────────┐
           │    What would you like   │
           │         to do?           │
           ├──────────────────────────┤
           │ ▶ View all challenges    │
           │   by year                │
           │   Search challenges      │
           │   Exit                   │
           └──────────────────────────┘
                      │
                      ▼
           ┌──────────────────────────┐
           │   Select a year:         │
           ├──────────────────────────┤
           │ ▶ 2015                   │
           │   2014                   │
           │   Back to main menu      │
           └──────────────────────────┘
                      │
                      ▼
           ┌──────────────────────────────────────┐
           │       2015 Challenges:               │
           ├──────────────────────────────────────┤
           │ ▶ Optimize a Data Center             │
           │   (Qualification Round)              │
           │   Loon (Final Round)                 │
           │   Back                               │
           └──────────────────────────────────────┘
                      │
                      ▼
┌────────────────────────────────────────────────────────────┐
│                                                            │
│  ┌──────────────────────────────────────────────────────┐ │
│  │  Optimize a Data Center                              │ │
│  │  2015 - Qualification Round                          │ │
│  └──────────────────────────────────────────────────────┘ │
│                                                            │
│  Overview:                                                 │
│  Optimize server placement in data center to maximize      │
│  availability                                              │
│                                                            │
│  What would you like to do?                                │
│  ┌────────────────────────────────┐                        │
│  │ ▶ View full description        │                        │
│  │   View solution explanation    │                        │
│  │   Run solution                 │                        │
│  │   Back                         │                        │
│  └────────────────────────────────┘                        │
└────────────────────────────────────────────────────────────┘
```

## Key Features

### 🎨 Beautiful UI
- Colored text and borders
- ASCII art header
- Clean panels and formatting
- Professional appearance

### 🔍 Easy Navigation
- Arrow key navigation
- Intuitive menu structure
- Clear back buttons
- Search functionality

### 📚 Rich Content
- Full problem descriptions
- Solution explanations
- Code examples
- Challenge metadata

### 🚀 Runnable Solutions
- Execute on real input files
- See parsed data
- Instant feedback
- Error handling

## Technologies

| Component | Technology |
|-----------|-----------|
| Runtime | .NET 9.0 |
| Language | C# 13 |
| UI Library | Spectre.Console 0.52.0 |
| Features | Records, Top-level statements, Pattern matching |

## Usage Example

```bash
# Start the application
cd src/HashCodeCli
dotnet run

# Navigate using:
# - Arrow Keys: Move between options
# - Enter: Select option
# - Type: Search challenges
```

## Sample Output

When running a solution:

```
Parsed Input File:
- Data Center Rows: 2
- Slots per Row: 5
- Unavailable Slots: 1
- Pools: 2
- Servers: 5
- Total Capacity: 9 slots

Note: This is a simplified demonstration. The original solution used F# with
optimization algorithms. A full implementation would:
1. Parse data center layout and server specifications
2. Sort servers by capacity-to-size ratio
3. Place servers to balance pool capacity across rows
4. Ensure fault tolerance (capacity maintained if one row fails)
5. Output server placement (row, slot, pool) for each server
```
