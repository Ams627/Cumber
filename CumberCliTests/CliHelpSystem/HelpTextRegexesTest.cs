using FluentAssertions;

namespace Cumber.HelpSystem.Tests;

[TestClass]
public class HelpTextRegexesTest
{
    [TestMethod]
    public void SectionHeaderTest()
    {
        var re = HelpTextRegexes.HeaderRegex;

        var input = "= Hello - this is a description of the hello command!   ";
        var match = re.Match(input);
        match.Success.Should().BeTrue();

        var equalsGroup = match.Groups["headerIntroducer"];
        equalsGroup.Success.Should().BeTrue();
        equalsGroup.Value.Should().Be("=");

        var commandGroup = match.Groups["commandName"];
        commandGroup.Success.Should().BeTrue();
        commandGroup.Value.Should().Be("Hello");

        var commandSummary = match.Groups["commandSummary"];
        commandSummary.Success.Should().BeTrue();
        commandSummary.Value.Should().Be("this is a description of the hello command!");
    }

    [TestMethod]
    public void SectionHeaderTest_JustADash()
    {
        var re = HelpTextRegexes.HeaderRegex;

        var input = "= Hello -";
        var match = re.Match(input);
        match.Success.Should().BeTrue();

        var equalsGroup = match.Groups["headerIntroducer"];
        equalsGroup.Success.Should().BeTrue();
        equalsGroup.Value.Should().Be("=");

        var commandGroup = match.Groups["commandName"];
        commandGroup.Success.Should().BeTrue();
        commandGroup.Value.Should().Be("Hello");

        var commandSummary = match.Groups["commandSummary"];
        commandSummary.Success.Should().BeFalse();

        var emptysummary = match.Groups["emptysummary"];
        emptysummary.Success.Should().BeTrue();
    }

}
