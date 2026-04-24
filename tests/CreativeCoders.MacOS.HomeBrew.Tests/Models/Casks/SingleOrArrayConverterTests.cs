using System.Text.Json;
using AwesomeAssertions;
using CreativeCoders.MacOS.HomeBrew.Models.Casks;

namespace CreativeCoders.MacOS.HomeBrew.Tests.Models.Casks;

public class SingleOrArrayConverterTests
{
    [Fact]
    public void Deserialize_WhenValueIsString_ReturnsArrayWithSingleElement()
    {
        const string json = """{"trash": "~/.copilot"}""";

        var result = JsonSerializer.Deserialize<BrewZapModel>(json);

        result!.Trash.Should().BeEquivalentTo(["~/.copilot"]);
    }

    [Fact]
    public void Deserialize_WhenValueIsArray_ReturnsArray()
    {
        const string json = """{"trash": ["~/Library/Preferences/com.test.plist", "~/Library/Caches/com.test"]}""";

        var result = JsonSerializer.Deserialize<BrewZapModel>(json);

        result!.Trash.Should().BeEquivalentTo(["~/Library/Preferences/com.test.plist", "~/Library/Caches/com.test"]);
    }

    [Fact]
    public void Deserialize_WhenValueIsNull_ReturnsNull()
    {
        const string json = """{"trash": null}""";

        var result = JsonSerializer.Deserialize<BrewZapModel>(json);

        result!.Trash.Should().BeNull();
    }

    [Fact]
    public void Deserialize_WhenPropertyIsMissing_ReturnsNull()
    {
        const string json = """{}""";

        var result = JsonSerializer.Deserialize<BrewZapModel>(json);

        result!.Trash.Should().BeNull();
    }

    [Fact]
    public void Deserialize_WhenValueIsEmptyArray_ReturnsEmptyArray()
    {
        const string json = """{"trash": []}""";

        var result = JsonSerializer.Deserialize<BrewZapModel>(json);

        result!.Trash.Should().BeEmpty();
    }

    [Fact]
    public void Serialize_WhenValueIsArray_WritesArray()
    {
        var model = new BrewZapModel { Trash = ["~/file1", "~/file2"] };

        var json = JsonSerializer.Serialize(model);

        using var doc = JsonDocument.Parse(json);
        var trash = doc.RootElement.GetProperty("trash");
        trash.ValueKind.Should().Be(JsonValueKind.Array);
        trash.GetArrayLength().Should().Be(2);
    }

    [Fact]
    public void Serialize_WhenValueIsNull_WritesNull()
    {
        var model = new BrewZapModel { Trash = null };

        var json = JsonSerializer.Serialize(model);

        using var doc = JsonDocument.Parse(json);
        var trash = doc.RootElement.GetProperty("trash");
        trash.ValueKind.Should().Be(JsonValueKind.Null);
    }
}
