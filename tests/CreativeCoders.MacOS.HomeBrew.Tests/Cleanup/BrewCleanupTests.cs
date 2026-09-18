using AwesomeAssertions;
using CreativeCoders.MacOS.HomeBrew.Cleanup;
using CreativeCoders.MacOS.HomeBrew.Tests.TestHelpers;
using CreativeCoders.ProcessUtils.Execution;
using FakeItEasy;

namespace CreativeCoders.MacOS.HomeBrew.Tests.Cleanup;

public class BrewCleanupTests
{
    /// <summary>
    /// Verifies that the cleanup invokes <c>brew cleanup</c> with the expected placeholders,
    /// because command and argument template are baked into the executor at construction time.
    /// </summary>
    [Fact]
    public void Ctor_ConfiguresBrewCleanupCommand()
    {
        var builder = FakeProcessExecutorBuilder.Create<string>(out _);

        _ = new BrewCleanup(builder);

        A.CallTo(() => builder.SetFileName("brew")).MustHaveHappenedOnceExactly();
        A.CallTo(() => builder.SetArguments(A<string[]>.That
                .IsSameSequenceAs("cleanup", "{{prune}}", "{{dryRun}}")))
            .MustHaveHappenedOnceExactly();
    }

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
    public async Task GetReclaimableSpaceDetailsAsync_SetsDryRunAndParsesItemsAndTotal()
    {
        var builder = FakeProcessExecutorBuilder.Create<string>(out var executor);
        A.CallTo(() => executor.ExecuteAsync(A<IDictionary<string, object?>>._))
            .Returns(Task.FromResult<string?>("""
                                              Removing: /tmp/a (1MB)
                                              Removing: /tmp/b (2MB)
                                              This operation would free approximately 3MB of disk space.
                                              """));
        var sut = new BrewCleanup(builder);

        var result = await sut.GetReclaimableSpaceDetailsAsync(
            new BrewCleanupOptions { Prune = BrewPruneOption.Days(7) });

        result.TotalBytes.Should().Be(3L * 1024 * 1024);
        result.Items.Should().HaveCount(2);
        result.Items[0].Path.Should().Be("/tmp/a");
        result.Items[0].SizeInBytes.Should().Be(1L * 1024 * 1024);
        result.Items[1].Path.Should().Be("/tmp/b");
        result.Items[1].SizeInBytes.Should().Be(2L * 1024 * 1024);
        A.CallTo(() => executor.ExecuteAsync(A<IDictionary<string, object?>>.That
                .Matches(d => (string?)d["prune"] == "--prune=7" && (string?)d["dryRun"] == "--dry-run")))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task GetReclaimableSpaceDetailsAsync_WithoutSizeInOutput_ReturnsEmptyResult()
    {
        var builder = FakeProcessExecutorBuilder.Create<string>(out var executor);
        A.CallTo(() => executor.ExecuteAsync(A<IDictionary<string, object?>>._))
            .Returns(Task.FromResult<string?>("Nothing to clean."));
        var sut = new BrewCleanup(builder);

        var result = await sut.GetReclaimableSpaceDetailsAsync();

        result.TotalBytes.Should().Be(0);
        result.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task GetReclaimableSpaceDetailsAsync_WhenExecutionFails_ThrowsBrewCleanupFailedException()
    {
        var builder = FakeProcessExecutorBuilder.Create<string>(out var executor);
        A.CallTo(() => executor.ExecuteAsync(A<IDictionary<string, object?>>._))
            .Throws(new ProcessExecutionFailedException(2, "fail", "out"));
        var sut = new BrewCleanup(builder);

        var act = () => sut.GetReclaimableSpaceDetailsAsync();

        var ex = await act.Should().ThrowAsync<BrewCleanupFailedException>();
        ex.Which.ExitCode.Should().Be(2);
    }

    [Fact]
    public async Task GetReclaimableSpaceAsync_WhenExecutionFails_ThrowsBrewCleanupFailedException()
    {
        var builder = FakeProcessExecutorBuilder.Create<string>(out var executor);
        A.CallTo(() => executor.ExecuteAsync(A<IDictionary<string, object?>>._))
            .Throws(new ProcessExecutionFailedException(4, "dry run failed", "std"));
        var sut = new BrewCleanup(builder);

        var act = () => sut.GetReclaimableSpaceAsync();

        var ex = await act.Should().ThrowAsync<BrewCleanupFailedException>();
        ex.Which.ErrorOutput.Should().Be("dry run failed");
        ex.Which.ExitCode.Should().Be(4);
    }

    /// <summary>
    /// Verifies that the original <see cref="ProcessExecutionFailedException"/> stays reachable as
    /// inner exception so the full process failure remains diagnosable.
    /// </summary>
    [Fact]
    public async Task CleanupAsync_WhenExecutionFails_KeepsProcessExceptionAsInnerException()
    {
        var builder = FakeProcessExecutorBuilder.Create<string>(out var executor);
        var processException = new ProcessExecutionFailedException(3, "boom", "std");
        A.CallTo(() => executor.ExecuteAsync(A<IDictionary<string, object?>>._)).Throws(processException);
        var sut = new BrewCleanup(builder);

        var act = () => sut.CleanupAsync();

        var ex = await act.Should().ThrowAsync<BrewCleanupFailedException>();
        ex.Which.InnerException.Should().BeSameAs(processException);
    }

    [Fact]
    public async Task GetReclaimableSpaceDetailsAsync_WithPruneAll_SetsPruneAndDryRunArguments()
    {
        var builder = FakeProcessExecutorBuilder.Create<string>(out var executor);
        A.CallTo(() => executor.ExecuteAsync(A<IDictionary<string, object?>>._))
            .Returns(Task.FromResult<string?>("Nothing to clean."));
        var sut = new BrewCleanup(builder);

        await sut.GetReclaimableSpaceDetailsAsync(new BrewCleanupOptions { Prune = BrewPruneOption.All });

        A.CallTo(() => executor.ExecuteAsync(A<IDictionary<string, object?>>.That
                .Matches(d => (string?)d["prune"] == "--prune=all" && (string?)d["dryRun"] == "--dry-run")))
            .MustHaveHappenedOnceExactly();
    }

    /// <summary>
    /// Covers the branch where options are supplied but <c>Prune</c> is left unset, which is
    /// distinct from passing no options at all.
    /// </summary>
    [Fact]
    public async Task CleanupAsync_WithOptionsButWithoutPrune_PassesEmptyPruneArgument()
    {
        var builder = FakeProcessExecutorBuilder.Create<string>(out var executor);
        var sut = new BrewCleanup(builder);

        await sut.CleanupAsync(new BrewCleanupOptions());

        A.CallTo(() => executor.ExecuteAsync(A<IDictionary<string, object?>>.That
                .Matches(d => (string?)d["prune"] == "" && (string?)d["dryRun"] == "")))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task GetReclaimableSpaceDetailsAsync_WhenOutputIsNull_ReturnsEmptyResult()
    {
        var builder = FakeProcessExecutorBuilder.Create<string>(out var executor);
        A.CallTo(() => executor.ExecuteAsync(A<IDictionary<string, object?>>._))
            .Returns(Task.FromResult<string?>(null));
        var sut = new BrewCleanup(builder);

        var result = await sut.GetReclaimableSpaceDetailsAsync();

        result.TotalBytes.Should().Be(0);
        result.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task GetReclaimableSpaceAsync_WhenOutputIsNull_ReturnsZero()
    {
        var builder = FakeProcessExecutorBuilder.Create<string>(out var executor);
        A.CallTo(() => executor.ExecuteAsync(A<IDictionary<string, object?>>._))
            .Returns(Task.FromResult<string?>(null));
        var sut = new BrewCleanup(builder);

        var bytes = await sut.GetReclaimableSpaceAsync();

        bytes.Should().Be(0);
    }

    /// <summary>
    /// Verifies that only <see cref="ProcessExecutionFailedException"/> is translated into a
    /// <see cref="BrewCleanupFailedException"/>; any other failure must surface unchanged.
    /// </summary>
    [Fact]
    public async Task CleanupAsync_WhenExecutorThrowsUnrelatedException_DoesNotWrapIt()
    {
        var builder = FakeProcessExecutorBuilder.Create<string>(out var executor);
        A.CallTo(() => executor.ExecuteAsync(A<IDictionary<string, object?>>._))
            .Throws(new InvalidOperationException("boom"));
        var sut = new BrewCleanup(builder);

        var act = () => sut.CleanupAsync();

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    /// <summary>
    /// Verifies that a preceding dry run does not leave <c>--dry-run</c> in effect, so the
    /// subsequent real cleanup actually deletes.
    /// </summary>
    [Fact]
    public async Task CleanupAsync_AfterDryRun_DoesNotKeepDryRunArgument()
    {
        var builder = FakeProcessExecutorBuilder.Create<string>(out var executor);
        A.CallTo(() => executor.ExecuteAsync(A<IDictionary<string, object?>>._))
            .Returns(Task.FromResult<string?>("Nothing to clean."));
        var sut = new BrewCleanup(builder);

        await sut.GetReclaimableSpaceDetailsAsync();
        await sut.CleanupAsync();

        A.CallTo(() => executor.ExecuteAsync(A<IDictionary<string, object?>>.That
                .Matches(d => (string?)d["dryRun"] == "")))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void Ctor_WhenBuilderIsNull_Throws()
    {
        var act = () => new BrewCleanup(null!);

        act.Should().Throw<ArgumentNullException>();
    }
}
