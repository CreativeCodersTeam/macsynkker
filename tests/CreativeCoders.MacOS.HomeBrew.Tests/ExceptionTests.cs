using AwesomeAssertions;
using CreativeCoders.MacOS.HomeBrew.Import;

namespace CreativeCoders.MacOS.HomeBrew.Tests;

public class ExceptionTests
{
    [Fact]
    public void BrewUpgradeException_StoresMessageErrorOutputAndExitCode()
    {
        // Arrange + Act
        var ex = new BrewUpgradeException("msg", "err", 42);

        // Assert
        ex.Message.Should().Be("msg");
        ex.ErrorOutput.Should().Be("err");
        ex.ExitCode.Should().Be(42);
    }

    [Fact]
    public void BrewUpgradeFailedException_FormatsMessageWithAppName()
    {
        // Arrange + Act
        var ex = new BrewUpgradeFailedException("wget", "err", 1);

        // Assert
        ex.AppName.Should().Be("wget");
        ex.Message.Should().Contain("wget");
        ex.ErrorOutput.Should().Be("err");
        ex.ExitCode.Should().Be(1);
        ex.Should().BeAssignableTo<BrewUpgradeException>();
    }

    [Theory]
    [InlineData(BrewInstallTargetKind.Tap, "tap")]
    [InlineData(BrewInstallTargetKind.Formula, "formula")]
    [InlineData(BrewInstallTargetKind.Cask, "cask")]
    public void BrewInstallFailedException_MessageContainsLowercaseKindAndTarget(
        BrewInstallTargetKind kind, string expectedKindText)
    {
        // Arrange + Act
        var ex = new BrewInstallFailedException(kind, "my-target", "err", 3);

        // Assert
        ex.Kind.Should().Be(kind);
        ex.Target.Should().Be("my-target");
        ex.ErrorOutput.Should().Be("err");
        ex.ExitCode.Should().Be(3);
        ex.Message.Should().Contain(expectedKindText);
        ex.Message.Should().Contain("my-target");
    }

    [Fact]
    public void BrewImportFailedException_AggregatesFailures()
    {
        // Arrange
        var failures = new[]
        {
            new BrewInstallFailedException(BrewInstallTargetKind.Formula, "a", "e", 1),
            new BrewInstallFailedException(BrewInstallTargetKind.Cask, "b", "e", 1)
        };

        // Act
        var ex = new BrewImportFailedException(failures);

        // Assert
        ex.Failures.Should().BeEquivalentTo(failures);
        ex.Message.Should().Contain("2");
    }
}
