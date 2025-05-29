using Cumber.CliOption;

namespace Cumber.HelpSystem;

public class HelpAccessor : IHelpAccessor
{
    private readonly string _helpText;
    private readonly List<HelpSection> _sections;

    private HelpAccessor(string helpText)
    {
        _helpText = helpText;
        _sections = HelpTextParser.Parse(helpText);
    }

    public static IHelpAccessor Create(string helpText)
    {
        return new HelpAccessor(helpText);
    }

    public bool GetLongestMatchingCommand(string toolName, string[] args, out int indexAfterMatch)
    {
        var match = FindSection(toolName, args, out var index);
        indexAfterMatch = index;
        return match is not null;
    }

    public bool GetHelpTextForCommand(string toolName, string[] args, int n, out string? helpText)
    {
        string[] argsToUse = n < 0 ? [.. args] : [.. args.Take(n)];
        HelpSection? section = FindSection(toolName, argsToUse, out var index);

        if (section == null)
        {
            helpText = default;
            return false;
        }

        helpText = section.HelpText.TrimEnd();
        return true;
    }
    public bool GetHelpSectionForCommand(string toolName, IEnumerable<string> args, int n, out HelpSection? helpSection)
    {
        string[] argsToUse = n < 0 ? [.. args] : [.. args.Take(n)];
        helpSection = FindSection(toolName, argsToUse, out var index);
        return helpSection != null;
    }

    public List<Option> GetPermittedOptionsForCommand(string toolName, string[] args, int n)
    {
        string[] argsToUse = n < 0 ? [.. args] : [.. args.Take(n)];
        var section = FindSection(toolName, argsToUse, out _);
        if (section == null)
        {
            return [];
        }
        return section.Options;
    }

    public string DumpCommandTree(string? toolName = null)
    {
        var filtered = string.IsNullOrEmpty(toolName) ? _sections : _sections.Where(s => GetLevel(s) == 1 && s.CommandPath == toolName);
        return string.Join("", filtered
            .OrderBy(s => s.CommandPath)
            .Select(s => new string(' ', (GetLevel(s) - 1) * 2) + s.CommandPath));
    }
    public string DumpAllHelp(string? toolName = null)
    {
        return string.Join("", _sections
            .Where(s => string.IsNullOrEmpty(toolName) || s.CommandPath.StartsWith(toolName))
            .Select(s => s.ToString()));
    }

    /// <summary>
    /// Find section with the longest match
    /// </summary>
    /// <param name="toolName">the name of the exe used to start the program</param>
    /// <param name="args">All the arguments of the program</param>
    /// <param name="matchedLength">The number of arguments matched</param>
    /// <returns>the matched section from the help text if found; otherwise null</returns>
    private HelpSection? FindSection(string toolName, string[] args, out int matchedLength)
    {
        matchedLength = 0;
        string[] inputWords = [toolName, .. args];

        HelpSection? bestMatch = null;
        int bestLength = 0;

        var l0 = _sections.Select(x => x.CommandPath);

        foreach (var section in _sections)
        {
            var commandParts = section.CommandPath.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (section.CommandPath == "doit all")
            {
                Console.WriteLine();
            }

            if (commandParts.Length > inputWords.Length)
                continue;

            bool isMatch = true;
            for (int i = 0; i < commandParts.Length; i++)
            {
                if (!string.Equals(inputWords[i], commandParts[i], StringComparison.OrdinalIgnoreCase))
                {
                    isMatch = false;
                    break;
                }
            }

            if (isMatch && commandParts.Length > bestLength)
            {
                bestMatch = section;
                bestLength = commandParts.Length;
            }
        }

        matchedLength = bestLength - 1; // subtract 1 for the tool name
        return bestMatch;
    }

    private static int GetLevel(HelpSection section)
    {
        return section.CommandPath.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
    }
}
