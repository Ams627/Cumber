using Cumber.CliOption;

namespace Cumber.HelpSystem;

public interface IHelpAccessor
{
    /// <summary>
    /// Determine the index of the first non-matching command
    /// </summary>
    /// <param name="toolName">The name of the tool (the name of the program executed without the path or the extension). If empty matches the first tool in the helpText</param>
    /// <param name="args">All the arguments from the command line (excluding the tool name)</param>
    /// <param name="indexAfterMatch">On exit this program will return the first argument after any matched command segments</param>
    /// <returns>true if a complete command from the root to the leaf node is matched; false otherwise</returns>
    bool GetLongestMatchingCommand(string toolName, string[] args, out int indexAfterMatch);

    /// <summary>
    /// Get the help text for a command.
    /// </summary>
    /// <param name="toolName">The name of the tool (the name of the program executed without the path or the extension). If empty matches the first tool in the helpText</param>
    /// <param name="args">All the arguments from the command line (excluding the tool name)</param>
    /// <param name="n">The number of arguments to use from the command line</param>
    /// <param name="helpText">The returned helptext</param>
    /// <returns>true if a complete command from the root to the leaf node is matched; false otherwise</returns>
    bool GetHelpTextForCommand(string toolName, string[] args, int n, out string helpText);

    /// <summary>
    /// Get the valid options for a command.
    /// </summary>
    /// <param name="toolName">The name of the tool (the name of the program executed without the path or the extension). If empty matches the first tool in the helpText</param>
    /// <param name="args">All the arguments from the command line</param>
    /// <param name="n">The number of arguments to use from the command line</param>
    /// <returns></returns>
    List<Option> GetPermittedOptionsForCommand(string toolName, string[] args, int n);

    /// <summary>
    /// Dump the command tree(s) (without any help descriptions)
    /// </summary>
    /// <param name="toolName">if not null or empty, dump only the help for the tool specified. Otherwise dump all the command tree(s)</param>
    /// <returns>A simple ascii tree representation of the command (like the Windows "tree" command)</returns>
    string DumpCommandTree(string? toolName = null);


    /// <summary>
    /// Dump all sections of the help
    /// </summary>
    /// <param name="toolName">if not null or empty, dump only the help for the tool specified. Otherwise dump all the help</param>
    /// <returns>All the help text or the help text for the tool</returns>
    string DumpAllHelp(string? toolName = null);
}

