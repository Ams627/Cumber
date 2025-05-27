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
                    ((?'emptysummary'\s+-\s*$)|(\s+-\s*(?'commandSummary'.+[^\s])\s*$))?
                    $
                    """, RegexOptions.IgnorePatternWhitespace);

    public static readonly Regex OptionLineRegex = new(
    """
            ^\s*                            # Leading whitespace
            (?:-(\w),?\s*)?                 # Optional short option (e.g. -f or -f,)
            (--[\w-]+)?                     # Optional long option (e.g. --file)
            (?:\s+(<[^>]+>|\[[^\]]+\]))?    # Optional parameter 1
            (?:\s+(<[^>]+>|\[[^\]]+\]))?    # Optional parameter 2
            (?:\s+(<[^>]+>|\[[^\]]+\]))?    # Optional parameter 3
            .*                              # Remainder (description)
        """,
    RegexOptions.IgnorePatternWhitespace
);


}

