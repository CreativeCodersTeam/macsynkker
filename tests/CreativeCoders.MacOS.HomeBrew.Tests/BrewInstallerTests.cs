using AwesomeAssertions;
using CreativeCoders.MacOS.HomeBrew.Import;
using CreativeCoders.MacOS.HomeBrew.Tests.TestHelpers;
using CreativeCoders.ProcessUtils.Execution;
using FakeItEasy;

namespace CreativeCoders.MacOS.HomeBrew.Tests;

public class BrewInstallerTests
{
    [Fact]
    public async Task TapAsync_WhenExecutorSucceeds_InvokesBrewTap()
    {
        // Arrange
        var builder = FakeProcessExecutorBuilder.Create<string>(out var executor);
        var sut = new BrewInstaller(builder);

        // Act
        await sut.TapAsync("homebrew/cask");

        // Assert
        A.CallTo(() => executor.ExecuteAsync(A<IDictionary<string, object?>>.That
                .Matches(d => (string?)d["tap"] == "homebrew/cask")))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task TapAsync_WhenExecutionFails_ThrowsBrewInstallFailedExceptionWithTapKind()
    {
        // Arrange
        var builder = FakeProcessExecutorBuilder.Create<string>(out var executor);
        A.CallTo(() => executor.ExecuteAsync(A<IDictionary<string, object?>>._))
            .Throws(new ProcessExecutionFailedException(2, "err", "std"));
        var sut = new BrewInstaller(builder);

        // Act
        var act = () => sut.TapAsync("bad/tap");

        // Assert
        var ex = await act.Should().ThrowAsync<BrewInstallFailedException>();
        ex.Which.Kind.Should().Be(BrewInstallTargetKind.Tap);
        ex.Which.Target.Should().Be("bad/tap");
        ex.Which.ErrorOutput.Should().Be("err");
        ex.Which.ExitCode.Should().Be(2);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task TapAsync_WhenTapIsNullOrWhitespace_Throws(string? tap)
    {
        // Arrange
        var builder = FakeProcessExecutorBuilder.Create<string>(out _);
        var sut = new BrewInstaller(builder);

        // Act
        var act = () => sut.TapAsync(tap!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task InstallFormulaAsync_WhenExecutorSucceeds_InvokesBrewInstall()
    {
        // Arrange
        var builder = FakeProcessExecutorBuilder.Create<string>(out var executor);
        var sut = new BrewInstaller(builder);

        // Act
        await sut.InstallFormulaAsync("wget");

        // Assert
        A.CallTo(() => executor.ExecuteAsync(A<IDictionary<string, object?>>.That
                .Matches(d => (string?)d["name"] == "wget")))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task InstallFormulaAsync_WhenExecutionFails_ThrowsWithFormulaKind()
    {
        // Arrange
        var builder = FakeProcessExecutorBuilder.Create<string>(out var executor);
        A.CallTo(() => executor.ExecuteAsync(A<IDictionary<string, object?>>._))
            .Throws(new ProcessExecutionFailedException(1, "boom", "std"));
        var sut = new BrewInstaller(builder);

        // Act
        var act = () => sut.InstallFormulaAsync("wget");

        // Assert
        var ex = await act.Should().ThrowAsync<BrewInstallFailedException>();
        ex.Which.Kind.Should().Be(BrewInstallTargetKind.Formula);
        ex.Which.Target.Should().Be("wget");
    }

    [Fact]
    public async Task InstallCaskAsync_WhenExecutorSucceeds_InvokesBrewInstallCask()
    {
        // Arrange
        var builder = FakeProcessExecutorBuilder.Create<string>(out var executor);
        var sut = new BrewInstaller(builder);

        // Act
        await sut.InstallCaskAsync("firefox");

        // Assert
        A.CallTo(() => executor.ExecuteAsync(A<IDictionary<string, object?>>.That
                .Matches(d => (string?)d["token"] == "firefox")))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task InstallCaskAsync_WhenExecutionFails_ThrowsWithCaskKind()
    {
        // Arrange
        var builder = FakeProcessExecutorBuilder.Create<string>(out var executor);
        A.CallTo(() => executor.ExecuteAsync(A<IDictionary<string, object?>>._))
            .Throws(new ProcessExecutionFailedException(3, "err", "std"));
        var sut = new BrewInstaller(builder);

        // Act
        var act = () => sut.InstallCaskAsync("firefox");

        // Assert
        var ex = await act.Should().ThrowAsync<BrewInstallFailedException>();
        ex.Which.Kind.Should().Be(BrewInstallTargetKind.Cask);
        ex.Which.Target.Should().Be("firefox");
        ex.Which.ExitCode.Should().Be(3);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task InstallFormulaAsync_WhenNameIsNullOrWhitespace_Throws(string? name)
    {
        // Arrange
        var builder = FakeProcessExecutorBuilder.Create<string>(out _);
        var sut = new BrewInstaller(builder);

        // Act
        var act = () => sut.InstallFormulaAsync(name!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task InstallCaskAsync_WhenTokenIsNullOrWhitespace_Throws(string? token)
    {
        // Arrange
        var builder = FakeProcessExecutorBuilder.Create<string>(out _);
        var sut = new BrewInstaller(builder);

        // Act
        var act = () => sut.InstallCaskAsync(token!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public void Ctor_WhenBuilderIsNull_Throws()
    {
        // Arrange + Act
        var act = () => new BrewInstaller(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }
}
