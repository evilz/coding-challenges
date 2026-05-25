using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

class GenerateRazorProgram 
{
    static void Main(string[] args)
    {
        string mdPath = "problems.md";
        if (!File.Exists(mdPath))
        {
            Console.WriteLine("problems.md not found in " + Directory.GetCurrentDirectory());
            return;
        }

        string content = File.ReadAllText(mdPath);
        
        // Regex to find sections
        // We look for ## Problem X: Title
        // Then content
        // Until --- or End of File
        
        string pattern = @"## Problem (\d+): ([^\r\n]+)\r?\n([\s\S]*?)(?=\r?\n---|$)";
        
        var matches = Regex.Matches(content, pattern);
        Console.WriteLine($"Found {matches.Count} problems.");

        string outDir = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "Components", "Problems"));
        if (!Directory.Exists(outDir))
        {
            Directory.CreateDirectory(outDir);
        }

        int count = 0;
        foreach (Match m in matches)
        {
            if (!m.Success) continue;
            
            string idStr = m.Groups[1].Value;
            if (!int.TryParse(idStr, out int id)) continue;
            
            if (id <= 20) continue; // Already done
            if (id > 100) break;    // Only want up to 100

            string title = m.Groups[2].Value.Trim();
            string desc = m.Groups[3].Value.Trim();

            string filePath = Path.Combine(outDir, $"Problem{id}.razor");
            if (File.Exists(filePath))
            {
                // Console.WriteLine($"Problem {id} already exists.");
                continue;
            }
            
            // Escape double quotes for C# string
            string escapedDesc = desc.Replace("\"", "\\\"");
            // Replace newlines with \n literal for the string
            escapedDesc = escapedDesc.Replace("\r", "").Replace("\n", "\\n");

            string fileContent = $@"@namespace Projecteuler.Components.Problems
@page ""/problem/{id}""
@inherits ProblemBase

@{{
    base.BuildRenderTree(__builder);
}}

@code {{
    protected override int Ref => {id};

    protected override string ProblemTitle => ""{title}"";

    protected override string Description => ""{escapedDesc}"";

    protected override string Explanation => ""Not implemented yet."";

    protected override long Resolve()
    {{
        // TODO: Implement problem {id}
        // {title}
        return 0;
    }}
}}
";
            File.WriteAllText(filePath, fileContent);
            Console.WriteLine($"Generated Problem{id}.razor");
            count++;
        }
        Console.WriteLine($"Total generated: {count}");
    }
}
