using Cumber.CliOption;
using System.Text.RegularExpressions;

namespace Cumber.HelpSystem;

public class HelpTextParser
{
    public static void DumpAllHelp(List<HelpSection> sections, string outputPath)
    {
        using var writer = new StreamWriter(outputPath);

        foreach (var section in sections)
        {
            writer.WriteLine($"== {section.CommandPath}");
            writer.WriteLine(section.HelpText.Trim());

            if (section.Options.Count > 0)
            {
                writer.WriteLine();
                writer.WriteLine("Options:");
                foreach (var opt in section.Options)
                {
                    var names = new List<string>();
                    if (opt.ShortOption != null) names.Add("-" + opt.ShortOption);
                    if (opt.LongOption != null) names.Add(opt.LongOption);

                    foreach (var p in opt.Parameters)
                    {
                        names.Add("<" + p.Name + (p.Type != null ? ":" + p.Type : "") + ">");
                    }

                    writer.WriteLine($"  {string.Join(" ", names),-20} {opt.Description}");
                }
            }

            writer.WriteLine(new string('-', 40));
        }

        Console.WriteLine($"Help dump written to {outputPath}");
    }

    public static List<Option> GetOptions(List<HelpSection> sections, string commandPath)
    {
        return sections
            .FirstOrDefault(s => s.CommandPath.Equals(commandPath, StringComparison.OrdinalIgnoreCase))
            ?.Options ?? [];
    }

    public static List<HelpSection> Parse(string rawText)
    {
        bool foundFirstHeader = false;
        var definedGroups = new Dictionary<string, List<OptionBuilder>>();
        var lines = rawText.Replace("\r", "").Split('\n');
        List<HelpSection> sections = [];

        HelpSection? currentSection = null;
        string? currentGroupName = null;
        OptionBuilder? currentBuilder = null;
        bool inOptions = false;
        bool collectingOptionDescription = false;

        foreach (var rawLine in lines)
        {
            var endTrimmedLine = rawLine.TrimEnd();

            if (!foundFirstHeader)
            {
                // everything we do in this block is processing lines before the first header:
                if (HelpTextRegexes.HeaderRegex.IsMatch(endTrimmedLine))
                {
                    foundFirstHeader = true;
                }
                else if (currentGroupName != null)
                {
                    if (HelpTextRegexes.OptionLineRegex.IsMatch(endTrimmedLine))
                    {
                        definedGroups[currentGroupName].Add(ParseOptionLine(endTrimmedLine));
                        collectingOptionDescription = true;
                    }
                    else if (collectingOptionDescription && HelpTextRegexes.WhiteSpaceAtStart.IsMatch(endTrimmedLine))
                    {
                        definedGroups[currentGroupName][^1].AppendDescription(" ");
                        definedGroups[currentGroupName][^1].AppendDescription(FormatAsciiDoc(endTrimmedLine.Trim()));
                    }
                    else
                    {
                        if (collectingOptionDescription)
                        {
                            definedGroups[currentGroupName][^1].AppendDescription("\u21b5");
                        }
                        collectingOptionDescription = false;
                    }
                    continue;
                }
                else if (DefineGroupIntroducer.TryCreate(rawLine, out DefineGroupIntroducer? defineGroupIntroducer))
                {
                    // found an option group definition starting with @group:
                    currentGroupName = defineGroupIntroducer!.GroupName;
                    definedGroups[currentGroupName] = [];
                    continue;
                }
            }

            if (CommandHeader.TryCreate(rawLine, out CommandHeader? commandHeader))
            {
                currentSection = new HelpSection { CommandPath = commandHeader!.FullCommand, CommandSummary = commandHeader.CommandSummary, CommandLength = commandHeader.CommandLevel };
                sections.Add(currentSection);

                inOptions = false;
                continue;
            }

            // if we're not in a command, section ignore this line:
            if (currentSection == null) continue;

            // look for options introducer:
            if (endTrimmedLine.Trim().Equals("Options:", StringComparison.OrdinalIgnoreCase))
            {
                currentSection.HelpText += "Options:" + "\u21B5";
                inOptions = true;
                continue;
            }

            // include an options group if the line contains @include. Indent the group
            // at the same level as the @include directive itself
            if (inOptions && IncludeHeader.TryCreate(rawLine, out IncludeHeader? includeHeader))
            {
                if (definedGroups.TryGetValue(includeHeader!.GroupName, out var groupOptions))
                {
                    foreach (var optBuilder in groupOptions)
                    {
                        var opt = optBuilder.Build();
                        currentSection.Options.Add(opt);
                        currentSection.HelpText += $"{includeHeader!.Indent}{opt.Description}";
                    }
                }
                else
                {
                    Console.WriteLine("Warning: invalid options group");
                }
                continue;
            }

            if (inOptions)
            {
                // check for lines matching an option specification:
                if (HelpTextRegexes.OptionLineRegex.IsMatch(endTrimmedLine))
                {
                    if (collectingOptionDescription)
                    {
                        collectingOptionDescription = false;
                        currentSection.HelpText += "\u21b5";
                    }
                    var optionBuilder = ParseOptionLine(endTrimmedLine);
                    currentSection.Options.Add(optionBuilder.Build());
                    currentSection.HelpText += FormatAsciiDoc(endTrimmedLine);
                    collectingOptionDescription = true;
                }
                else if (collectingOptionDescription && currentBuilder != null && endTrimmedLine.StartsWith(" "))
                {
                    currentSection.HelpText += " " + FormatAsciiDoc(endTrimmedLine);
                }
                else
                {
                    if (collectingOptionDescription)
                    {
                        currentSection.HelpText += " " + FormatAsciiDoc(endTrimmedLine);
                        //var last = currentSection.Options.Last();
                        //currentSection.Options[^1] = last with { Description = last.Description + "\u21B5" };
                    }
                    collectingOptionDescription = false;
                    currentSection.HelpText += endTrimmedLine + Environment.NewLine;
                }
            }
            else if (endTrimmedLine.TrimStart().StartsWith("#"))
            {
                continue; // skip comment lines
            }
            else
            {
                var cleanLine = endTrimmedLine.StartsWith("\\#") ? endTrimmedLine[1..] : endTrimmedLine;
                currentSection.HelpText += FormatAsciiDoc(cleanLine) + Environment.NewLine;
            }
        }

        return sections;
    }

