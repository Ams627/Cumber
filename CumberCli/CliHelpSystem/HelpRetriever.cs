using Cumber.CliOption;

namespace Cumber.HelpSystem;

public class HelpRetriever
{
    public string HelpText => _helpSection.HelpText;
    public string Command => _helpSection.CommandPath;
    public List<Option> PermittedOptions => _helpSection.Options;
    public int NumberOfArgsConsumed => _helpSection.CommandLength;
    public bool IsHelpCommand => _isHelpCommand;

    private readonly HelpSection _helpSection;
    private readonly bool _isHelpCommand;

    public HelpRetriever(HelpSection helpSection, bool isHelpCommand)
    {
        _helpSection = helpSection;
        this._isHelpCommand = isHelpCommand;
    }

    public static bool TryCreate(string helpText, string toolname, string[] args, out HelpRetriever? helpTextRetriever)
    {
        IHelpAccessor help = HelpAccessor.Create(helpText);

        if (args.Length == 0)
        {
            // in this case we get the help text for the case when there are no commands given. 
            // this is the help text for the commands at the root of the tree.
            if (help.GetHelpSectionForCommand(toolname, [], -1, out var helpSection))
            {
                helpTextRetriever = new HelpRetriever(helpSection!, isHelpCommand: true);
                return true;
            }
            helpTextRetriever = default;
            return false;
        }

        var sectionArgs = args.TakeWhile(x => !x.StartsWith("-")).ToArray();

        if (args.Length >= 1 && args[0] == "help")
        {
            if (help.GetHelpSectionForCommand(toolname, sectionArgs.Skip(1), -1, out HelpSection? helpSection))
            {
                helpTextRetriever = new HelpRetriever(helpSection!, isHelpCommand: true);
                return true;
            }
            helpTextRetriever = default;
            return false;
        }

        // here we have a real command, not a request for help (e.g. we might have git remote add, not git help remote add)
        // We retrieve the helptext and the options here, but these may not be relevant to clients of this method
        if (help.GetHelpSectionForCommand(toolname, sectionArgs, -1, out HelpSection? section))
        {
            helpTextRetriever = new HelpRetriever(section!, isHelpCommand: false);
            return true;
        }

        helpTextRetriever = default;
        return false;
    }
}