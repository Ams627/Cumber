using Cumber.CommandLine;

namespace Cumber;

[Cumber.HelpSystem.CommandHandler("doit")]
class Doit : Cumber.HelpSystem.ICommandHandler
{
    public Task<int> ExecuteAsync(string[] args, IOptionAccessor optionAccessor)
    {
        Console.WriteLine($"In the doit command args are {string.Join(",", args)}");

        Console.WriteLine($"non-options are {string.Join(",", optionAccessor.NonOptions)}");
        Console.WriteLine($"illegal options are {string.Join(",", optionAccessor.IllegalOptions)}");
        if (optionAccessor.IsOptionPresent("pause"))
        {
            Console.WriteLine("pausing the system");
        }
        else if (optionAccessor.IsOptionPresent("write"))
        {
            Console.WriteLine("overwriting windows: HAAHA!");
        }
        return Task.FromResult(0);
    }
    public string[] GetAllowedOptionGroups() => ["doit"];
}


[Cumber.HelpSystem.CommandHandler("doit all")]
class DoitAll : Cumber.HelpSystem.ICommandHandler
{
    public Task<int> ExecuteAsync(string[] args, IOptionAccessor optionAccessor)
    {
        Console.WriteLine($"In the doitALL command args are {string.Join(",", args)}");

        Console.WriteLine($"non-options are {string.Join(",", optionAccessor.NonOptions)}");
        Console.WriteLine($"illegal options are {string.Join(",", optionAccessor.IllegalOptions)}");
        if (optionAccessor.IsOptionPresent("pause"))
        {
            Console.WriteLine("pausing the system");
        }
        else if (optionAccessor.IsOptionPresent("write"))
        {
            Console.WriteLine("overwriting windows: HAAHA!");
        }
        return Task.FromResult(0);
    }
    public string[] GetAllowedOptionGroups() => ["doit"];
}


[Cumber.HelpSystem.CommandHandler("doit partly")]
class DoitAllPartly : Cumber.HelpSystem.ICommandHandler
{
    public Task<int> ExecuteAsync(string[] args, IOptionAccessor optionAccessor)
    {
        Console.WriteLine($"In the doitPartly command args are {string.Join(",", args)}");

        Console.WriteLine($"non-options are {string.Join(",", optionAccessor.NonOptions)}");
        Console.WriteLine($"illegal options are {string.Join(",", optionAccessor.IllegalOptions)}");
        if (optionAccessor.IsOptionPresent("pause"))
        {
            Console.WriteLine("pausing the system");
        }
        else if (optionAccessor.IsOptionPresent("write"))
        {
            Console.WriteLine("overwriting windows: HAAHA!");
        }
        return Task.FromResult(0);
    }
    public string[] GetAllowedOptionGroups() => ["doit"];
}
