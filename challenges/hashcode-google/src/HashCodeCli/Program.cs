using Spectre.Console;
using HashCodeCli;
using HashCodeCli.Models;

// Display header
AnsiConsole.Write(
    new FigletText("Hash Code")
        .LeftJustified()
        .Color(Color.Blue));

AnsiConsole.MarkupLine("[bold yellow]Google Hash Code Challenge Viewer[/]");
AnsiConsole.MarkupLine("[dim]Interactive CLI to explore and run Hash Code challenges[/]\n");

// Main menu loop
var exit = false;
while (!exit)
{
    var choice = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("[green]What would you like to do?[/]")
            .PageSize(10)
            .AddChoices(new[]
            {
                "View all challenges by year",
                "Search challenges",
                "Exit"
            }));

    switch (choice)
    {
        case "View all challenges by year":
            ShowChallengesByYear();
            break;
        case "Search challenges":
            SearchChallenges();
            break;
        case "Exit":
            exit = true;
            break;
    }
}

AnsiConsole.MarkupLine("[green]Thank you for using Hash Code Challenge Viewer![/]");

void ShowChallengesByYear()
{
    var challengesByYear = ChallengeRepository.GetChallengesByYear();
    
    var yearChoices = challengesByYear.Keys.OrderByDescending(y => y).Select(y => y.ToString()).ToList();
    yearChoices.Add("Back to main menu");

    var yearChoice = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("[green]Select a year:[/]")
            .AddChoices(yearChoices));

    if (yearChoice == "Back to main menu")
        return;

    var year = int.Parse(yearChoice);
    var challenges = challengesByYear[year];

    var challengeChoices = challenges.Select(c => $"{c.Name} ({c.Round})").ToList();
    challengeChoices.Add("Back");

    var challengeChoice = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title($"[green]{year} Challenges:[/]")
            .PageSize(15)
            .AddChoices(challengeChoices));

    if (challengeChoice == "Back")
        return;

    var selectedChallenge = challenges.First(c => $"{c.Name} ({c.Round})" == challengeChoice);
    ShowChallengeDetails(selectedChallenge);
}

void SearchChallenges()
{
    var searchTerm = AnsiConsole.Ask<string>("[green]Enter search term:[/]");
    var allChallenges = ChallengeRepository.GetAllChallenges();
    var results = allChallenges.Where(c =>
        c.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
        c.ShortDescription.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
        c.Round.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
    ).ToList();

    if (results.Count == 0)
    {
        AnsiConsole.MarkupLine($"[red]No challenges found matching '{searchTerm}'[/]\n");
        return;
    }

    AnsiConsole.MarkupLine($"[green]Found {results.Count} challenge(s):[/]\n");
    
    var challengeChoices = results.Select(c => $"{c.Year} - {c.Name} ({c.Round})").ToList();
    challengeChoices.Add("Back");

    var choice = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("[green]Select a challenge to view:[/]")
            .AddChoices(challengeChoices));

    if (choice == "Back")
        return;

    var selectedChallenge = results.First(c => $"{c.Year} - {c.Name} ({c.Round})" == choice);
    ShowChallengeDetails(selectedChallenge);
}

void ShowChallengeDetails(Challenge challenge)
{
    AnsiConsole.Clear();
    
    // Display challenge header
    var panel = new Panel(
        new Markup($"[bold yellow]{challenge.Name}[/]\n" +
                   $"[dim]{challenge.Year} - {challenge.Round}[/]"))
        .BorderColor(Color.Blue)
        .Padding(1, 0);
    AnsiConsole.Write(panel);

    // Display short description
    AnsiConsole.WriteLine();
    AnsiConsole.MarkupLine($"[bold]Overview:[/]");
    AnsiConsole.MarkupLine($"[dim]{challenge.ShortDescription}[/]");
    AnsiConsole.WriteLine();

    // Menu for challenge actions
    var actions = new List<string>
    {
        "View full description",
        "View solution explanation",
        "Back"
    };

    if (challenge.SolutionRunner != null && challenge.InputFilePath != null)
    {
        actions.Insert(2, "Run solution");
    }

    var action = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("[green]What would you like to do?[/]")
            .AddChoices(actions));

    switch (action)
    {
        case "View full description":
            ShowFullDescription(challenge);
            ShowChallengeDetails(challenge); // Return to details
            break;
        case "View solution explanation":
            ShowSolutionExplanation(challenge);
            ShowChallengeDetails(challenge); // Return to details
            break;
        case "Run solution":
            RunSolution(challenge);
            ShowChallengeDetails(challenge); // Return to details
            break;
        case "Back":
            break;
    }
}

