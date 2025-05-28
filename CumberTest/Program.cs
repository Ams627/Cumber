using Cumber.HelpSystem;
using System.Text;
using System.Text.RegularExpressions;

namespace Cumber;

class Program
{
    private static string HelpText = """
        Put all the help-text here. You can put banners and copyright notices here if you wish: 
        they will not be displayed.

        @group Sphincter
            -a, --all       let's read all the _files_.
        Hello, world
                            yes, all of them you daft prick!
            -b, --bottom - let's read all the files from the bottom to the top!
        
        = CumberTest - test the cumber help system
        
        This program is used to *test* the cumber help system.
        
        == doit

        Do something really bad. Subcommands are
        $(subcommands) 

        
        === all
        Do **everything** that it is [green]#necessary# to do

        Options:
            @include Sphincter
            -p, --pause     pause the _entire_ system momentarily
            -w, --write     write over the Windows folder in c:\

        If you specify any of the above options, a kitten will be killed.
        
        === partly
        Options:
            -a, --all       do all of it!
            -b, --briefly   do it briefly

        ==== fast
        Options:
            -a, --all       do all of it!
            -b, --briefly   do it briefly

        ==== slow

        Do a partial run, but do it very slowly.

        Options:
            -a, --all       do all of it!
            -b, --briefly   do it briefly
        === half
        Options:
            -a, --all       do all of it!
            -b, --briefly   do it briefly
        
        
        """;
    private static async Task Main(string[] args)
    {
        try
        {
            List<HelpSection> sections = HelpTextParser.Parse(HelpText);
            var lk = sections.ToLookup(x => x.CommandPath);

            var path = string.Join(" ", args);
            var instance = lk[path].First();
            var text = instance.HelpText;

            var rewrapped = RewrapText(text, 80);
            Console.WriteLine(rewrapped);

            Console.WriteLine($"There are {instance.Options.Count} options:");
            foreach (var opt in instance.Options)
            {
                Console.WriteLine($"    -{opt.ShortOption} {opt.LongOption}: {opt.Description}");
            }

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            var fullname = System.Reflection.Assembly.GetEntryAssembly().Location;
            var progname = Path.GetFileNameWithoutExtension(fullname);
            Console.Error.WriteLine($"{progname} Error: {ex}");
        }
    }

    static string RewrapText(string input, int width)
    {
        // Normalize line endings and split into paragraphs
        string normalized = input.Replace("\r\n", "\n");
        string[] paragraphs = Regex.Split(normalized, @"\n\s*\n");

        var sb = new StringBuilder();

        foreach (string para in paragraphs)
        {
            // Remove single newlines, compress whitespace to single spaces
            string clean = Regex.Replace(para, @"\s*\n\s*", " ");
            clean = Regex.Replace(clean, @"\s+", " ").Trim();

            // Re-wrap text to the specified width
            int lineStart = 0;
            while (lineStart < clean.Length)
            {
                int remaining = clean.Length - lineStart;
                int lineLength = Math.Min(width, remaining);
                int searchEnd = lineStart + lineLength;

                // Look for last space in the allowed range
                int lastSpace = clean.LastIndexOf(' ', searchEnd - 1, lineLength);

                if (lastSpace > lineStart)
                {
                    sb.AppendLine(clean.Substring(lineStart, lastSpace - lineStart));
                    lineStart = lastSpace + 1;
                }
                else
                {
                    // No space found, hard wrap
                    sb.AppendLine(clean.Substring(lineStart, lineLength));
                    lineStart += lineLength;
                }
            }
            sb.AppendLine(); // blank line between paragraphs
        }

        return sb.ToString().TrimEnd(); // Remove trailing blank lines
    }

}
