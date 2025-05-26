using System.Reflection;

namespace Cumber;

class Program
{
    private static string HelpText = """
        = CumberTest - test the cumber help system

        This program is used to *test* the cumber help system.

        == doit

        Do something really bad. Subcommands are
        $(subcommands)

        
        === all
        Options:
            -p, --pause     pause the entire system momentarily
            -w, --write     write over the Windows folder in c:\
        
        === partly
        Options:
            -a, --all       do all of it!
            -b, --briefly   do it briefly
        
        """;
    private static async Task Main(string[] args)
    {
        try
        {
            await Cumber.Helpers.CliLauncher.RunAsync(HelpText, args, Assembly.GetExecutingAssembly());
        }
        catch (Exception ex)
        {
            var fullname = System.Reflection.Assembly.GetEntryAssembly().Location;
            var progname = Path.GetFileNameWithoutExtension(fullname);
            Console.Error.WriteLine($"{progname} Error: {ex.Message}");
        }
    }
}
