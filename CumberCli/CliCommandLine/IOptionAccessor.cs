

namespace Cumber.CommandLine
{
    public interface IOptionAccessor
    {
        IReadOnlyCollection<NonOption> NonOptions { get; }
        IReadOnlyCollection<IllegalOption> IllegalOptions { get; }

        bool IsOptionPresent<T>(T name);
        bool TryGetAllParams<T>(T name, out List<string> allValues);
        bool TryGetParam<T>(T name, out string? value, int offset = 0);
        bool TryGetParams<T>(T name, out List<string> values, int offset = 0);
    }
}