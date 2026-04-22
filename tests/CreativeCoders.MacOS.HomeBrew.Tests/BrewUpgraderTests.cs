using AwesomeAssertions;
using CreativeCoders.MacOS.HomeBrew.Tests.TestHelpers;
using CreativeCoders.ProcessUtils.Execution;
using FakeItEasy;

namespace CreativeCoders.MacOS.HomeBrew.Tests;

public class BrewUpgraderTests
{
    [Fact]
    public async Task UpgradeAsync_WithoutForce_CallsExecutorWithEmptyArgs()
    {
        // Arrange
        var builder = FakeProcessExecutorBuilder.Create<string>(out var executor);
        var sut = new BrewUpgrader(builder);

        // Act
        await sut.UpgradeAsync();

        // Assert
        A.CallTo(() => executor.ExecuteAsync(A<IDictionary<string, object?>>.That
                .Matches(d => (string?)d["appName"] == "" && (string?)d["force"] == "")))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task UpgradeAsync_WithForce_SetsForceFlag()
    {
        // Arrange
        var builder = FakeProcessExecutorBuilder.Create<string>(out var executor);
        var sut = new BrewUpgrader(builder);

        // Act
        await sut.UpgradeAsync(force: true);

        // Assert
        A.CallTo(() => executor.ExecuteAsync(A<IDictionary<string, object?>>.That
                .Matches(d => (string?)d["force"] == "-f")))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task UpgradeAsync_WhenExecutionFails_ThrowsBrewUpgradeException()
    {
        // Arrange
        var builder = FakeProcessExecutorBuilder.Create<string>(out var executor);
        A.CallTo(() => executor.ExecuteAsync(A<IDictionary<string, object?>>._))
            .Throws(new ProcessExecutionFailedException(5, "err", "std"));
        var sut = new BrewUpgrader(builder);

        // Act
        var act = () => sut.UpgradeAsync();

        // Assert
        var ex = await act.Should().ThrowAsync<BrewUpgradeException>();
        ex.Which.Should().NotBeOfType<BrewUpgradeFailedException>();
        ex.Which.ErrorOutput.Should().Be("err");
        ex.Which.ExitCode.Should().Be(5);
    }

    [Fact]
    public async Task UpgradeSoftwareAsync_PassesAppNameToExecutor()
    {
        // Arrange
        var builder = FakeProcessExecutorBuilder.Create<string>(out var executor);
        var sut = new BrewUpgrader(builder);

        // Act
        await sut.UpgradeSoftwareAsync("wget");

        // Assert
        A.CallTo(() => executor.ExecuteAsync(A<IDictionary<string, object?>>.That
                .Matches(d => (string?)d["appName"] == "wget" && (string?)d["force"] == "")))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task UpgradeSoftwareAsync_WithForce_SetsForceFlag()
    {
        // Arrange
        var builder = FakeProcessExecutorBuilder.Create<string>(out var executor);
        var sut = new BrewUpgrader(builder);

        // Act
        await sut.UpgradeSoftwareAsync("wget", force: true);

        // Assert
        A.CallTo(() => executor.ExecuteAsync(A<IDictionary<string, object?>>.That
                .Matches(d => (string?)d["appName"] == "wget" && (string?)d["force"] == "-f")))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task UpgradeSoftwareAsync_WhenExecutionFails_ThrowsBrewUpgradeFailedException()
    {
        // Arrange
        var builder = FakeProcessExecutorBuilder.Create<string>(out var executor);
        A.CallTo(() => executor.ExecuteAsync(A<IDictionary<string, object?>>._))
            .Throws(new ProcessExecutionFailedException(7, "oops", "std"));
        var sut = new BrewUpgrader(builder);

        // Act
        var act = () => sut.UpgradeSoftwareAsync("wget");

        // Assert
        var ex = await act.Should().ThrowAsync<BrewUpgradeFailedException>();
        ex.Which.AppName.Should().Be("wget");
        ex.Which.ErrorOutput.Should().Be("oops");
        ex.Which.ExitCode.Should().Be(7);
        ex.Which.Message.Should().Contain("wget");
    }

    [Fact]
    public void Ctor_WhenBuilderIsNull_Throws()
    {
        // Arrange + Act
        var act = () => new BrewUpgrader(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }
}