    public static void PrintHelp(List<HelpSection> sections, string commandPath)
    {
        var section = sections.FirstOrDefault(s => s.CommandPath.Equals(commandPath, StringComparison.OrdinalIgnoreCase));
        if (section == null)
        {
            Console.WriteLine($"No help found for '{commandPath}'");
            return;
        }

        var helpText = section.HelpText;
        if (section.CommandPath == "" && helpText.Contains("$(subcommands)"))
        {
            var children = sections
                .Where(s => s.CommandPath.Split(' ').Length == 1 && s.CommandPath != "")
                .Select(s => $"  {s.CommandPath,-12} {s.CommandSummary}");
            var subList = string.Join(Environment.NewLine, children);
            helpText = helpText.Replace("$(subcommands)", subList);
        }
        Console.WriteLine(helpText.Trim());

        if (section.Options.Count > 0)
        {
            Console.WriteLine();
            Console.WriteLine("Options:");
            foreach (var opt in section.Options)
            {
                List<string> names = [];
                if (opt.ShortOption != null && opt.LongOption != null)
                    names.Add($"-{opt.ShortOption}, --{opt.LongOption}");
                else if (opt.ShortOption != null)
                    names.Add($"-{opt.ShortOption}");
                else if (opt.LongOption != null)
                    names.Add($"--{opt.LongOption}");

                foreach (var p in opt.Parameters)
                {
                    names.Add("<" + p.Name + (p.Type != null ? ":" + p.Type : "") + ">");
                }

                Console.WriteLine($"  {string.Join(" ", names),-20} {FormatAsciiDoc(opt.Description!)}");
            }
        }
    }
    private static string FormatAsciiDoc(string input)
    {
        input = Regex.Replace(input, @"\[(\w+)\]#(.*?)#", m =>
        {
            string color = m.Groups[1].Value.ToLower();
            string text = m.Groups[2].Value;
            string code = color switch
            {
                "red" => "\u001b[31m",
                "green" => "\u001b[32m",
                "yellow" => "\u001b[33m",
                "blue" => "\u001b[34m",
                "magenta" => "\u001b[35m",
                "cyan" => "\u001b[36m",
                "white" => "\u001b[37m",
                _ => ""
            };
            return code + text + "\u001b[0m";
        });

        input = Regex.Replace(input, "\\*\\*(.+?)\\*\\*", "\u001b[1m$1\u001b[22m");  // bold
        input = Regex.Replace(input, "_(.+?)_", "\u001b[4m$1\u001b[24m");              // underline
        input = Regex.Replace(input, "\\*(.+?)\\*", "\u001b[3m$1\u001b[23m");          // italic

        return input;
    }

    private static OptionBuilder ParseOptionLine(string line)
    {
        var match = HelpTextRegexes.OptionLineRegex.Match(line);

        Group shortOptionGroup = match.Groups["shortOption"];
        Group longOptionGroup = match.Groups["longOption"];

        var shortName = shortOptionGroup.Success ? shortOptionGroup.Value[0] : (char?)null;
        var longName = longOptionGroup.Success ? longOptionGroup.Value : null;

        var parameters = new List<(string Name, string? Type)>();
        for (int i = 0; i < 5; i++)
        {
            var g = match.Groups[$"param{i}"];
            if (g.Success)
            {
                var paramMatch = Regex.Match(g.Value, "<(?<name>[^:>]+)(:(?<type>[^>]+))?>");
                var name = paramMatch.Success ? paramMatch.Groups["name"].Value : g.Value;
                var type = paramMatch.Groups["type"].Success ? paramMatch.Groups["type"].Value : null;
                parameters.Add((name, type));
            }
        }

        var builder = new OptionBuilder()
            .WithShortOption(shortName)
            .WithLongOption(longName)
            .AppendDescription(FormatAsciiDoc(line));

        foreach (var (name, type) in parameters)
        {
            builder.WithParameter(name, type);
        }

        return builder;
    }
}

