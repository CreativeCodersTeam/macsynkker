using AwesomeAssertions;
using CreativeCoders.MacOS.HomeBrew.Models;
using CreativeCoders.MacOS.HomeBrew.Models.Casks;
using CreativeCoders.MacOS.HomeBrew.Models.Formulae;

namespace CreativeCoders.MacOS.HomeBrew.Tests;

public class BrewInstalledModelExtensionsTests
{
    [Fact]
    public void GetOutdatedCasks_OnlyReturnsCasksWhereInstalledDiffersFromVersion()
    {
        // Arrange
        var model = new BrewInstalledModel
        {
            Casks =
            [
                new BrewCaskModel { Token = "a", Installed = "1.0", Version = "1.0" },
                new BrewCaskModel { Token = "b", Installed = "1.0", Version = "2.0" },
                new BrewCaskModel { Token = "c", Installed = null, Version = "1.0" }
            ]
        };

        // Act
        var result = model.GetOutdatedCasks();

        // Assert
        result.Select(x => x.Token).Should().BeEquivalentTo("b", "c");
    }

    [Fact]
    public void GetCasks_WhenOnlyOutdatedTrue_ReturnsOutdatedOnly()
    {
        // Arrange
        var model = new BrewInstalledModel
        {
            Casks =
            [
                new BrewCaskModel { Token = "a", Installed = "1.0", Version = "1.0" },
                new BrewCaskModel { Token = "b", Installed = "1.0", Version = "2.0" }
            ]
        };

        // Act
        var result = model.GetCasks(onlyOutdated: true);

        // Assert
        result.Should().HaveCount(1);
        result[0].Token.Should().Be("b");
    }

