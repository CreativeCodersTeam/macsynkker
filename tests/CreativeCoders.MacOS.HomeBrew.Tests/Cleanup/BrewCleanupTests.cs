using AwesomeAssertions;
using CreativeCoders.MacOS.HomeBrew.Cleanup;
using CreativeCoders.MacOS.HomeBrew.Tests.TestHelpers;
using CreativeCoders.ProcessUtils.Execution;
using FakeItEasy;

namespace CreativeCoders.MacOS.HomeBrew.Tests.Cleanup;

public class BrewCleanupTests
{
    [Fact]
    public async Task CleanupAsync_WithoutOptions_PassesEmptyPlaceholders()
    {
        var builder = FakeProcessExecutorBuilder.Create<string>(out var executor);
        var sut = new BrewCleanup(builder);

        await sut.CleanupAsync();

        A.CallTo(() => executor.ExecuteAsync(A<IDictionary<string, object?>>.That
                .Matches(d => (string?)d["prune"] == "" && (string?)d["dryRun"] == "")))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task CleanupAsync_WithPruneAll_SetsPruneAllArgument()
    {
        var builder = FakeProcessExecutorBuilder.Create<string>(out var executor);
        var sut = new BrewCleanup(builder);

        await sut.CleanupAsync(new BrewCleanupOptions { Prune = BrewPruneOption.All });

        A.CallTo(() => executor.ExecuteAsync(A<IDictionary<string, object?>>.That
                .Matches(d => (string?)d["prune"] == "--prune=all" && (string?)d["dryRun"] == "")))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task CleanupAsync_WithPruneDays_SetsPruneDaysArgument()
    {
        var builder = FakeProcessExecutorBuilder.Create<string>(out var executor);
        var sut = new BrewCleanup(builder);

        await sut.CleanupAsync(new BrewCleanupOptions { Prune = BrewPruneOption.Days(14) });

        A.CallTo(() => executor.ExecuteAsync(A<IDictionary<string, object?>>.That
                .Matches(d => (string?)d["prune"] == "--prune=14")))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task CleanupAsync_WhenExecutionFails_ThrowsBrewCleanupFailedException()
    {
        var builder = FakeProcessExecutorBuilder.Create<string>(out var executor);
        A.CallTo(() => executor.ExecuteAsync(A<IDictionary<string, object?>>._))
            .Throws(new ProcessExecutionFailedException(3, "boom", "std"));
        var sut = new BrewCleanup(builder);

        var act = () => sut.CleanupAsync();

        var ex = await act.Should().ThrowAsync<BrewCleanupFailedException>();
        ex.Which.ErrorOutput.Should().Be("boom");
        ex.Which.ExitCode.Should().Be(3);
    }

    [Fact]
    public async Task GetReclaimableSpaceAsync_SetsDryRunAndParsesOutput()
    {
        var builder = FakeProcessExecutorBuilder.Create<string>(out var executor);
        A.CallTo(() => executor.ExecuteAsync(A<IDictionary<string, object?>>._))
            .Returns(Task.FromResult<string?>("This operation would free approximately 3MB of disk space."));
        var sut = new BrewCleanup(builder);

        var bytes = await sut.GetReclaimableSpaceAsync(
            new BrewCleanupOptions { Prune = BrewPruneOption.Days(7) });

        bytes.Should().Be(3L * 1024 * 1024);
        A.CallTo(() => executor.ExecuteAsync(A<IDictionary<string, object?>>.That
                .Matches(d => (string?)d["prune"] == "--prune=7" && (string?)d["dryRun"] == "--dry-run")))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task GetReclaimableSpaceAsync_WithoutSizeInOutput_ReturnsZero()
    {
        var builder = FakeProcessExecutorBuilder.Create<string>(out var executor);
        A.CallTo(() => executor.ExecuteAsync(A<IDictionary<string, object?>>._))
            .Returns(Task.FromResult<string?>("Nothing to clean."));
        var sut = new BrewCleanup(builder);

        var bytes = await sut.GetReclaimableSpaceAsync();

        bytes.Should().Be(0);
    }

    [Fact]
    public void Ctor_WhenBuilderIsNull_Throws()
    {
        var act = () => new BrewCleanup(null!);

        act.Should().Throw<ArgumentNullException>();
    }
}
