using System.Text.Json;
using AwesomeAssertions;
using CreativeCoders.MacOS.HomeBrew.Models.Formulae;

namespace CreativeCoders.MacOS.HomeBrew.Tests.Models.Formulae;

public class BrewFormulaOptionModelTests
{
    [Fact]
    public void Deserialize_WhenOptionsContainObjects_ReturnsModelsWithProperties()
    {
        const string json = """
            {
                "options": [
                    {"option": "--without-mono", "description": "Build without mono support"}
                ]
            }
            """;

        var result = JsonSerializer.Deserialize<BrewFormulaModel>(json);

        result!.Options.Should().HaveCount(1);
        result.Options![0].Option.Should().Be("--without-mono");
        result.Options[0].Description.Should().Be("Build without mono support");
    }

    [Fact]
    public void Deserialize_WhenOptionsIsEmptyArray_ReturnsEmptyArray()
    {
        const string json = """{"options": []}""";

        var result = JsonSerializer.Deserialize<BrewFormulaModel>(json);

        result!.Options.Should().BeEmpty();
    }

    [Fact]
    public void Deserialize_WhenOptionsIsNull_ReturnsNull()
    {
        const string json = """{"options": null}""";

        var result = JsonSerializer.Deserialize<BrewFormulaModel>(json);

        result!.Options.Should().BeNull();
    }

    [Fact]
    public void Deserialize_WhenOptionsIsMissing_ReturnsNull()
    {
        const string json = """{}""";

        var result = JsonSerializer.Deserialize<BrewFormulaModel>(json);

        result!.Options.Should().BeNull();
    }
}
