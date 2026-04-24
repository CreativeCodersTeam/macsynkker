using System.Text.Json;
using AwesomeAssertions;
using CreativeCoders.MacOS.HomeBrew.Export;
using CreativeCoders.MacOS.HomeBrew.Models;
using CreativeCoders.MacOS.HomeBrew.Models.Casks;
using CreativeCoders.MacOS.HomeBrew.Models.Export;
using CreativeCoders.MacOS.HomeBrew.Models.Formulae;
using FakeItEasy;

namespace CreativeCoders.MacOS.HomeBrew.Tests.Export;

public class BrewExporterTests
{
    [Fact]
    public async Task CreateExportModelAsync_MapsFormulaeUsingFullNameWhenAvailable()
    {
        // Arrange
        var installed = new BrewInstalledModel
        {
            Formulae =
            [
                new BrewFormulaModel { Name = "wget", FullName = "homebrew/core/wget", Tap = "homebrew/core" }
            ]
        };
        var software = A.Fake<IBrewInstalledSoftware>();
        A.CallTo(() => software.GetInstalledSoftwareAsync()).Returns(installed);
        var sut = new BrewExporter(software);

        // Act
        var model = await sut.CreateExportModelAsync(false);

        // Assert
        model.Formulae.Should().ContainSingle();
        model.Formulae[0].Name.Should().Be("homebrew/core/wget");
        // Default formula tap is stripped to keep JSON slim
        model.Formulae[0].Tap.Should().BeNull();
    }

    [Fact]
    public async Task CreateExportModelAsync_FallsBackToNameWhenFullNameMissing()
    {
        // Arrange
        var installed = new BrewInstalledModel
        {
            Formulae = [new BrewFormulaModel { Name = "wget", FullName = null, Tap = "custom/tap" }]
        };
        var software = A.Fake<IBrewInstalledSoftware>();
        A.CallTo(() => software.GetInstalledSoftwareAsync()).Returns(installed);
        var sut = new BrewExporter(software);

        // Act
        var model = await sut.CreateExportModelAsync(false);

        // Assert
        model.Formulae[0].Name.Should().Be("wget");
        model.Formulae[0].Tap.Should().Be("custom/tap");
    }

    [Fact]
    public async Task CreateExportModelAsync_SkipsFormulaeWithEmptyName()
    {
        // Arrange
        var installed = new BrewInstalledModel
        {
            Formulae =
            [
                new BrewFormulaModel { Name = null, FullName = null },
                new BrewFormulaModel { Name = "   ", FullName = null },
                new BrewFormulaModel { Name = "wget", FullName = null }
            ]
        };
        var software = A.Fake<IBrewInstalledSoftware>();
        A.CallTo(() => software.GetInstalledSoftwareAsync()).Returns(installed);
        var sut = new BrewExporter(software);

        // Act
        var model = await sut.CreateExportModelAsync(false);

        // Assert
        model.Formulae.Should().ContainSingle().Which.Name.Should().Be("wget");
    }

    [Fact]
    public async Task CreateExportModelAsync_MapsCasksUsingFullTokenWhenAvailable()
    {
        // Arrange
        var installed = new BrewInstalledModel
        {
            Casks =
            [
                new BrewCaskModel { Token = "firefox", FullToken = "homebrew/cask/firefox", Tap = "HOMEBREW/CASK" }
            ]
        };
        var software = A.Fake<IBrewInstalledSoftware>();
        A.CallTo(() => software.GetInstalledSoftwareAsync()).Returns(installed);
        var sut = new BrewExporter(software);

        // Act
        var model = await sut.CreateExportModelAsync(false);

        // Assert
        model.Casks.Should().ContainSingle();
        model.Casks[0].Token.Should().Be("homebrew/cask/firefox");
        // Default cask tap is stripped regardless of casing
        model.Casks[0].Tap.Should().BeNull();
    }

    [Fact]
    public async Task CreateExportModelAsync_SkipsCasksWithEmptyToken()
    {
        // Arrange
        var installed = new BrewInstalledModel
        {
            Casks =
            [
                new BrewCaskModel { Token = null, FullToken = null },
                new BrewCaskModel { Token = "firefox", FullToken = null, Tap = null }
            ]
        };
        var software = A.Fake<IBrewInstalledSoftware>();
        A.CallTo(() => software.GetInstalledSoftwareAsync()).Returns(installed);
        var sut = new BrewExporter(software);

        // Act
        var model = await sut.CreateExportModelAsync(false);

        // Assert
        model.Casks.Should().ContainSingle().Which.Token.Should().Be("firefox");
    }

