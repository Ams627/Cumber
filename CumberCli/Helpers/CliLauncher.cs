using Cumber.CommandLine;
using Cumber.HelpSystem;
using System.Reflection;

namespace Cumber.Helpers;

public static class CliLauncher
{
    public static async Task<int> RunAsync(string helpText, string toolName, string[] args, Assembly commandsAssembly)
    {
        if (HelpRetriever.TryCreate(helpText, toolName, args, out HelpRetriever? helpTextRetriever))
        {
            if (helpTextRetriever!.IsHelpCommand)
            {
                var rewrapped = TextWrapper.RewrapText(helpTextRetriever!.HelpText);
                Console.WriteLine(rewrapped);
                return 0;
            }

            var command = helpTextRetriever.Command;
            string commandWithoutTool = command.Contains(' ') ? command[(command.IndexOf(' ') + 1)..] : "";


            ICommandHandler? handler = CommandRegistry.Resolve(commandWithoutTool, commandsAssembly);

            if (handler is null)
            {
                Console.Error.WriteLine($"The command {helpTextRetriever.Command} is not implemented.");
                return 0;
            }

            var optionParser = new OptionsParser(helpTextRetriever.PermittedOptions);
            var parsedOptionsResult = optionParser.Parse(args, helpTextRetriever.NumberOfArgsConsumed);
            var accessor = new OptionAccessor(parsedOptionsResult);

            return await handler.ExecuteAsync(args, accessor);
        }
        Console.WriteLine("Invalid command");
        return 0;
    }
}