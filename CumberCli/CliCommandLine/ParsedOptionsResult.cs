namespace Cumber.CommandLine;

public record ParsedOptionsResult(
    Dictionary<int, List<ParsedOption>> Parsed,
    List<IllegalOption> IllegalOptions,
    List<NonOption> NonOptions,
    IDictionary<string, int> LongNameToIndex,
    IDictionary<char, int> ShortNameToIndex
);