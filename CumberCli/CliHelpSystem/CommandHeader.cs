namespace Cumber.HelpSystem;

class CommandHeader
{
    public string Command { get; private init; } = string.Empty;
    public string FullCommand { get; private init; } = string.Empty;
    public string CommandSummary { get; private init; } = string.Empty;


    private static Stack<string> _commandStack = [];
    public static bool TryCreate(string line, out CommandHeader? commandHeader)
    {
        var match = HelpTextRegexes.HeaderRegex.Match(line);
        if (!match.Success)
        {
            commandHeader = default;
            return false;
        }

        int level = match.Groups["headerIntroducer"].Value.Length;

        if (level == 1)
        {
            _commandStack.Clear();
        }


        var commandGrp = match.Groups["commandName"];
        if (!commandGrp.Success)
        {
            commandHeader = default;
            return false;
        }

        while (_commandStack.Count >= level - 1)
        {
            _commandStack.Pop();
        }
        _commandStack.Push(commandGrp.Value);


        var commandSummaryGrp = match.Groups["commandSummary"];
        string commandSummary = commandSummaryGrp.Success ? commandSummaryGrp.Value : string.Empty;

        commandHeader = new CommandHeader { Command = commandGrp.Value, FullCommand = string.Join(" ", _commandStack.Reverse()), CommandSummary = commandSummary };
        return true;
    }
}

