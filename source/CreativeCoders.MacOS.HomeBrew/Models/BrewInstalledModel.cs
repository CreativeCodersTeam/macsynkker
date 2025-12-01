using System.Text.Json.Serialization;
using CreativeCoders.MacOS.HomeBrew.Models.Formulae;

namespace CreativeCoders.MacOS.HomeBrew.Models;

public class BrewInstalledModel
{
    [JsonPropertyName("casks")]
    public BrewCaskModel[] Casks { get; set; } = [];

    [JsonPropertyName("formulae")]
    public BrewFormulaModel[] Formulae { get; set; } = [];
}

