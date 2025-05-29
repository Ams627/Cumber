namespace Cumber.HelpSystem;

using Cumber.CommandLine;
using System.Threading.Tasks;

public interface ICommandHandler
{
    Task<int> ExecuteAsync(string[] args, IOptionAccessor optionAccessor);
}
