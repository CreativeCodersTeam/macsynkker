using AwesomeAssertions;
using CreativeCoders.MacOS.HomeBrew.Cleanup;

namespace CreativeCoders.MacOS.HomeBrew.Tests.Cleanup;

public class BrewPruneOptionTests
{
    [Fact]
    public void All_ProducesPruneAllArgument()
    {
        BrewPruneOption.All.ToCommandLineArgument().Should().Be("--prune=all");
    }

    [Theory]
    [InlineData(0, "--prune=0")]
    [InlineData(7, "--prune=7")]
    [InlineData(365, "--prune=365")]
    public void Days_ProducesExpectedArgument(int days, string expected)
    {
        BrewPruneOption.Days(days).ToCommandLineArgument().Should().Be(expected);
    }

    [Fact]
    public void Days_WhenNegative_Throws()
    {
        var act = () => BrewPruneOption.Days(-1);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
