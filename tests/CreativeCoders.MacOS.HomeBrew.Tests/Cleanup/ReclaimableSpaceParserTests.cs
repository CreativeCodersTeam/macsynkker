using AwesomeAssertions;
using CreativeCoders.MacOS.HomeBrew.Cleanup;

namespace CreativeCoders.MacOS.HomeBrew.Tests.Cleanup;

public class ReclaimableSpaceParserTests
{
    [Fact]
    public void ParseBytes_WithNullOrWhitespace_ReturnsZero()
    {
        ReclaimableSpaceParser.ParseBytes(null).Should().Be(0);
        ReclaimableSpaceParser.ParseBytes("").Should().Be(0);
        ReclaimableSpaceParser.ParseBytes("   ").Should().Be(0);
    }

    [Fact]
    public void ParseBytes_WithoutSize_ReturnsZero()
    {
        ReclaimableSpaceParser.ParseBytes("Nothing to clean.").Should().Be(0);
    }

    [Theory]
    [InlineData("This operation would free approximately 512B of disk space.", 512L)]
    [InlineData("This operation would free approximately 2KB of disk space.", 2L * 1024)]
    [InlineData("This operation would free approximately 5MB of disk space.", 5L * 1024 * 1024)]
    [InlineData("This operation would free approximately 1GB of disk space.", 1L * 1024 * 1024 * 1024)]
    public void ParseBytes_ParsesSimpleUnits(string output, long expected)
    {
        ReclaimableSpaceParser.ParseBytes(output).Should().Be(expected);
    }

    [Fact]
    public void ParseBytes_ParsesDecimalGigabytes()
    {
        var bytes = ReclaimableSpaceParser.ParseBytes(
            "This operation would free approximately 1.5GB of disk space.");

        bytes.Should().Be((long)(1.5 * 1024 * 1024 * 1024));
    }

    [Fact]
    public void ParseBytes_PicksLargestSizeInMultilineOutput()
    {
        var output = """
                     Removing: /tmp/file (123KB)
                     Removing: /tmp/other (2MB)
                     This operation would free approximately 2MB of disk space.
                     """;

        var bytes = ReclaimableSpaceParser.ParseBytes(output);

        bytes.Should().Be(2L * 1024 * 1024);
    }
}
