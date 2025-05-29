using Cumber.Helpers;
using System.Reflection;

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
        Do **everything** that it is [green]#necessary# to do for the cat to be appropriately positioned on the charcoal before you light it. The cat will start to scream but this does not matter!

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
        This is the halfway staging post!

        Options:
            -a, --all       do all of it!
            -b, --briefly   do it briefly
        
        === three
        This is 3/4 complete
        
        Options:
            -a, --all       do all of it!
            -b, --briefly   do it briefly
                
        """;
    private static async Task Main(string[] args)
    {
        try
        {
            var tool = Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]);
            await CliLauncher.RunAsync(HelpText, tool, args, Assembly.GetExecutingAssembly());
        }
        catch (Exception ex)
        {
            var fullname = System.Reflection.Assembly.GetEntryAssembly().Location;
            var progname = Path.GetFileNameWithoutExtension(fullname);
            Console.Error.WriteLine($"{progname} Error: {ex}");
        }
    }
}


