using Cumber.CliOption;
using Cumber.CommandLine;
using Cumber.HelpSystem;
using System.Reflection;

namespace Cumber.Helpers;

public static class CliLauncher
{
    public static async Task<int> RunAsync(string helpText, string[] args, Assembly commandsAssembly)
    {
        var toolName = Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]);
        var sections = HelpTextParser.Parse(helpText, toolName);

        // Handle top-level help or help for a specific command
        if (args.Length == 0 || args.Contains("--help") || args[0] == "help")
        {
            var query = args.Length > 1 && args[0] == "help"
                ? string.Join(" ", args.Skip(1))
                : string.Empty;

            HelpTextParser.PrintHelp(sections, query);
            return 0;
        }

        // Determine command path and resolve handler
        var tokens = args.TakeWhile(arg => !arg.StartsWith("-")).ToArray();

        ICommandHandler? handler = null;
        string commandPath = "";

        for (int i = tokens.Length; i > 0; i--)
        {
            var candidate = string.Join(" ", tokens.Take(i));
            handler = CommandRegistry.Resolve(candidate, commandsAssembly);
            if (handler != null)
            {
                commandPath = candidate;
                break;
            }
        }

        if (handler is null)
        {
            if (tokens.Length > 0)
            {
                Console.Error.WriteLine($"'{tokens[0]}' is not a valid command.");
            }
            else
            {
                Console.Error.WriteLine("No command specified.");
            }
            HelpTextParser.PrintHelp(sections, "");
            return 1;
        }

        // Extract options from remaining args
        var commandParts = commandPath.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var nonOptionStart = commandParts.Length;
        var remainingArgs = args.Skip(nonOptionStart).ToArray();

        List<Option> options = HelpTextParser.GetOptions(sections, commandPath);
        var parser = new OptionsParser(options);
        var allowedGroups = handler.GetAllowedOptionGroups().Append("@global").ToArray();
        var parsedOptionsResult = parser.Parse(remainingArgs, allowedGroups: allowedGroups);
        var accessor = new OptionAccessor(parsedOptionsResult);

        // Append actual non-option tokens from remainingArgs
        var consumedArgsCount = parsedOptionsResult.NonOptions.Count + parsedOptionsResult.Parsed.SelectMany(x => x.Value).Sum(p => p.Params!.Count + 1);
        var nonOptionTokens = remainingArgs.Skip(consumedArgsCount).ToArray();
        foreach (var token in nonOptionTokens)
        {
            parsedOptionsResult.NonOptions.Add(new NonOption(token, -1));
        }

        // Execute the command
        return await handler.ExecuteAsync(commandParts, accessor);
    }
}
