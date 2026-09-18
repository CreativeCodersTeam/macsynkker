using AwesomeAssertions;
using CreativeCoders.MacOS.HomeBrew.Models.Formulae;

namespace CreativeCoders.MacOS.HomeBrew.Tests.Models.Formulae;

public class BrewFormulaModelExtensionsTests
{
    [Fact]
    public void IsInstalledAsDependency_WhenInstalledIsNull_ReturnsFalse()
    {
        // Arrange
        var formula = new BrewFormulaModel { Name = "wget" };

        // Act
        var result = formula.IsInstalledAsDependency();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsInstalledAsDependency_WhenInstalledIsEmpty_ReturnsFalse()
    {
        // Arrange
        var formula = new BrewFormulaModel { Name = "wget", Installed = [] };

        // Act
        var result = formula.IsInstalledAsDependency();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsInstalledAsDependency_WhenSingleEntryIsDependency_ReturnsTrue()
    {
        // Arrange
        var formula = new BrewFormulaModel
        {
            Name = "wget",
            Installed = [new BrewInstalledFormulaModel { InstalledAsDependency = true }]
        };

        // Act
        var result = formula.IsInstalledAsDependency();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsInstalledAsDependency_WhenNoEntryIsDependency_ReturnsFalse()
    {
        // Arrange
        var formula = new BrewFormulaModel
        {
            Name = "wget",
            Installed =
            [
                new BrewInstalledFormulaModel { InstalledAsDependency = false },
                new BrewInstalledFormulaModel { InstalledAsDependency = false }
            ]
        };

        // Act
        var result = formula.IsInstalledAsDependency();

        // Assert
        result.Should().BeFalse();
    }

    /// <summary>
    /// Verifies that a formula counts as a dependency as soon as at least one of its installed
    /// instances was pulled in as one, even if another instance was requested explicitly.
    /// </summary>
    [Fact]
    public void IsInstalledAsDependency_WhenAnyEntryIsDependency_ReturnsTrue()
    {
        // Arrange
        var formula = new BrewFormulaModel
        {
            Name = "wget",
            Installed =
            [
                new BrewInstalledFormulaModel { InstalledAsDependency = false },
                new BrewInstalledFormulaModel { InstalledAsDependency = true }
            ]
        };

        // Act
        var result = formula.IsInstalledAsDependency();

        // Assert
        result.Should().BeTrue();
    }

    /// <summary>
    /// Verifies that a missing <c>installed_as_dependency</c> field in the brew JSON is treated
    /// as "not a dependency" instead of throwing.
    /// </summary>
    [Fact]
    public void IsInstalledAsDependency_WhenFlagIsNull_ReturnsFalse()
    {
        // Arrange
        var formula = new BrewFormulaModel
        {
            Name = "wget",
            Installed = [new BrewInstalledFormulaModel { InstalledAsDependency = null }]
        };

        // Act
        var result = formula.IsInstalledAsDependency();

        // Assert
        result.Should().BeFalse();
    }
}
