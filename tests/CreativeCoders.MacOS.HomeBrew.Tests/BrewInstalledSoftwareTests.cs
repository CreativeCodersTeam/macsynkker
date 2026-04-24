using AwesomeAssertions;
using CreativeCoders.MacOS.HomeBrew.Models;
using CreativeCoders.MacOS.HomeBrew.Models.Casks;
using CreativeCoders.MacOS.HomeBrew.Models.Formulae;
using CreativeCoders.MacOS.HomeBrew.Tests.TestHelpers;
using FakeItEasy;

namespace CreativeCoders.MacOS.HomeBrew.Tests;

public class BrewInstalledSoftwareTests
{
    [Fact]
    public async Task GetInstalledSoftwareAsync_WhenExecutorReturnsModel_ReturnsSameModel()
    {
        // Arrange
        var model = new BrewInstalledModel
        {
            Casks = [new BrewCaskModel { Token = "firefox" }],
            Formulae = [new BrewFormulaModel { Name = "wget" }]
        };
        var builder = FakeProcessExecutorBuilder.Create<BrewInstalledModel>(out var executor);
        A.CallTo(() => executor.ExecuteAsync()).Returns(model);
        var sut = new BrewInstalledSoftware(builder);

        // Act
        var result = await sut.GetInstalledSoftwareAsync();

        // Assert
        result.Should().BeSameAs(model);
    }

    [Fact]
    public async Task GetInstalledSoftwareAsync_WhenExecutorReturnsNull_ReturnsEmptyModel()
    {
        // Arrange
        var builder = FakeProcessExecutorBuilder.Create<BrewInstalledModel>(out var executor);
        A.CallTo(() => executor.ExecuteAsync()).Returns(Task.FromResult<BrewInstalledModel?>(null));
        var sut = new BrewInstalledSoftware(builder);

        // Act
        var result = await sut.GetInstalledSoftwareAsync();

        // Assert
        result.Should().NotBeNull();
        result.Casks.Should().BeEmpty();
        result.Formulae.Should().BeEmpty();
    }

    [Fact]
    public void Ctor_WhenBuilderIsNull_Throws()
    {
        // Arrange + Act
        var act = () => new BrewInstalledSoftware(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }
}
