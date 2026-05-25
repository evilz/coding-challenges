using HashCodeCli.Models;

namespace HashCodeCli;

public static class ChallengeRepository
{
    public static List<Challenge> GetAllChallenges()
    {
        return new List<Challenge>
        {
            new Challenge(
                Year: 2014,
                Round: "Final Round",
                Name: "Street View Routing",
                ShortDescription: "Optimize Street View car routes to cover maximum city streets",
                FullDescription: @"The Street View imagery available in Google Maps is captured using specialized vehicles called Street View cars. These cars carry multiple cameras capturing pictures as the car moves around a city. Capturing the imagery of a city poses an optimization problem: the fleet of cars is available for a limited amount of time and we want to cover as much of the city streets as possible.

Problem:
Given a city map with junctions and streets, determine the optimal routes for Street View cars to maximize coverage within a time limit. Each street has a direction (one-way or bidirectional), length, and cost (time to traverse).

Input:
- Number of junctions and streets
- Available time and number of cars
- Starting junction
- List of junctions with coordinates
- List of streets with endpoints, direction, length, and cost

Output:
- Route for each car as a sequence of junctions",
                SolutionExplanation: @"Solution Approach:
1. Parse the input to build a graph of junctions and streets
2. Use a greedy algorithm to find routes that maximize coverage
3. Prioritize streets that haven't been covered yet
4. Balance the workload across multiple cars

Key Code Concepts:
- Graph representation using adjacency lists
- Pathfinding algorithms (e.g., Dijkstra, BFS)
- Optimization heuristics for coverage maximization

The solution uses F# with pattern matching for clean graph traversal and functional approaches to route optimization.",
                InputFilePath: "2014/paris_54000.txt",
                SolutionRunner: Solutions.StreetViewSolution.Run
            ),
            new Challenge(
                Year: 2015,
                Round: "Qualification Round",
                Name: "Optimize a Data Center",
                ShortDescription: "Optimize server placement in data center to maximize availability",
                FullDescription: @"For over ten years, Google has been building data centers of its own design, deploying thousands of machines in locations around the globe. In each of these locations, batteries of servers are at work around the clock, running services we use every day, from Google Search and YouTube to the Judge System of Hash Code.

Problem:
Given a schema of a data center and a list of available servers, optimize the layout of the data center to maximize its availability. The data center is organized in rows, each row has slots where servers can be placed. Some slots may be unavailable. Servers must be assigned to pools, and the goal is to balance capacity across pools while accounting for row failures.

Input:
- Data center dimensions (rows and slots per row)
- List of unavailable slots
- Number of pools
- List of servers with size and capacity

Output:
- Assignment of each server to a row, starting slot, and pool
- Or indication that a server is not used

Goal:
Maximize the guaranteed capacity of each pool, which is the minimum capacity available to a pool when any single row fails.",
                SolutionExplanation: @"Solution Approach:
1. Parse the input to understand data center layout and server specs
2. Sort servers by capacity-to-size ratio for efficient placement
3. Distribute servers across rows to balance pool capacity
4. Ensure that each pool maintains capacity even if one row fails

Key Code Concepts:
- 2D array representation of data center slots
- Greedy placement algorithms
- Capacity calculation considering row failures
- Pool balancing heuristics

The solution uses F# with discriminated unions for slot states and functional approaches to optimize server placement.",
                InputFilePath: "2015/Qualification Round/dc.in",
                SolutionRunner: Solutions.DataCenterSolution.Run
            ),
            new Challenge(
                Year: 2015,
                Round: "Final Round",
                Name: "Loon",
                ShortDescription: "Plan altitude adjustments for Internet-providing balloons",
                FullDescription: @"Project Loon aims to bring universal Internet access using a fleet of high altitude balloons equipped with LTE transmitters. Circulating around the world, Loon balloons deliver Internet access in areas that lack conventional means of Internet connectivity.

Problem:
Given wind data at different altitudes and target locations that need coverage, plan altitude adjustments for a fleet of balloons to provide Internet coverage to select locations. Balloons can only move by changing altitude to catch different wind currents.

Input:
- Grid dimensions and altitude levels
- Wind data for each cell and altitude
- Number of balloons and turns
- Target locations and their coverage radius
- Starting positions of balloons

Output:
- Altitude adjustment for each balloon at each turn

Goal:
Maximize the total coverage of target locations across all turns.",
                SolutionExplanation: "This challenge was part of the 2015 final round. A complete solution implementation is not yet available in this repository.",
                InputFilePath: null,
                SolutionRunner: null
            )
        };
    }

    public static Dictionary<int, List<Challenge>> GetChallengesByYear()
    {
        return GetAllChallenges()
            .GroupBy(c => c.Year)
            .ToDictionary(g => g.Key, g => g.ToList());
    }
}
