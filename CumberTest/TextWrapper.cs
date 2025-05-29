using System.Text;
using System.Text.RegularExpressions;

namespace Cumber;

public static class TextWrapper
{
    public static string RewrapText(string input, int width = -1)
    {
        width = width == -1 ? GetConsoleWidthOrDefault() : width;
        string normalized = input.Replace("\r\n", "\n");
        normalized = normalized.Replace("\u21B5", "\n\u21B5\n"); // Treat ↵ as hard line break

        string[] paragraphs = Regex.Split(normalized, @"\n\s*\n");

        var sb = new StringBuilder();
        foreach (string para in paragraphs)
        {
            string[] lines = para.Split('\n');

            int lineLen = 0;
            var lineBuilder = new StringBuilder();
            string currentIndent = "";

            foreach (var rawLine in lines)
            {
                if (rawLine.Trim() == "\u21B5")
                {
                    if (lineBuilder.Length > 0)
                        sb.AppendLine(lineBuilder.ToString().TrimEnd());
                    else
                        sb.AppendLine(currentIndent);
                    lineBuilder.Clear();
                    lineLen = 0;
                    continue;
                }

                string line = rawLine;
                string indent = Regex.Match(line, @"^\s*").Value;
                currentIndent = indent;
                line = Regex.Replace(line, @"\s*\n\s*", " ");
                List<string> words = new Regex(@"(?<=\S) (?=\S)").Split(line).ToList();

                if (lineBuilder.Length == 0)
                    lineBuilder.Append(indent);
                lineLen = lineBuilder.Length;

                foreach (var word in words)
                {
                    string visible = StripAnsi(word);
                    int wordLen = visible.Length;

                    if (lineLen + wordLen + 1 > width && lineBuilder.Length > indent.Length)
                    {
                        sb.AppendLine(lineBuilder.ToString().TrimEnd());
                        lineBuilder.Clear();
                        lineBuilder.Append(indent);
                        lineLen = indent.Length;
                    }

                    lineBuilder.Append(word).Append(' ');
                    lineLen += wordLen + 1;
                }
            }

            if (lineBuilder.Length > currentIndent.Length)
                sb.AppendLine(lineBuilder.ToString().TrimEnd());

            sb.AppendLine();
        }

        return sb.ToString().TrimEnd();
    }

    private static string StripAnsi(string input)
    {
        return Regex.Replace(input, @"\u001B\[[0-9;]*[A-Za-z]", "");
    }

    static int GetConsoleWidthOrDefault(int fallback = 80)
    {
        try
        {
            return Console.IsOutputRedirected ? fallback : Console.WindowWidth;
        }
        catch
        {
            return fallback;
        }
    }

}