    [Fact]
    public void GetCasks_WhenOnlyOutdatedFalse_ReturnsAll()
    {
        // Arrange
        var model = new BrewInstalledModel
        {
            Casks =
            [
                new BrewCaskModel { Token = "a", Installed = "1.0", Version = "1.0" },
                new BrewCaskModel { Token = "b", Installed = "1.0", Version = "2.0" }
            ]
        };

        // Act
        var result = model.GetCasks(onlyOutdated: false);

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public void GetOutdatedFormulae_WhenInstalledVersionMatchesStable_IsNotOutdated()
    {
        // Arrange
        var formula = new BrewFormulaModel
        {
            Name = "f1",
            Versions = new BrewVersionsModel { Stable = "1.0" },
            Installed = [new BrewInstalledFormulaModel { Version = "1.0" }]
        };
        var model = new BrewInstalledModel { Formulae = [formula] };

        // Act
        var result = model.GetOutdatedFormulae();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void GetOutdatedFormulae_WhenInstalledVersionIsRevisionOfStable_IsNotOutdated()
    {
        // Arrange - Homebrew formats rebuilds as "<stable>_<rev>" (e.g., "1.0_1")
        var formula = new BrewFormulaModel
        {
            Name = "f1",
            Versions = new BrewVersionsModel { Stable = "1.0" },
            Installed = [new BrewInstalledFormulaModel { Version = "1.0_1" }]
        };
        var model = new BrewInstalledModel { Formulae = [formula] };

        // Act
        var result = model.GetOutdatedFormulae();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void GetOutdatedFormulae_WhenInstalledVersionDiffers_IsOutdated()
    {
        // Arrange
        var formula = new BrewFormulaModel
        {
            Name = "f1",
            Versions = new BrewVersionsModel { Stable = "2.0" },
            Installed = [new BrewInstalledFormulaModel { Version = "1.0" }]
        };
        var model = new BrewInstalledModel { Formulae = [formula] };

        // Act
        var result = model.GetOutdatedFormulae();

        // Assert
        result.Should().ContainSingle().Which.Name.Should().Be("f1");
    }

    [Fact]
    public void GetOutdatedFormulae_WhenInstalledIsNull_IsNotOutdated()
    {
        // Arrange
        var formula = new BrewFormulaModel
        {
            Name = "f1",
            Versions = new BrewVersionsModel { Stable = "2.0" },
            Installed = null
        };
        var model = new BrewInstalledModel { Formulae = [formula] };

        // Act
        var result = model.GetOutdatedFormulae();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void GetOutdatedFormulae_WhenInstalledHasMultipleAndOneDiffers_IsOutdated()
    {
        // Arrange
        var formula = new BrewFormulaModel
        {
            Name = "f1",
            Versions = new BrewVersionsModel { Stable = "2.0" },
            Installed =
            [
                new BrewInstalledFormulaModel { Version = "2.0" },
                new BrewInstalledFormulaModel { Version = "1.0" }
            ]
        };
        var model = new BrewInstalledModel { Formulae = [formula] };

        // Act
        var result = model.GetOutdatedFormulae();

        // Assert
        result.Should().ContainSingle();
    }

    [Fact]
    public void GetOutdatedFormulae_WhenBothVersionsNull_IsNotOutdated()
    {
        // Arrange
        var formula = new BrewFormulaModel
        {
            Name = "f1",
            Versions = new BrewVersionsModel { Stable = null },
            Installed = [new BrewInstalledFormulaModel { Version = null }]
        };
        var model = new BrewInstalledModel { Formulae = [formula] };

        // Act
        var result = model.GetOutdatedFormulae();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void GetOutdatedFormulae_WhenOnlyInstalledVersionIsNull_IsOutdated()
    {
        // Arrange
        var formula = new BrewFormulaModel
        {
            Name = "f1",
            Versions = new BrewVersionsModel { Stable = "2.0" },
            Installed = [new BrewInstalledFormulaModel { Version = null }]
        };
        var model = new BrewInstalledModel { Formulae = [formula] };

        // Act
        var result = model.GetOutdatedFormulae();

        // Assert
        result.Should().ContainSingle();
    }

    [Fact]
    public void GetFormulae_WhenOnlyOutdatedTrue_ReturnsOutdatedOnly()
    {
        // Arrange
        var outdated = new BrewFormulaModel
        {
            Name = "outdated",
            Versions = new BrewVersionsModel { Stable = "2.0" },
            Installed = [new BrewInstalledFormulaModel { Version = "1.0" }]
        };
        var current = new BrewFormulaModel
        {
            Name = "current",
            Versions = new BrewVersionsModel { Stable = "1.0" },
            Installed = [new BrewInstalledFormulaModel { Version = "1.0" }]
        };
        var model = new BrewInstalledModel { Formulae = [outdated, current] };

        // Act
        var result = model.GetFormulae(onlyOutdated: true);

        // Assert
        result.Should().ContainSingle().Which.Name.Should().Be("outdated");
    }

    [Fact]
    public void GetOutdatedFormulae_WhenInstalledArrayIsEmpty_IsNotOutdated()
    {
        // Arrange
        var formula = new BrewFormulaModel
        {
            Name = "f1",
            Versions = new BrewVersionsModel { Stable = "2.0" },
            Installed = []
        };
        var model = new BrewInstalledModel { Formulae = [formula] };

        // Act
        var result = model.GetOutdatedFormulae();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void GetOutdatedFormulae_WhenInstalledIsPrefixButWithoutUnderscore_IsOutdated()
    {
        // Arrange - Only "<stable>_..." should be treated as equivalent; "1.01" must not match "1.0"
        var formula = new BrewFormulaModel
        {
            Name = "f1",
            Versions = new BrewVersionsModel { Stable = "1.0" },
            Installed = [new BrewInstalledFormulaModel { Version = "1.01" }]
        };
        var model = new BrewInstalledModel { Formulae = [formula] };

        // Act
        var result = model.GetOutdatedFormulae();

        // Assert
        result.Should().ContainSingle();
    }

    [Fact]
    public void GetOutdatedFormulae_WhenOnlyStableVersionIsNull_IsOutdated()
    {
        // Arrange
        var formula = new BrewFormulaModel
        {
            Name = "f1",
            Versions = new BrewVersionsModel { Stable = null },
            Installed = [new BrewInstalledFormulaModel { Version = "1.0" }]
        };
        var model = new BrewInstalledModel { Formulae = [formula] };

        // Act
        var result = model.GetOutdatedFormulae();

        // Assert
        result.Should().ContainSingle();
    }

    [Fact]
    public void GetOutdatedCasks_WhenCasksAreEmpty_ReturnsEmptyArray()
    {
        // Arrange
        var model = new BrewInstalledModel();

        // Act
        var result = model.GetOutdatedCasks();

        // Assert
        result.Should().BeEmpty();
    }
}
