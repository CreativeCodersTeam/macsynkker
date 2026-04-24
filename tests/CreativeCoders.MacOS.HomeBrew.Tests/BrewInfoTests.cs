using AwesomeAssertions;
using CreativeCoders.MacOS.HomeBrew.Tests.TestHelpers;
using FakeItEasy;

namespace CreativeCoders.MacOS.HomeBrew.Tests;

public class BrewInfoTests
{
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
    public void Ctor_WhenBuilderIsNull_Throws()
    {
        // Arrange + Act
        var act = () => new BrewInfo(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }
}