    [Fact]
    public async Task CreateExportModelAsync_WhenCaskTapIsWhitespace_SetsTapNull()
    {
        // Arrange
        var installed = new BrewInstalledModel
        {
            Casks = [new BrewCaskModel { Token = "firefox", FullToken = "firefox", Tap = "   " }]
        };
        var software = A.Fake<IBrewInstalledSoftware>();
        A.CallTo(() => software.GetInstalledSoftwareAsync()).Returns(installed);
        var sut = new BrewExporter(software);

        // Act
        var model = await sut.CreateExportModelAsync(false);

        // Assert
        model.Casks[0].Tap.Should().BeNull();
    }

    [Fact]
    public async Task CreateExportModelAsync_WhenIncludeDependenciesFalse_ExcludesDependencyFormulae()
    {
        // Arrange
        var installed = new BrewInstalledModel
        {
            Formulae =
            [
                new BrewFormulaModel
                {
                    Name = "wget",
                    Installed = [new BrewInstalledFormulaModel { InstalledAsDependency = false }]
                },
                new BrewFormulaModel
                {
                    Name = "openssl",
                    Installed = [new BrewInstalledFormulaModel { InstalledAsDependency = true }]
                }
            ]
        };
        var software = A.Fake<IBrewInstalledSoftware>();
        A.CallTo(() => software.GetInstalledSoftwareAsync()).Returns(installed);
        var sut = new BrewExporter(software);

        // Act
        var model = await sut.CreateExportModelAsync(false);

        // Assert
        model.Formulae.Should().ContainSingle().Which.Name.Should().Be("wget");
    }

    [Fact]
    public async Task CreateExportModelAsync_WhenIncludeDependenciesTrue_IncludesDependencyFormulae()
    {
        // Arrange
        var installed = new BrewInstalledModel
        {
            Formulae =
            [
                new BrewFormulaModel
                {
                    Name = "wget",
                    Installed = [new BrewInstalledFormulaModel { InstalledAsDependency = false }]
                },
                new BrewFormulaModel
                {
                    Name = "openssl",
                    Installed = [new BrewInstalledFormulaModel { InstalledAsDependency = true }]
                }
            ]
        };
        var software = A.Fake<IBrewInstalledSoftware>();
        A.CallTo(() => software.GetInstalledSoftwareAsync()).Returns(installed);
        var sut = new BrewExporter(software);

        // Act
        var model = await sut.CreateExportModelAsync(true);

        // Assert
        model.Formulae.Should().HaveCount(2);
        model.Formulae.Select(f => f.Name).Should().BeEquivalentTo("wget", "openssl");
    }

    [Fact]
    public async Task CreateExportModelAsync_WhenFormulaHasNoInstalledInfo_IncludedRegardlessOfFlag()
    {
        // Arrange
        var installed = new BrewInstalledModel
        {
            Formulae =
            [
                new BrewFormulaModel { Name = "wget", Installed = null },
                new BrewFormulaModel { Name = "curl", Installed = [] }
            ]
        };
        var software = A.Fake<IBrewInstalledSoftware>();
        A.CallTo(() => software.GetInstalledSoftwareAsync()).Returns(installed);
        var sut = new BrewExporter(software);

        // Act
        var model = await sut.CreateExportModelAsync(false);

        // Assert
        model.Formulae.Should().HaveCount(2);
    }

