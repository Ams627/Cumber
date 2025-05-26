namespace Cumber.CommandLine;

using System.Collections.Generic;
using System.Linq;
public class OptionAccessor : IOptionAccessor
{
    private readonly Dictionary<int, List<ParsedOption>> _parsed;
    private readonly IDictionary<string, int> _longNameToIndex;
    private readonly IDictionary<char, int> _shortNameToIndex;
    private readonly List<NonOption> _nonOptions;
    private readonly List<IllegalOption> _illegalOptions;

    public OptionAccessor(ParsedOptionsResult result)
    {
        _parsed = result.Parsed;
        _longNameToIndex = result.LongNameToIndex;
        _shortNameToIndex = result.ShortNameToIndex;
        _nonOptions = result.NonOptions;
        _illegalOptions = result.IllegalOptions;
    }

    private bool TryResolveIndex<T>(T name, out int index)
    {
        if (name is string s)
            return _longNameToIndex.TryGetValue(s, out index);
        else if (name is char c)
            return _shortNameToIndex.TryGetValue(c, out index);

        index = -1;
        return false;
    }

    public IReadOnlyCollection<NonOption> NonOptions => _nonOptions;
    public IReadOnlyCollection<IllegalOption> IllegalOptions => _illegalOptions;

    public bool IsOptionPresent<T>(T name)
    {
        return TryResolveIndex(name, out var index) && _parsed.TryGetValue(index, out var list) && list.Count > 0;
    }

    public bool TryGetParam<T>(T name, out string? value, int offset = 0)
    {
        value = null;
        if (TryResolveIndex(name, out var optionIndex) && _parsed.TryGetValue(optionIndex, out List<ParsedOption>? entries) && entries.Count > offset)
        {
            var entry = entries[offset];
            if (entry.Params!.Count > 0)
            {
                value = entry.Params[0];
                return true;
            }
        }
        return false;
    }

    public bool TryGetParams<T>(T name, out List<string> values, int offset = 0)
    {
        values = [];
        if (TryResolveIndex(name, out var optionIndex) && _parsed.TryGetValue(optionIndex, out var entries) && entries.Count > offset)
        {
            values = entries[offset].Params!;
            return true;
        }
        return false;
    }

    public bool TryGetAllParams<T>(T name, out List<string> allValues)
    {
        allValues = [];
        if (TryResolveIndex(name, out var optionIndex) && _parsed.TryGetValue(optionIndex, out var entries))
        {
            allValues = [.. entries.SelectMany(e => e.Params!)];
            return true;
        }
        return false;
    }
}
