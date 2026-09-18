using AwesomeAssertions;
using CreativeCoders.MacOS.HomeBrew.Tests.TestHelpers;
using CreativeCoders.ProcessUtils.Execution;
using FakeItEasy;

namespace CreativeCoders.MacOS.HomeBrew.Tests;

public class BrewUpgraderTests
{
    /// <summary>
    /// Verifies that the upgrader invokes <c>brew upgrade</c> with the expected placeholders,
    /// because the command and its argument template are baked into the executor at construction
    /// time and are therefore invisible to the other tests.
    /// </summary>
    [Fact]
    public void Ctor_ConfiguresBrewUpgradeCommand()
    {
        // Arrange
        var builder = FakeProcessExecutorBuilder.Create<string>(out _);

        // Act
        _ = new BrewUpgrader(builder);

        // Assert
        A.CallTo(() => builder.SetFileName("brew")).MustHaveHappenedOnceExactly();
        A.CallTo(() => builder.SetArguments(A<string[]>.That
                .IsSameSequenceAs("upgrade", "{{appName}}", "{{force}}", "{{askForConfirmation}}")))
            .MustHaveHappenedOnceExactly();
    }

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

    /// <summary>
    /// Verifies the inverted confirmation logic: leaving <c>askForConfirmation</c> at its default
    /// passes <c>-y</c> so brew runs unattended.
    /// </summary>
    [Fact]
    public async Task UpgradeAsync_WithoutAskForConfirmation_SetsAutoConfirmFlag()
    {
        // Arrange
        var builder = FakeProcessExecutorBuilder.Create<string>(out var executor);
        var sut = new BrewUpgrader(builder);

        // Act
        await sut.UpgradeAsync();

        // Assert
        A.CallTo(() => executor.ExecuteAsync(A<IDictionary<string, object?>>.That
                .Matches(d => (string?)d["askForConfirmation"] == "-y")))
            .MustHaveHappenedOnceExactly();
    }

    /// <summary>
    /// Verifies the inverted confirmation logic: requesting confirmation omits <c>-y</c> so brew
    /// prompts the user.
    /// </summary>
    [Fact]
    public async Task UpgradeAsync_WithAskForConfirmation_OmitsAutoConfirmFlag()
    {
        // Arrange
        var builder = FakeProcessExecutorBuilder.Create<string>(out var executor);
        var sut = new BrewUpgrader(builder);

        // Act
        await sut.UpgradeAsync(askForConfirmation: true);

        // Assert
        A.CallTo(() => executor.ExecuteAsync(A<IDictionary<string, object?>>.That
                .Matches(d => (string?)d["askForConfirmation"] == "")))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task UpgradeSoftwareAsync_WithoutAskForConfirmation_SetsAutoConfirmFlag()
    {
        // Arrange
        var builder = FakeProcessExecutorBuilder.Create<string>(out var executor);
        var sut = new BrewUpgrader(builder);

        // Act
        await sut.UpgradeSoftwareAsync("wget");

        // Assert
        A.CallTo(() => executor.ExecuteAsync(A<IDictionary<string, object?>>.That
                .Matches(d => (string?)d["askForConfirmation"] == "-y")))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task UpgradeSoftwareAsync_WithAskForConfirmation_OmitsAutoConfirmFlag()
    {
        // Arrange
        var builder = FakeProcessExecutorBuilder.Create<string>(out var executor);
        var sut = new BrewUpgrader(builder);

        // Act
        await sut.UpgradeSoftwareAsync("wget", askForConfirmation: true);

        // Assert
        A.CallTo(() => executor.ExecuteAsync(A<IDictionary<string, object?>>.That
                .Matches(d => (string?)d["askForConfirmation"] == "")))
            .MustHaveHappenedOnceExactly();
    }

    /// <summary>
    /// Verifies that force and confirmation are independent switches and do not overwrite each
    /// other's placeholder.
    /// </summary>
    [Fact]
    public async Task UpgradeSoftwareAsync_WithForceAndAskForConfirmation_SetsBothArgumentsIndependently()
    {
        // Arrange
        var builder = FakeProcessExecutorBuilder.Create<string>(out var executor);
        var sut = new BrewUpgrader(builder);

        // Act
        await sut.UpgradeSoftwareAsync("wget", force: true, askForConfirmation: true);

        // Assert
        A.CallTo(() => executor.ExecuteAsync(A<IDictionary<string, object?>>.That
                .Matches(d => (string?)d["appName"] == "wget"
                              && (string?)d["force"] == "-f"
                              && (string?)d["askForConfirmation"] == "")))
            .MustHaveHappenedOnceExactly();
    }

    /// <summary>
    /// Verifies that only <see cref="ProcessExecutionFailedException"/> is translated into a
    /// <see cref="BrewUpgradeException"/>; any other failure must surface unchanged.
    /// </summary>
    [Fact]
    public async Task UpgradeAsync_WhenExecutorThrowsUnrelatedException_DoesNotWrapIt()
    {
        // Arrange
        var builder = FakeProcessExecutorBuilder.Create<string>(out var executor);
        A.CallTo(() => executor.ExecuteAsync(A<IDictionary<string, object?>>._))
            .Throws(new InvalidOperationException("boom"));
        var sut = new BrewUpgrader(builder);

        // Act
        var act = () => sut.UpgradeAsync();

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    /// <summary>
    /// Verifies that only <see cref="ProcessExecutionFailedException"/> is translated into a
    /// <see cref="BrewUpgradeFailedException"/>; any other failure must surface unchanged.
    /// </summary>
    [Fact]
    public async Task UpgradeSoftwareAsync_WhenExecutorThrowsUnrelatedException_DoesNotWrapIt()
    {
        // Arrange
        var builder = FakeProcessExecutorBuilder.Create<string>(out var executor);
        A.CallTo(() => executor.ExecuteAsync(A<IDictionary<string, object?>>._))
            .Throws(new InvalidOperationException("boom"));
        var sut = new BrewUpgrader(builder);

        // Act
        var act = () => sut.UpgradeSoftwareAsync("wget");

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    /// <summary>
    /// Verifies that both methods share one executor without leaking state: a global upgrade
    /// following a single-package upgrade must not re-use the previous package name.
    /// </summary>
    [Fact]
    public async Task UpgradeAsync_AfterUpgradeSoftwareAsync_DoesNotReuseAppName()
    {
        // Arrange
        var builder = FakeProcessExecutorBuilder.Create<string>(out var executor);
        var sut = new BrewUpgrader(builder);

        // Act
        await sut.UpgradeSoftwareAsync("wget");
        await sut.UpgradeAsync();

        // Assert
        A.CallTo(() => executor.ExecuteAsync(A<IDictionary<string, object?>>.That
                .Matches(d => (string?)d["appName"] == "")))
            .MustHaveHappenedOnceExactly();
    }

    /// <summary>
    /// Verifies that a missing package name is rejected instead of degrading into a global
    /// <c>brew upgrade</c> of every installed package.
    /// </summary>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task UpgradeSoftwareAsync_WhenAppNameIsNullOrWhitespace_ThrowsWithoutInvokingExecutor(
        string? appName)
    {
        // Arrange
        var builder = FakeProcessExecutorBuilder.Create<string>(out var executor);
        var sut = new BrewUpgrader(builder);

        // Act
        var act = () => sut.UpgradeSoftwareAsync(appName!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
        A.CallTo(() => executor.ExecuteAsync(A<IDictionary<string, object?>>._)).MustNotHaveHappened();
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
