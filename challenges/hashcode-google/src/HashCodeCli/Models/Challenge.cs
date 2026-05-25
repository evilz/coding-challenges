namespace HashCodeCli.Models;

public record Challenge(
    int Year,
    string Round,
    string Name,
    string ShortDescription,
    string FullDescription,
    string? SolutionExplanation,
    string? InputFilePath,
    Func<string, string>? SolutionRunner
);