void ShowFullDescription(Challenge challenge)
{
    AnsiConsole.Clear();
    
    var panel = new Panel(
        new Markup($"[bold yellow]{challenge.Name} - Full Description[/]"))
        .BorderColor(Color.Blue);
    AnsiConsole.Write(panel);
    AnsiConsole.WriteLine();

    // Display description with word wrapping
    var descriptionLines = challenge.FullDescription.Split('\n');
    foreach (var line in descriptionLines)
    {
        AnsiConsole.MarkupLine($"[white]{line.EscapeMarkup()}[/]");
    }

    AnsiConsole.WriteLine();
    AnsiConsole.Markup("[dim]Press any key to continue...[/]");
    Console.ReadKey(true);
}

void ShowSolutionExplanation(Challenge challenge)
{
    AnsiConsole.Clear();
    
    var panel = new Panel(
        new Markup($"[bold yellow]{challenge.Name} - Solution Explanation[/]"))
        .BorderColor(Color.Green);
    AnsiConsole.Write(panel);
    AnsiConsole.WriteLine();

    if (string.IsNullOrEmpty(challenge.SolutionExplanation))
    {
        AnsiConsole.MarkupLine("[red]No solution explanation available for this challenge.[/]");
    }
    else
    {
        var explanationLines = challenge.SolutionExplanation.Split('\n');
        foreach (var line in explanationLines)
        {
            AnsiConsole.MarkupLine($"[white]{line.EscapeMarkup()}[/]");
        }
    }

    AnsiConsole.WriteLine();
    AnsiConsole.Markup("[dim]Press any key to continue...[/]");
    Console.ReadKey(true);
}

void RunSolution(Challenge challenge)
{
    AnsiConsole.Clear();
    
    var panel = new Panel(
        new Markup($"[bold yellow]{challenge.Name} - Running Solution[/]"))
        .BorderColor(Color.Yellow);
    AnsiConsole.Write(panel);
    AnsiConsole.WriteLine();

    if (challenge.SolutionRunner == null)
    {
        AnsiConsole.MarkupLine("[red]No solution runner available for this challenge.[/]");
    }
    else if (string.IsNullOrEmpty(challenge.InputFilePath))
    {
        AnsiConsole.MarkupLine("[red]No input file specified for this challenge.[/]");
    }
    else
    {
        // Get the full path relative to the repository root
        var repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../../"));
        var inputPath = Path.Combine(repoRoot, challenge.InputFilePath);

        AnsiConsole.Status()
            .Start("Running solution...", ctx =>
            {
                ctx.Spinner(Spinner.Known.Star);
                ctx.SpinnerStyle(Style.Parse("green"));
                Thread.Sleep(500); // Brief pause for effect
            });

        var result = challenge.SolutionRunner(inputPath);
        
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[bold green]Solution Output:[/]");
        AnsiConsole.WriteLine();
        
        var resultLines = result.Split('\n');
        foreach (var line in resultLines)
        {
            AnsiConsole.MarkupLine($"[white]{line.EscapeMarkup()}[/]");
        }
    }

    AnsiConsole.WriteLine();
    AnsiConsole.Markup("[dim]Press any key to continue...[/]");
    Console.ReadKey(true);
}