    [Fact]
    public async Task CreateExportModelAsync_WhenFormulaHasMixedInstalledEntries_TreatedAsDependency()
    {
        // Arrange – one installed entry is a dependency, the other is not
        var installed = new BrewInstalledModel
        {
            Formulae =
            [
                new BrewFormulaModel
                {
                    Name = "openssl",
                    Installed =
                    [
                        new BrewInstalledFormulaModel { InstalledAsDependency = false },
                        new BrewInstalledFormulaModel { InstalledAsDependency = true }
                    ]
                }
            ]
        };
        var software = A.Fake<IBrewInstalledSoftware>();
        A.CallTo(() => software.GetInstalledSoftwareAsync()).Returns(installed);
        var sut = new BrewExporter(software);

        // Act
        var model = await sut.CreateExportModelAsync(false);

        // Assert – IsInstalledAsDependency returns true if ANY entry is a dependency
        model.Formulae.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateExportModelAsync_WhenAllFormulaeAreDependencies_IncludeDependenciesTrue_ReturnsAll()
    {
        // Arrange
        var installed = new BrewInstalledModel
        {
            Formulae =
            [
                new BrewFormulaModel
                {
                    Name = "openssl",
                    Installed = [new BrewInstalledFormulaModel { InstalledAsDependency = true }]
                },
                new BrewFormulaModel
                {
                    Name = "zlib",
                    Installed = [new BrewInstalledFormulaModel { InstalledAsDependency = true }]
                }
            ]
        };
        var software = A.Fake<IBrewInstalledSoftware>();
        A.CallTo(() => software.GetInstalledSoftwareAsync()).Returns(installed);
        var sut = new BrewExporter(software);

        // Act
        var model = await sut.CreateExportModelAsync(true);

        // Assert
        model.Formulae.Should().HaveCount(2);
    }

    [Fact]
    public async Task CreateExportModelAsync_WhenIncludeDependenciesFalse_DoesNotAffectCasks()
    {
        // Arrange
        var installed = new BrewInstalledModel
        {
            Formulae =
            [
                new BrewFormulaModel
                {
                    Name = "openssl",
                    Installed = [new BrewInstalledFormulaModel { InstalledAsDependency = true }]
                }
            ],
            Casks = [new BrewCaskModel { Token = "firefox" }]
        };
        var software = A.Fake<IBrewInstalledSoftware>();
        A.CallTo(() => software.GetInstalledSoftwareAsync()).Returns(installed);
        var sut = new BrewExporter(software);

        // Act
        var model = await sut.CreateExportModelAsync(false);

        // Assert
        model.Formulae.Should().BeEmpty();
        model.Casks.Should().ContainSingle().Which.Token.Should().Be("firefox");
    }

    [Fact]
    public async Task ExportToFileAsync_PassesIncludeDependenciesToModel()
    {
        // Arrange
        var installed = new BrewInstalledModel
        {
            Formulae =
            [
                new BrewFormulaModel
                {
                    Name = "wget",
                    Installed = [new BrewInstalledFormulaModel { InstalledAsDependency = false }]
                },
                new BrewFormulaModel
                {
                    Name = "openssl",
                    Installed = [new BrewInstalledFormulaModel { InstalledAsDependency = true }]
                }
            ]
        };
        var software = A.Fake<IBrewInstalledSoftware>();
        A.CallTo(() => software.GetInstalledSoftwareAsync()).Returns(installed);
        var sut = new BrewExporter(software);
        var filePath = Path.Combine(Path.GetTempPath(), $"brew-export-{Guid.NewGuid():N}.json");

        try
        {
            // Act
            await sut.ExportToFileAsync(filePath, true);

            // Assert
            var content = await File.ReadAllTextAsync(filePath);
            var roundtrip = JsonSerializer.Deserialize<BrewExportModel>(content);
            roundtrip.Should().NotBeNull();
            roundtrip!.Formulae.Should().HaveCount(2);
        }
        finally
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }

    [Fact]
    public async Task ExportToFileAsync_WritesJsonWithFormulaeAndCasks()
    {
        // Arrange
        var installed = new BrewInstalledModel
        {
            Formulae = [new BrewFormulaModel { Name = "wget", Tap = "homebrew/core" }],
            Casks = [new BrewCaskModel { Token = "firefox", Tap = "homebrew/cask" }]
        };
        var software = A.Fake<IBrewInstalledSoftware>();
        A.CallTo(() => software.GetInstalledSoftwareAsync()).Returns(installed);
        var sut = new BrewExporter(software);
        var filePath = Path.Combine(Path.GetTempPath(), $"brew-export-{Guid.NewGuid():N}.json");

        try
        {
            // Act
            await sut.ExportToFileAsync(filePath, false);

            // Assert
            File.Exists(filePath).Should().BeTrue();
            var content = await File.ReadAllTextAsync(filePath);
            var roundtrip = JsonSerializer.Deserialize<BrewExportModel>(content);
            roundtrip.Should().NotBeNull();
            roundtrip.Formulae.Should().ContainSingle().Which.Name.Should().Be("wget");
            roundtrip.Casks.Should().ContainSingle().Which.Token.Should().Be("firefox");
        }
        finally
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }

    [Fact]
    public async Task ExportToFileAsync_CreatesMissingDirectory()
    {
        // Arrange
        var software = A.Fake<IBrewInstalledSoftware>();
        A.CallTo(() => software.GetInstalledSoftwareAsync()).Returns(new BrewInstalledModel());
        var sut = new BrewExporter(software);
        var dir = Path.Combine(Path.GetTempPath(), $"brew-export-dir-{Guid.NewGuid():N}");
        var filePath = Path.Combine(dir, "nested", "export.json");

        try
        {
            // Act
            await sut.ExportToFileAsync(filePath, false);

            // Assert
            File.Exists(filePath).Should().BeTrue();
        }
        finally
        {
            if (Directory.Exists(dir))
            {
                Directory.Delete(dir, recursive: true);
            }
        }
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task ExportToFileAsync_WhenFilePathInvalid_Throws(string? filePath)
    {
        // Arrange
        var software = A.Fake<IBrewInstalledSoftware>();
        var sut = new BrewExporter(software);

        // Act
        var act = () => sut.ExportToFileAsync(filePath!, false);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public void Ctor_WhenInstalledSoftwareIsNull_Throws()
    {
        // Arrange + Act
        var act = () => new BrewExporter(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }
}
