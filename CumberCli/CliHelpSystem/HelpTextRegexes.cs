using System.Text.RegularExpressions;

namespace Cumber.HelpSystem;

public static class HelpTextRegexes
{
    /// <summary>
    /// let's not change these to GeneratedRegexAttribute as VS cannot render the comments properly!
    /// </summary>
    public static Regex HeaderRegex => new Regex("""
                    ^(?'headerIntroducer'=+)            # one or more equals signs
                    \s*                                 # white space
                    (?'commandName'[^\s]+)              # command name (cannot contain a space)
                    (?:
                    (\s+-\s*(?'commandSummary'.+[^\s])\s*$)
                    |
                    (?'emptysummary'\s+-\s*$)
                    )?
                    $
                    """, RegexOptions.IgnorePatternWhitespace);

    public static Regex IncludeGroupRegex => new Regex("""
                    ^(?'whitespace'\s*)            # zero or more whitespace characters
                    (?:@include\s+)                # @include
                    (?'groupName'\w+)              # group name
                    $
                    """, RegexOptions.IgnorePatternWhitespace);


    public static Regex DefineGroupRegex => new Regex("""
                    ^(?'whitespace'\s*)            # zero or more whitespace characters
                    (?:@group\s+)                  # @include
                    (?'groupName'\w+)              # group name
                    $
                    """, RegexOptions.IgnorePatternWhitespace);


    public static readonly Regex OptionLineRegex = new(
        """
                    ^\s*                                # Leading whitespace
                    (?'option'                          # start of option group (includes both options - must have one of them!)
                    (?'both'-(?'shortOption'\w),?\s*)?(?:--(?'longOption'[\w-]+))        # long option (e.g. --file)
                    |                                   # alternation
                    (?:-(?'shortOption'\w)\s*)          # short option (e.g. -f or -f,)
                    )                                   # end of option group
                    (?:\s+(?'param1'<[^>]+>|\[[^\]]+\]))?        # Optional parameter 1
                    (?:\s+(?'param2'<[^>]+>|\[[^\]]+\]))?        # Optional parameter 2
                    (?:\s+(?'param3'<[^>]+>|\[[^\]]+\]))?        # Optional parameter 3
                    (?:\s+(?'param4'<[^>]+>|\[[^\]]+\]))?        # Optional parameter 3
                    (?:\s+(?'param5'<[^>]+>|\[[^\]]+\]))?        # Optional parameter 3
                    .*                                  # Remainder (description)
                """,
        RegexOptions.IgnorePatternWhitespace
        );




    public static readonly Regex WhiteSpaceAtStart = new("""^\s+ # Leading whitespace""", RegexOptions.IgnorePatternWhitespace);
}

