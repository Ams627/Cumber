﻿using Cumber.CliOption;
using System.Collections.Immutable;

namespace Cumber.CommandLine;

public class OptionsParser
{
    private readonly IEnumerable<Option> _supportedOptions;
    private readonly IDictionary<char, int> _charToIndex;
    private readonly IDictionary<string, int> _stringToIndex;
    private readonly IDictionary<char, Option> _shortOptions;
    private readonly IDictionary<string, Option> _longOptions;

    public OptionsParser(IEnumerable<Option> options)
    {
        _supportedOptions = options;

        _charToIndex = options
            .Where(x => x.ShortOption.HasValue)
            .Select((x, i) => new { x.ShortOption, i })
            .ToImmutableDictionary(x => x.ShortOption!.Value, x => x.i);

        _stringToIndex = options
            .Where(x => !string.IsNullOrEmpty(x.LongOption))
            .Select((x, i) => new { x.LongOption, i })
            .ToImmutableDictionary(x => x.LongOption!, x => x.i);

        _shortOptions = options
            .Where(x => x.ShortOption.HasValue)
            .ToImmutableDictionary(x => x.ShortOption!.Value);

        _longOptions = options
            .Where(x => !string.IsNullOrEmpty(x.LongOption))
            .ToImmutableDictionary(x => x.LongOption!);
    }

    public ParsedOptionsResult Parse(string[] args, int offset = 0, string[]? allowedGroups = null)
    {
        var queue = new RewindableQueue<string>(args, offset);
        HashSet<string> allowed = allowedGroups is null ? [] : [.. allowedGroups];
        var parsedOptions = new Dictionary<int, List<ParsedOption>>();
        var illegalOptions = new List<IllegalOption>();
        var nonOptions = new List<NonOption>();

        while (!queue.Empty)
        {
            var (arg, index) = queue.PopFront();

            if (arg.StartsWith("--"))
            {
                if (arg.Length == 2)
                    break;
                var eqPos = arg.IndexOf('=');
                if (eqPos == 0)
                {
                    illegalOptions.Add(new("--=", index, ErrorCodes.EqualFirstChar));
                }
                else if (eqPos > 0)
                {
                    var name = arg[2..eqPos];
                    var value = arg[(eqPos + 1)..];
                    if (!_longOptions.TryGetValue(name, out Option? spec))
                    {
                        illegalOptions.Add(new(name, index, ErrorCodes.OptionNotSpecified));
                        continue;
                    }
                    if (spec.Parameters?.Count != 1)
                    {
                        illegalOptions.Add(new(name, index, ErrorCodes.EqualOptionNotSingleParam));
                        continue;
                    }
                    if (value.Length == 0)
                    {
                        illegalOptions.Add(new(name, index, ErrorCodes.EqualOptionEmptyParameter));
                    }
                    var parsed = new ParsedOption(index, false, _stringToIndex[name], [value]);
                    DictUtils.AddEntryToList(parsedOptions, parsed.OptionIndex, parsed);
                }
                else
                {
                    var optionName = arg[2..];
                    if (!_longOptions.TryGetValue(optionName, out var spec))
                    {
                        illegalOptions.Add(new(optionName, index, ErrorCodes.OptionNotSpecified));
                        continue;
                    }
                    if (spec.Parameters?.Count > queue.Remaining)
                    {
                        illegalOptions.Add(new(optionName, index, ErrorCodes.OptionNotEnoughParams));
                        continue;
                    }
                    var optionIndex = _stringToIndex[optionName];
                    var parsed = new ParsedOption(index, false, optionIndex, queue.PopN(spec.ParameterCount));
                    DictUtils.AddEntryToList(parsedOptions, optionIndex, parsed);
                }
            }
            else if (arg.StartsWith("-"))
            {
                if (arg.Length == 1)
                {
                    nonOptions.Add(new(arg, index));
                }
                else
                {
                    for (int i = 0; i < arg.Length; i++)
                    {
                        var c = arg[i];
                        var isLast = i == arg.Length - 1;

                        if (!_shortOptions.TryGetValue(c, out var spec))
                        {
                            illegalOptions.Add(new(c.ToString(), index, ErrorCodes.OptionNotSpecified));
                            continue;
                        }

                        var optionIndex = _charToIndex[c];
                        if (isLast)
                        {
                            if (spec.Parameters.Count > queue.Remaining)
                            {
                                illegalOptions.Add(new(c.ToString(), index, ErrorCodes.OptionNotEnoughParams));
                                continue;
                            }
                            var parsed = new ParsedOption(index, false, optionIndex, queue.PopN(spec.Parameters.Count));
                            DictUtils.AddEntryToList(parsedOptions, optionIndex, parsed);
                        }
                        else if (spec.Parameters.Count == 0)
                        {
                            var parsed = new ParsedOption(index, true, optionIndex, []);
                            DictUtils.AddEntryToList(parsedOptions, optionIndex, parsed);
                        }
                        else if (spec.Parameters.Count == 1)
                        {
                            var value = arg[(i + 1)..];
                            var parsed = new ParsedOption(index, true, optionIndex, [value]);
                            DictUtils.AddEntryToList(parsedOptions, optionIndex, parsed);
                            break;
                        }
                        else
                        {
                            illegalOptions.Add(new(c.ToString(), index, ErrorCodes.AdjoiningOptionNotSingleParam));
                            continue;
                        }
                    }
                }
            }
            else
            {
                nonOptions.Add(new(arg, index));
            }
        }

        return new ParsedOptionsResult(
            parsedOptions,
            illegalOptions,
            nonOptions,
            _stringToIndex,
            _charToIndex
        );

    }
}
