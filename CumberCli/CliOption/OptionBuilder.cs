﻿using System.Text;

namespace Cumber.CliOption;
public class OptionBuilder
{
    private char? _shortOption;
    private string? _longOption;
    private int _maxOccurs = 1;
    private readonly List<ParameterSpec> _parameters = [];
    private readonly StringBuilder _descriptionBuilder = new StringBuilder();

    public OptionBuilder WithShortOption(char? c)
    {
        _shortOption = c;
        return this;
    }

    public OptionBuilder WithLongOption(string? s)
    {
        _longOption = s;
        return this;
    }

    public OptionBuilder WithMaxOccurs(int max)
    {
        _maxOccurs = max;
        return this;
    }

    public OptionBuilder AppendDescription(string? description)
    {
        _descriptionBuilder.Append(description);
        return this;
    }

    public OptionBuilder WithParameter(string name, string? type)
    {
        _parameters.Add(new ParameterSpec(name, type));
        return this;
    }

    public Option Build()
    {
        if (_shortOption == '\0' && string.IsNullOrWhiteSpace(_longOption))
            throw new InvalidOperationException("Must specify either short or long option.");

        if (_maxOccurs < 1)
            throw new InvalidOperationException("MaxOccurs must be at least 1.");

        return new Option(_shortOption, _longOption, _maxOccurs, _descriptionBuilder.ToString(), _parameters);
    }

    public OptionBuilder Reset()
    {
        _shortOption = '\0';
        _longOption = null;
        _maxOccurs = 1;
        return this;
    }
}

