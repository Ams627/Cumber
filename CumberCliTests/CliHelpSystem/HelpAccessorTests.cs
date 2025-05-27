using FluentAssertions;
using System.Diagnostics;

namespace Cumber.HelpSystem.Tests;


[TestClass]
public class HelpAccessorTests
{
    static string HelpText = """
        This is some help text for a big and complex tool with lots of big and complicated commands.
        The lines before a line starting with equals should mainly be ignored. These lines are for comments
        and general remarks.

        So we can write a poem here if we want.

        The one thing that is interpreted in this section is option group definition.

        = animal

        Do something to an animal

        == remove - remove some part of the animal
        This command removes a part of the animal

        === tail
        This command removes the tail of the animal

        Options:
            -n, --nokill - do not kill the animal first

        === legs

        ==== front
        Remove the front legs of the animal before cooking! This is done so that the animal fits over the barbecue pit.

        ==== back

        = vegetable

        == peel

        == chop

        """;

    [TestMethod()]
    public void CreateTest()
    {
        Assert.Fail();
    }

    [TestMethod]
    public void GetLongestMatchingCommandTest()
    {
        string argStr = "remove legs with a long sword";
        string[] args = argStr.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        var helpAccessor = HelpAccessor.Create(HelpText);
        var completeCommand = helpAccessor.GetLongestMatchingCommand("animal", args, out int indexAfterMatch);
        completeCommand.Should().BeTrue();
        indexAfterMatch.Should().Be(2);
    }

    [TestMethod()]
    public void GetHelpTextForCommandTest()
    {
        string argStr = "remove tail with a knife";
        string[] args = argStr.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        var helpAccessor = HelpAccessor.Create(HelpText);
        var completeCommand = helpAccessor.GetHelpTextForCommand("animal", args, 2, out string text);
        Trace.WriteLine(text);
    }

    [TestMethod()]
    public void GetHelpTextWithOptionsTest()
    {

    }

    [TestMethod()]
    public void DumpCommandTreeTest()
    {
        Assert.Fail();
    }

    [TestMethod()]
    public void DumpAllHelpTest()
    {
        Assert.Fail();
    }
}