/*
 * Script to download all Project Euler problems information.
 * Creates a problems.md file with titles and descriptions from problems 1 to 969.
 * 
 * Note: This script requires internet access to projecteuler.net
 * 
 * Usage: 
 *   cd DownloadProblems && dotnet run -- [arguments]
 * 
 * Arguments:
 *   --start N       Starting problem number (default: 1)
 *   --end N         Ending problem number (default: 969)
 *   --output FILE   Output file (default: problems.md)
 *   --delay N       Delay between requests in seconds (default: 0.5)
 *   --test          Test mode - only download first 5 problems
 */

using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;

// Parse command line arguments
var commandArgs = Environment.GetCommandLineArgs().Skip(1).ToArray();
int start = 1;
int end = 969;
string outputFile = "problems.md";
double delay = 0.5;
bool testMode = false;

for (int i = 0; i < commandArgs.Length; i++)
{
    switch (commandArgs[i])
    {
        case "--start":
            if (i + 1 < commandArgs.Length)
            {
                if (!int.TryParse(commandArgs[++i], out start))
                {
                    Console.Error.WriteLine($"Error: Invalid value for --start: {commandArgs[i]}");
                    return;
                }
            }
            break;
        case "--end":
            if (i + 1 < commandArgs.Length)
            {
                if (!int.TryParse(commandArgs[++i], out end))
                {
                    Console.Error.WriteLine($"Error: Invalid value for --end: {commandArgs[i]}");
                    return;
                }
            }
            break;
        case "--output":
            if (i + 1 < commandArgs.Length) outputFile = commandArgs[++i];
            break;
        case "--delay":
            if (i + 1 < commandArgs.Length)
            {
                if (!double.TryParse(commandArgs[++i], out delay))
                {
                    Console.Error.WriteLine($"Error: Invalid value for --delay: {commandArgs[i]}");
                    return;
                }
            }
            break;
        case "--test":
            testMode = true;
            break;
        case "--help":
        case "-h":
            Console.WriteLine("Usage: cd DownloadProblems && dotnet run -- [arguments]");
            Console.WriteLine();
            Console.WriteLine("Arguments:");
            Console.WriteLine("  --start N       Starting problem number (default: 1)");
            Console.WriteLine("  --end N         Ending problem number (default: 969)");
            Console.WriteLine("  --output FILE   Output file (default: problems.md)");
            Console.WriteLine("  --delay N       Delay between requests in seconds (default: 0.5)");
            Console.WriteLine("  --test          Test mode - only download first 5 problems");
            Console.WriteLine("  --help, -h      Show this help message");
            return;
    }
}

if (testMode)
{
    Console.WriteLine("Running in test mode - downloading only problems 1-5");
    start = 1;
    end = 5;
}

await DownloadAllProblems(start, end, outputFile, delay);

static async Task<string?> GetProblemTitle(int problemRef, HttpClient client)
{
    string url = $"https://projecteuler.net/problem={problemRef}";
    try
    {
        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        
        var html = await response.Content.ReadAsStringAsync();
        
        // Simple parsing to extract title from h2 tag
        // Looking for pattern: <h2>Problem Title</h2>
        var h2Start = html.IndexOf("<h2>");
        if (h2Start >= 0)
        {
            h2Start += 4; // Move past <h2>
            var h2End = html.IndexOf("</h2>", h2Start);
            if (h2End > h2Start)
            {
                var title = html.Substring(h2Start, h2End - h2Start).Trim();
                // Decode HTML entities
                title = System.Net.WebUtility.HtmlDecode(title);
                return title;
            }
        }
        return null;
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"Error fetching title for problem {problemRef}: {ex.Message}");
        return null;
    }
}

static async Task<string?> GetProblemDescription(int problemRef, HttpClient client)
{
    string url = $"https://projecteuler.net/minimal={problemRef}";
    try
    {
        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        
        var html = await response.Content.ReadAsStringAsync();
        
       
        return html;
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"Error fetching description for problem {problemRef}: {ex.Message}");
        return null;
    }
}

static async Task DownloadAllProblems(int start, int end, string outputFile, double delay)
{
    Console.WriteLine($"Downloading problems {start} to {end}...");
    Console.WriteLine($"Output file: {outputFile}");
    
    using var client = new HttpClient();
    client.Timeout = TimeSpan.FromSeconds(30);
    
    using var writer = new StreamWriter(outputFile, false, System.Text.Encoding.UTF8);
    await writer.WriteLineAsync("# Project Euler Problems");
    await writer.WriteLineAsync();
    await writer.WriteLineAsync($"Problems {start} to {end}");
    await writer.WriteLineAsync();
    await writer.WriteLineAsync("---");
    await writer.WriteLineAsync();
    
    for (int problemRef = start; problemRef <= end; problemRef++)
    {
        Console.Write($"Processing problem {problemRef}... ");
        
        var title = await GetProblemTitle(problemRef, client);
        var description = await GetProblemDescription(problemRef, client);
        
        if (title != null && description != null)
        {
            await writer.WriteLineAsync($"## Problem {problemRef}: {title}");
            await writer.WriteLineAsync();
            await writer.WriteLineAsync(description);
            await writer.WriteLineAsync();
            await writer.WriteLineAsync("---");
            await writer.WriteLineAsync();
            Console.WriteLine("✓");
        }
        else
        {
            Console.WriteLine($"✗ (title: {(title != null ? "✓" : "✗")}, description: {(description != null ? "✓" : "✗")})");
            if (title != null)
            {
                await writer.WriteLineAsync($"## Problem {problemRef}: {title}");
                await writer.WriteLineAsync();
                await writer.WriteLineAsync("*Description not available*");
                await writer.WriteLineAsync();
                await writer.WriteLineAsync("---");
                await writer.WriteLineAsync();
            }
        }
        
        // Be nice to the server - add a small delay
        await Task.Delay(TimeSpan.FromSeconds(delay));
    }
    
    Console.WriteLine($"\n✓ Done! Problems saved to {outputFile}");
}
