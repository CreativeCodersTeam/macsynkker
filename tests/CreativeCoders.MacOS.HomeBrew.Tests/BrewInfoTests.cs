using System.ComponentModel;
using AwesomeAssertions;
using CreativeCoders.MacOS.HomeBrew.Tests.TestHelpers;
using CreativeCoders.ProcessUtils.Execution;
using FakeItEasy;

namespace CreativeCoders.MacOS.HomeBrew.Tests;

public class BrewInfoTests
{
    /// <summary>
    /// Verifies that the info service invokes <c>brew --version</c>, because command and
    /// arguments are baked into the executor at construction time.
    /// </summary>
    [Fact]
    public void Ctor_ConfiguresBrewVersionCommand()
    {
        // Arrange
        var builder = FakeProcessExecutorBuilder.Create<string>(out _);

        // Act
        _ = new BrewInfo(builder);

        // Assert
        A.CallTo(() => builder.SetFileName("brew")).MustHaveHappenedOnceExactly();
        A.CallTo(() => builder.SetArguments(A<string[]>.That.IsSameSequenceAs("--version")))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task IsInstalledAsync_WhenOutputStartsWithHomebrew_ReturnsTrue()
    {
        // Arrange
        var builder = FakeProcessExecutorBuilder.Create<string>(out var executor);
        A.CallTo(() => executor.ExecuteAsync()).Returns("Homebrew 4.3.1\nHomebrew/homebrew-core (git revision abc)");
        var sut = new BrewInfo(builder);

        // Act
        var result = await sut.IsInstalledAsync();

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("not brew")]
    [InlineData("")]
    public async Task IsInstalledAsync_WhenOutputDoesNotStartWithHomebrew_ReturnsFalse(string output)
    {
        // Arrange
        var builder = FakeProcessExecutorBuilder.Create<string>(out var executor);
        A.CallTo(() => executor.ExecuteAsync()).Returns(output);
        var sut = new BrewInfo(builder);

        // Act
        var result = await sut.IsInstalledAsync();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsInstalledAsync_WhenOutputIsNull_ReturnsFalse()
    {
        // Arrange
        var builder = FakeProcessExecutorBuilder.Create<string>(out var executor);
        A.CallTo(() => executor.ExecuteAsync()).Returns(Task.FromResult<string?>(null));
        var sut = new BrewInfo(builder);

        // Act
        var result = await sut.IsInstalledAsync();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetVersionAsync_WhenOutputIsStandardFormat_ReturnsVersionToken()
    {
        // Arrange
        var builder = FakeProcessExecutorBuilder.Create<string>(out var executor);
        A.CallTo(() => executor.ExecuteAsync()).Returns("Homebrew 4.3.1");
        var sut = new BrewInfo(builder);

        // Act
        var result = await sut.GetVersionAsync();

        // Assert
        result.Should().Be("4.3.1");
    }

    [Fact]
    public async Task GetVersionAsync_WhenOutputIsNull_ReturnsEmpty()
    {
        // Arrange
        var builder = FakeProcessExecutorBuilder.Create<string>(out var executor);
        A.CallTo(() => executor.ExecuteAsync()).Returns(Task.FromResult<string?>(null));
        var sut = new BrewInfo(builder);

        // Act
        var result = await sut.GetVersionAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetVersionAsync_WhenOutputHasNoSecondToken_ReturnsEmpty()
    {
        // Arrange
        var builder = FakeProcessExecutorBuilder.Create<string>(out var executor);
        A.CallTo(() => executor.ExecuteAsync()).Returns("Homebrew");
        var sut = new BrewInfo(builder);

        // Act
        var result = await sut.GetVersionAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetVersionAsync_WhenOutputIsEmptyString_ReturnsEmpty()
    {
        // Arrange
        var builder = FakeProcessExecutorBuilder.Create<string>(out var executor);
        A.CallTo(() => executor.ExecuteAsync()).Returns(string.Empty);
        var sut = new BrewInfo(builder);

        // Act
        var result = await sut.GetVersionAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetVersionAsync_WhenOutputContainsAdditionalTokens_ReturnsSecondToken()
    {
        // Arrange
        var builder = FakeProcessExecutorBuilder.Create<string>(out var executor);
        A.CallTo(() => executor.ExecuteAsync()).Returns("Homebrew 4.3.1 (git revision abc)");
        var sut = new BrewInfo(builder);

        // Act
        var result = await sut.GetVersionAsync();

        // Assert
        result.Should().Be("4.3.1");
    }

    /// <summary>
    /// Verifies that the version is extracted from the real multi-line <c>brew --version</c>
    /// output, whose second line reports the homebrew-core revision.
    /// </summary>
    [Fact]
    public async Task GetVersionAsync_WhenOutputIsMultiLine_ReturnsVersionOfFirstLineOnly()
    {
        // Arrange
        var builder = FakeProcessExecutorBuilder.Create<string>(out var executor);
        A.CallTo(() => executor.ExecuteAsync()).Returns("Homebrew 4.3.1\nHomebrew/homebrew-core (git revision abc)");
        var sut = new BrewInfo(builder);

        // Act
        var result = await sut.GetVersionAsync();

        // Assert
        result.Should().Be("4.3.1");
    }

    /// <summary>
    /// Verifies that a non-zero exit code of <c>brew --version</c> is reported as "not installed"
    /// instead of surfacing as an exception from the probe.
    /// </summary>
    [Fact]
    public async Task IsInstalledAsync_WhenExecutionFails_ReturnsFalse()
    {
        // Arrange
        var builder = FakeProcessExecutorBuilder.Create<string>(out var executor);
        A.CallTo(() => executor.ExecuteAsync())
            .Throws(new ProcessExecutionFailedException(127, "command not found", "std"));
        var sut = new BrewInfo(builder);

        // Act
        var result = await sut.IsInstalledAsync();

        // Assert
        result.Should().BeFalse();
    }

    /// <summary>
    /// Verifies the realistic "brew is not installed" case: starting the process fails with a
    /// <see cref="Win32Exception"/> because the executable is not on the PATH.
    /// </summary>
    [Fact]
    public async Task IsInstalledAsync_WhenBrewExecutableIsMissing_ReturnsFalse()
    {
        // Arrange
        var builder = FakeProcessExecutorBuilder.Create<string>(out var executor);
        A.CallTo(() => executor.ExecuteAsync()).Throws(new Win32Exception(2, "No such file or directory"));
        var sut = new BrewInfo(builder);

        // Act
        var result = await sut.IsInstalledAsync();

        // Assert
        result.Should().BeFalse();
    }

    /// <summary>
    /// Verifies that the failure handling stays narrow: unrelated exceptions must not be swallowed
    /// and silently reported as "not installed".
    /// </summary>
    [Fact]
    public async Task IsInstalledAsync_WhenExecutorThrowsUnrelatedException_DoesNotSwallowIt()
    {
        // Arrange
        var builder = FakeProcessExecutorBuilder.Create<string>(out var executor);
        A.CallTo(() => executor.ExecuteAsync()).Throws(new InvalidOperationException("boom"));
        var sut = new BrewInfo(builder);

        // Act
        var act = () => sut.IsInstalledAsync();

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task GetVersionAsync_WhenExecutionFails_ReturnsEmpty()
    {
        // Arrange
        var builder = FakeProcessExecutorBuilder.Create<string>(out var executor);
        A.CallTo(() => executor.ExecuteAsync())
            .Throws(new ProcessExecutionFailedException(127, "command not found", "std"));
        var sut = new BrewInfo(builder);

        // Act
        var result = await sut.GetVersionAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetVersionAsync_WhenBrewExecutableIsMissing_ReturnsEmpty()
    {
        // Arrange
        var builder = FakeProcessExecutorBuilder.Create<string>(out var executor);
        A.CallTo(() => executor.ExecuteAsync()).Throws(new Win32Exception(2, "No such file or directory"));
        var sut = new BrewInfo(builder);

        // Act
        var result = await sut.GetVersionAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetVersionAsync_WhenExecutorThrowsUnrelatedException_DoesNotSwallowIt()
    {
        // Arrange
        var builder = FakeProcessExecutorBuilder.Create<string>(out var executor);
        A.CallTo(() => executor.ExecuteAsync()).Throws(new InvalidOperationException("boom"));
        var sut = new BrewInfo(builder);

        // Act
        var act = () => sut.GetVersionAsync();

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public void Ctor_WhenBuilderIsNull_Throws()
    {
        // Arrange + Act
        var act = () => new BrewInfo(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }
}
