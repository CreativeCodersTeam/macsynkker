using System.Text.Json;
using AwesomeAssertions;
using CreativeCoders.MacOS.HomeBrew.Models;
using CreativeCoders.MacOS.HomeBrew.Models.Formulae;

namespace CreativeCoders.MacOS.HomeBrew.Tests.Models.Formulae;

public class BrewFormulaModelTests
{
    [Fact]
    public void Deserialize_WhenCompatibilityVersionIsNumber_ReturnsNumber()
    {
        // Arrange
        const string json = """{"compatibility_version": 3}""";

        // Act
        var result = JsonSerializer.Deserialize<BrewFormulaModel>(json);

        // Assert
        result!.CompatibilityVersion.Should().Be(3);
    }

    [Fact]
    public void Deserialize_WhenCompatibilityVersionIsNull_ReturnsNull()
    {
        // Arrange
        const string json = """{"compatibility_version": null}""";

        // Act
        var result = JsonSerializer.Deserialize<BrewFormulaModel>(json);

        // Assert
        result!.CompatibilityVersion.Should().BeNull();
    }

    [Fact]
    public void Deserialize_WhenCompatibilityVersionIsMissing_ReturnsNull()
    {
        // Arrange
        const string json = """{}""";

        // Act
        var result = JsonSerializer.Deserialize<BrewFormulaModel>(json);

        // Assert
        result!.CompatibilityVersion.Should().BeNull();
    }

    [Fact]
    public void Deserialize_WhenInstalledFormulaeHaveNumericCompatibilityVersion_ReturnsAllFormulae()
    {
        // Arrange
        const string json = """
            {
                "formulae": [
                    {"name": "abseil", "compatibility_version": 3},
                    {"name": "wget", "compatibility_version": null}
                ],
                "casks": []
            }
            """;

        // Act
        var result = JsonSerializer.Deserialize<BrewInstalledModel>(json);

        // Assert
        result!.Formulae.Should().HaveCount(2);
        result.Formulae[0].Name.Should().Be("abseil");
        result.Formulae[0].CompatibilityVersion.Should().Be(3);
        result.Formulae[1].Name.Should().Be("wget");
        result.Formulae[1].CompatibilityVersion.Should().BeNull();
    }
}
