namespace Cumber.CliOption;
public record Option(char? ShortOption, string? LongOption, int MaxOccurs, string? Description, List<ParameterSpec> Parameters)
{
    public int ParameterCount => Parameters?.Count ?? 0;
}
