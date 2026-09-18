using AwesomeAssertions;
using CreativeCoders.MacOS.HomeBrew.Tests.TestHelpers;
using CreativeCoders.ProcessUtils.Execution;
using FakeItEasy;

namespace CreativeCoders.MacOS.HomeBrew.Tests;

public class BrewUpdaterTests
{
    /// <summary>
    /// Verifies that the updater invokes <c>brew update</c> and not one of the other brew
    /// subcommands, because the command name is baked into the executor at construction time.
    /// </summary>
    [Fact]
    public void Ctor_ConfiguresBrewUpdateCommand()
    {
        // Arrange
        var builder = FakeProcessExecutorBuilder.Create<string>(out _);

        // Act
        _ = new BrewUpdater(builder);

        // Assert
        A.CallTo(() => builder.SetFileName("brew")).MustHaveHappenedOnceExactly();
        A.CallTo(() => builder.SetArguments(A<string[]>.That
                .IsSameSequenceAs("update", "{{force}}")))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task UpdateAsync_WithoutForce_CallsExecutorWithEmptyForceArgument()
    {
        // Arrange
        var builder = FakeProcessExecutorBuilder.Create<string>(out var executor);
        var sut = new BrewUpdater(builder);

        // Act
        await sut.UpdateAsync();

        // Assert
        A.CallTo(() => executor.ExecuteAsync(A<IDictionary<string, object?>>.That
                .Matches(d => (string?)d["force"] == "")))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task UpdateAsync_WithForce_SetsForceFlag()
    {
        // Arrange
        var builder = FakeProcessExecutorBuilder.Create<string>(out var executor);
        var sut = new BrewUpdater(builder);

        // Act
        await sut.UpdateAsync(force: true);

        // Assert
        A.CallTo(() => executor.ExecuteAsync(A<IDictionary<string, object?>>.That
                .Matches(d => (string?)d["force"] == "-f")))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task UpdateAsync_WhenExecutionFails_ThrowsBrewUpdateException()
    {
        // Arrange
        var builder = FakeProcessExecutorBuilder.Create<string>(out var executor);
        A.CallTo(() => executor.ExecuteAsync(A<IDictionary<string, object?>>._))
            .Throws(new ProcessExecutionFailedException(9, "update failed", "std"));
        var sut = new BrewUpdater(builder);

        // Act
        var act = () => sut.UpdateAsync();

        // Assert
        var ex = await act.Should().ThrowAsync<BrewUpdateException>();
        ex.Which.ErrorOutput.Should().Be("update failed");
        ex.Which.ExitCode.Should().Be(9);
    }

    /// <summary>
    /// Verifies that only <see cref="ProcessExecutionFailedException"/> is translated into a
    /// <see cref="BrewUpdateException"/>; any other failure must surface unchanged.
    /// </summary>
    [Fact]
    public async Task UpdateAsync_WhenExecutorThrowsUnrelatedException_DoesNotWrapIt()
    {
        // Arrange
        var builder = FakeProcessExecutorBuilder.Create<string>(out var executor);
        A.CallTo(() => executor.ExecuteAsync(A<IDictionary<string, object?>>._))
            .Throws(new InvalidOperationException("boom"));
        var sut = new BrewUpdater(builder);

        // Act
        var act = () => sut.UpdateAsync();

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}
