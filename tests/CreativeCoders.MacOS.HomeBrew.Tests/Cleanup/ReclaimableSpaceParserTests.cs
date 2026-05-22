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

    [Fact]
    public void Parse_WithNullOrWhitespace_ReturnsEmptyResult()
    {
        var result = ReclaimableSpaceParser.Parse(null);

        result.TotalBytes.Should().Be(0);
        result.Items.Should().BeEmpty();
    }

    [Fact]
    public void Parse_WithRemovingLines_ReturnsItemsAndTotal()
    {
        var output = """
                     Removing: /Users/foo/Library/Caches/Homebrew/bar--1.0.tar.gz (1.5MB)
                     Removing: /Users/foo/Library/Caches/Homebrew/baz--2.0.tar.gz (512KB)
                     This operation would free approximately 2MB of disk space.
                     """;

        var result = ReclaimableSpaceParser.Parse(output);

        result.TotalBytes.Should().Be(2L * 1024 * 1024);
        result.Items.Should().HaveCount(2);
        result.Items[0].Path.Should().Be("/Users/foo/Library/Caches/Homebrew/bar--1.0.tar.gz");
        result.Items[0].SizeInBytes.Should().Be((long)(1.5 * 1024 * 1024));
        result.Items[1].Path.Should().Be("/Users/foo/Library/Caches/Homebrew/baz--2.0.tar.gz");
        result.Items[1].SizeInBytes.Should().Be(512L * 1024);
    }

    [Fact]
    public void Parse_WithWouldRemovePrefix_RecognizesItems()
    {
        var output = """
                     Would remove: /tmp/a (1KB)
                     Would remove: /tmp/b (2KB)
                     This operation would free approximately 3KB of disk space.
                     """;

        var result = ReclaimableSpaceParser.Parse(output);

        result.TotalBytes.Should().Be(3L * 1024);
        result.Items.Should().HaveCount(2);
        result.Items.Select(i => i.Path).Should().ContainInOrder("/tmp/a", "/tmp/b");
    }

    [Fact]
    public void Parse_WithoutTotalLine_FallsBackToSumOfItems()
    {
        var output = """
                     Removing: /tmp/a (1KB)
                     Removing: /tmp/b (2KB)
                     """;

        var result = ReclaimableSpaceParser.Parse(output);

        result.TotalBytes.Should().Be(3L * 1024);
        result.Items.Should().HaveCount(2);
    }

    [Fact]
    public void Parse_WithMixedUnits_ParsesAllItems()
    {
        var output = """
                     Removing: /tmp/a (512B)
                     Removing: /tmp/b (1.5MB)
                     Removing: /tmp/c (2GB)
                     """;

        var result = ReclaimableSpaceParser.Parse(output);

        result.Items.Should().HaveCount(3);
        result.Items[0].SizeInBytes.Should().Be(512L);
        result.Items[1].SizeInBytes.Should().Be((long)(1.5 * 1024 * 1024));
        result.Items[2].SizeInBytes.Should().Be(2L * 1024 * 1024 * 1024);
    }

    [Fact]
    public void Parse_WithoutItems_ReturnsEmptyItemsAndTotalFromApproximateLine()
    {
        var result = ReclaimableSpaceParser.Parse(
            "This operation would free approximately 5MB of disk space.");

        result.TotalBytes.Should().Be(5L * 1024 * 1024);
        result.Items.Should().BeEmpty();
    }
}
