using System.Text.Json;
using AwesomeAssertions;
using CreativeCoders.MacOS.HomeBrew.Import;
using CreativeCoders.MacOS.HomeBrew.Models.Export;
using FakeItEasy;

namespace CreativeCoders.MacOS.HomeBrew.Tests.Import;

public class BrewImporterTests
{
    [Fact]
    public async Task ImportAsync_WithFormulaeAndCasks_InstallsDistinctTapsFirst()
    {
        // Arrange
        var installer = A.Fake<IBrewInstaller>();
        var sut = new BrewImporter(installer);

        var exportModel = new BrewExportModel
        {
            Formulae =
            [
                new BrewExportFormulaModel { Name = "wget", Tap = "custom/tap" },
                new BrewExportFormulaModel { Name = "curl", Tap = "custom/tap" }
            ],
            Casks =
            [
                new BrewExportCaskModel { Token = "firefox", Tap = "other/tap" }
            ]
        };

        // Act
        await sut.ImportAsync(exportModel);

        // Assert
        A.CallTo(() => installer.TapAsync("custom/tap")).MustHaveHappenedOnceExactly();
        A.CallTo(() => installer.TapAsync("other/tap")).MustHaveHappenedOnceExactly();
        A.CallTo(() => installer.InstallFormulaAsync("wget")).MustHaveHappenedOnceExactly();
        A.CallTo(() => installer.InstallFormulaAsync("curl")).MustHaveHappenedOnceExactly();
        A.CallTo(() => installer.InstallCaskAsync("firefox")).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ImportAsync_DeduplicatesTapsCaseInsensitively()
    {
        // Arrange
        var installer = A.Fake<IBrewInstaller>();
        var sut = new BrewImporter(installer);
        var exportModel = new BrewExportModel
        {
            Formulae =
            [
                new BrewExportFormulaModel { Name = "wget", Tap = "custom/tap" },
                new BrewExportFormulaModel { Name = "curl", Tap = "CUSTOM/TAP" }
            ]
        };

        // Act
        await sut.ImportAsync(exportModel);

        // Assert
        A.CallTo(() => installer.TapAsync(A<string>._)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ImportAsync_SkipsFormulaeAndCasksWithEmptyNameOrToken()
    {
        // Arrange
        var installer = A.Fake<IBrewInstaller>();
        var sut = new BrewImporter(installer);
        var exportModel = new BrewExportModel
        {
            Formulae =
            [
                new BrewExportFormulaModel { Name = "" },
                new BrewExportFormulaModel { Name = "   " }
            ],
            Casks =
            [
                new BrewExportCaskModel { Token = "" }
            ]
        };

        // Act
        await sut.ImportAsync(exportModel);

        // Assert
        A.CallTo(() => installer.InstallFormulaAsync(A<string>._)).MustNotHaveHappened();
        A.CallTo(() => installer.InstallCaskAsync(A<string>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task ImportAsync_WhenInstallFails_CollectsAndThrowsAggregate()
    {
        // Arrange
        var installer = A.Fake<IBrewInstaller>();
        A.CallTo(() => installer.InstallFormulaAsync("bad"))
            .Throws(new BrewInstallFailedException(BrewInstallTargetKind.Formula, "bad", "err", 1));
        A.CallTo(() => installer.InstallCaskAsync("badcask"))
            .Throws(new BrewInstallFailedException(BrewInstallTargetKind.Cask, "badcask", "err", 1));
        var sut = new BrewImporter(installer);
        var exportModel = new BrewExportModel
        {
            Formulae =
            [
                new BrewExportFormulaModel { Name = "good" },
                new BrewExportFormulaModel { Name = "bad" }
            ],
            Casks = [new BrewExportCaskModel { Token = "badcask" }]
        };

        // Act
        var act = () => sut.ImportAsync(exportModel);

        // Assert
        var ex = await act.Should().ThrowAsync<BrewImportFailedException>();
        ex.Which.Failures.Should().HaveCount(2);
        // Ensure the good formula was still installed despite failures
        A.CallTo(() => installer.InstallFormulaAsync("good")).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ImportAsync_ReportsProgressForEachStep()
    {
        // Arrange
        var installer = A.Fake<IBrewInstaller>();
        var sut = new BrewImporter(installer);
        var exportModel = new BrewExportModel
        {
            Formulae = [new BrewExportFormulaModel { Name = "wget", Tap = "some/tap" }],
            Casks = [new BrewExportCaskModel { Token = "firefox" }]
        };
        var reports = new List<BrewImportProgress>();
        var progress = new Progress<BrewImportProgress>(reports.Add);

        // Act
        await sut.ImportAsync(exportModel, progress);

        // Allow the Progress<T> SynchronizationContext to flush callbacks
        await Task.Delay(50);

        // Assert
        reports.Should().Contain(r => r.Step == BrewImportStep.Tap
            && r.State == BrewImportStepState.Starting && r.Target == "some/tap");
        reports.Should().Contain(r => r.Step == BrewImportStep.Tap
            && r.State == BrewImportStepState.Succeeded && r.Target == "some/tap");
        reports.Should().Contain(r => r.Step == BrewImportStep.InstallFormula
            && r.State == BrewImportStepState.Succeeded && r.Target == "wget");
        reports.Should().Contain(r => r.Step == BrewImportStep.InstallCask
            && r.State == BrewImportStepState.Succeeded && r.Target == "firefox");
    }

    [Fact]
    public async Task ImportAsync_WhenStepFails_ReportsFailedProgressWithError()
    {
        // Arrange
        var failure = new BrewInstallFailedException(BrewInstallTargetKind.Formula, "bad", "err", 1);
        var installer = A.Fake<IBrewInstaller>();
        A.CallTo(() => installer.InstallFormulaAsync("bad")).Throws(failure);
        var sut = new BrewImporter(installer);
        var exportModel = new BrewExportModel
        {
            Formulae = [new BrewExportFormulaModel { Name = "bad" }]
        };
        var reports = new List<BrewImportProgress>();
        var progress = new Progress<BrewImportProgress>(reports.Add);

        // Act
        var act = () => sut.ImportAsync(exportModel, progress);

        await act.Should().ThrowAsync<BrewImportFailedException>();
        await Task.Delay(50);

        // Assert
        reports.Should().Contain(r => r.State == BrewImportStepState.Failed
            && r.Target == "bad" && ReferenceEquals(r.Error, failure));
    }

    [Fact]
    public async Task ImportAsync_WhenExportModelIsNull_Throws()
    {
        // Arrange
        var installer = A.Fake<IBrewInstaller>();
        var sut = new BrewImporter(installer);

        // Act
        var act = () => sut.ImportAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task ReadFileAsync_ReturnsDeserializedModel()
    {
        // Arrange
        var installer = A.Fake<IBrewInstaller>();
        var sut = new BrewImporter(installer);
        var model = new BrewExportModel
        {
            Formulae = [new BrewExportFormulaModel { Name = "wget", Tap = "homebrew/core" }],
            Casks = [new BrewExportCaskModel { Token = "firefox" }]
        };
        var filePath = Path.Combine(Path.GetTempPath(), $"brew-import-{Guid.NewGuid():N}.json");
        await File.WriteAllTextAsync(filePath, JsonSerializer.Serialize(model));

        try
        {
            // Act
            var result = await sut.ReadFileAsync(filePath);

            // Assert
            result.Formulae.Should().ContainSingle().Which.Name.Should().Be("wget");
            result.Casks.Should().ContainSingle().Which.Token.Should().Be("firefox");
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public async Task ReadFileAsync_WhenJsonIsNullLiteral_ReturnsEmptyModel()
    {
        // Arrange
        var installer = A.Fake<IBrewInstaller>();
        var sut = new BrewImporter(installer);
        var filePath = Path.Combine(Path.GetTempPath(), $"brew-import-{Guid.NewGuid():N}.json");
        await File.WriteAllTextAsync(filePath, "null");

        try
        {
            // Act
            var result = await sut.ReadFileAsync(filePath);

            // Assert
            result.Should().NotBeNull();
            result.Formulae.Should().BeEmpty();
            result.Casks.Should().BeEmpty();
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task ReadFileAsync_WhenFilePathInvalid_Throws(string? filePath)
    {
        // Arrange
        var installer = A.Fake<IBrewInstaller>();
        var sut = new BrewImporter(installer);

        // Act
        var act = () => sut.ReadFileAsync(filePath!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task ImportFromFileAsync_ReadsFileThenImports()
    {
        // Arrange
        var installer = A.Fake<IBrewInstaller>();
        var sut = new BrewImporter(installer);
        var model = new BrewExportModel
        {
            Formulae = [new BrewExportFormulaModel { Name = "wget" }]
        };
        var filePath = Path.Combine(Path.GetTempPath(), $"brew-import-{Guid.NewGuid():N}.json");
        await File.WriteAllTextAsync(filePath, JsonSerializer.Serialize(model));

        try
        {
            // Act
            await sut.ImportFromFileAsync(filePath);

            // Assert
            A.CallTo(() => installer.InstallFormulaAsync("wget")).MustHaveHappenedOnceExactly();
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public async Task ImportAsync_WhenModelIsEmpty_DoesNotInvokeInstallerAndDoesNotThrow()
    {
        // Arrange
        var installer = A.Fake<IBrewInstaller>();
        var sut = new BrewImporter(installer);

        // Act
        await sut.ImportAsync(new BrewExportModel());

        // Assert
        A.CallTo(installer).MustNotHaveHappened();
    }

    [Fact]
    public async Task ImportAsync_WhenTapFails_DoesNotPreventSubsequentInstalls()
    {
        // Arrange
        var installer = A.Fake<IBrewInstaller>();
        A.CallTo(() => installer.TapAsync("custom/tap"))
            .Throws(new BrewInstallFailedException(BrewInstallTargetKind.Tap, "custom/tap", "err", 1));
        var sut = new BrewImporter(installer);
        var exportModel = new BrewExportModel
        {
            Formulae = [new BrewExportFormulaModel { Name = "wget", Tap = "custom/tap" }],
            Casks = [new BrewExportCaskModel { Token = "firefox" }]
        };

        // Act
        var act = () => sut.ImportAsync(exportModel);

        // Assert
        await act.Should().ThrowAsync<BrewImportFailedException>();
        A.CallTo(() => installer.InstallFormulaAsync("wget")).MustHaveHappenedOnceExactly();
        A.CallTo(() => installer.InstallCaskAsync("firefox")).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ReadFileAsync_WhenFileDoesNotExist_ThrowsFileNotFoundException()
    {
        // Arrange
        var sut = new BrewImporter(A.Fake<IBrewInstaller>());
        var filePath = Path.Combine(Path.GetTempPath(), $"does-not-exist-{Guid.NewGuid():N}.json");

        // Act
        var act = () => sut.ReadFileAsync(filePath);

        // Assert
        await act.Should().ThrowAsync<FileNotFoundException>();
    }

    [Fact]
    public async Task ReadFileAsync_WhenJsonIsMalformed_Throws()
    {
        // Arrange
        var sut = new BrewImporter(A.Fake<IBrewInstaller>());
        var filePath = Path.Combine(Path.GetTempPath(), $"malformed-{Guid.NewGuid():N}.json");
        await File.WriteAllTextAsync(filePath, "{ this is not valid json");

        try
        {
            // Act
            var act = () => sut.ReadFileAsync(filePath);

            // Assert
            await act.Should().ThrowAsync<JsonException>();
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public async Task ReadFileAsync_WhenJsonIsEmptyObject_ReturnsModelWithEmptyCollections()
    {
        // Arrange
        var sut = new BrewImporter(A.Fake<IBrewInstaller>());
        var filePath = Path.Combine(Path.GetTempPath(), $"empty-{Guid.NewGuid():N}.json");
        await File.WriteAllTextAsync(filePath, "{}");

        try
        {
            // Act
            var result = await sut.ReadFileAsync(filePath);

            // Assert
            result.Formulae.Should().BeEmpty();
            result.Casks.Should().BeEmpty();
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [Fact]
    public void Ctor_WhenInstallerIsNull_Throws()
    {
        // Arrange + Act
        var act = () => new BrewImporter(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }
}
