
namespace Cumber.HelpSystem;

internal class IncludeHeader
{
    public string GroupName { get; private init; }
    public string Indent { get; private init; }

    public IncludeHeader(string groupName, string indent)
    {
        GroupName = groupName ?? string.Empty;
        Indent = indent ?? string.Empty;
    }
    internal static bool TryCreate(string rawLine, out IncludeHeader? includeHeader)
    {
        var includeMatch = HelpTextRegexes.IncludeGroupRegex.Match(rawLine);
        if (!includeMatch.Success)
        {
            includeHeader = default;
            return false;
        }
        var grpNameMatch = includeMatch.Groups["groupName"];
        if (!grpNameMatch.Success)
        {
            includeHeader = default;
            return false;
        }
        var groupName = grpNameMatch.Value;
        var wsGrp = includeMatch.Groups["whiteSpace"];
        var indent = wsGrp.Success ? wsGrp.Value : string.Empty;

        includeHeader = new IncludeHeader(groupName, indent);
        return true;
    }
}
