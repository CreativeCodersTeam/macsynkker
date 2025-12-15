using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AwesomeAssertions;
using CreativeCoders.DependencyInjection;
using CreativeCoders.MacOS.UserDefaults;
using CreativeCoders.ProcessUtils.Execution;
using CreativeCoders.ProcessUtils.Execution.Parsers;
using FakeItEasy;
using Xunit;

namespace CreativeCoders.MacOS.UserDefaults.Tests;

public class DefaultUserDefaultsTests
{
    // Helper to create the System Under Test (SUT) with faked dependencies.
    private static (DefaultUserDefaults sut,
        IObjectFactory factory,
        IProcessExecutor<string[]> domainsExecutor,
        IProcessExecutor exportExecutor,
        IProcessExecutor importExecutor) CreateSut(
            string[]? domainsToReturn = null)
    {
        var factory = A.Fake<IObjectFactory>();

        // Fakes for the typed builder and executor used by GetDomainsAsync
        var domainsBuilder = A.Fake<IProcessExecutorBuilder<string[]>>();
        var domainsExecutor = A.Fake<IProcessExecutor<string[]>>();

        A.CallTo(() => factory.GetInstance<IProcessExecutorBuilder<string[]>>())
            .Returns(domainsBuilder);

        // Fluent builder setup (returns itself)
        A.CallTo(() => domainsBuilder.SetFileName(A<string>._))
            .Returns(domainsBuilder);
        A.CallTo(() => domainsBuilder.SetArguments(A<string[]>._))
            .Returns(domainsBuilder);
        A.CallTo(() => domainsBuilder.SetOutputParser<SplitLinesOutputParser>(A<Action<SplitLinesOutputParser>>._))
            .Returns(domainsBuilder);
        A.CallTo(() => domainsBuilder.Build())
            .Returns(domainsExecutor);

        A.CallTo(() => domainsExecutor.ExecuteAsync())
            .Returns(domainsToReturn);

        // Fakes for export and import builders/executors (two separate instances)
        var exportBuilder = A.Fake<IProcessExecutorBuilder>();
        var importBuilder = A.Fake<IProcessExecutorBuilder>();
        var exportExecutor = A.Fake<IProcessExecutor>();
        var importExecutor = A.Fake<IProcessExecutor>();

        // First non-generic builder request -> export, second -> import
        A.CallTo(() => factory.GetInstance<IProcessExecutorBuilder>())
            .ReturnsNextFromSequence(exportBuilder, importBuilder);

        // Fluent for export
        A.CallTo(() => exportBuilder.SetFileName(A<string>._))
            .Returns(exportBuilder);
        A.CallTo(() => exportBuilder.SetArguments(A<string[]>._))
            .Returns(exportBuilder);
        A.CallTo(() => exportBuilder.Build())
            .Returns(exportExecutor);

        // Fluent for import
        A.CallTo(() => importBuilder.SetFileName(A<string>._))
            .Returns(importBuilder);
        A.CallTo(() => importBuilder.SetArguments(A<string[]>._))
            .Returns(importBuilder);
        A.CallTo(() => importBuilder.Build())
            .Returns(importExecutor);

        var sut = new DefaultUserDefaults(factory);
        return (sut, factory, domainsExecutor, exportExecutor, importExecutor);
    }

    [Fact]
    public async Task GetDomainsAsync_ReturnsDefaultsDomains_FromExecutorOutput()
    {
        // Arrange
        var domains = new[] { "com.apple", "org.test" };
        var (sut, _, _, _, _) = CreateSut(domains);

        // Act
        var result = await sut.GetDomainsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Select(x => x.Name).Should().BeEquivalentTo("com.apple", "org.test");
    }

    [Fact]
    public async Task GetDomainsAsync_WhenNull_ReturnsEmptyArray()
    {
        // Arrange
        var (sut, _, _, _, _) = CreateSut(null);

        // Act
        var result = await sut.GetDomainsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task ExportDomainAsync_CallsExecutor()
    {
        // Arrange
        var (sut, _, _, exportExecutor, _) = CreateSut(Array.Empty<string>());

        A.CallTo(exportExecutor)
            .Where(call => call.Method.Name == nameof(IProcessExecutor.ExecuteAsync)
                           && call.Arguments.Count == 1)
            .WithReturnType<Task>()
            .Returns(Task.CompletedTask);

        // Act
        await sut.ExportDomainAsync("com.apple", "/tmp/file.plist");

        // Assert
        A.CallTo(exportExecutor)
            .Where(call => call.Method.Name == nameof(IProcessExecutor.ExecuteAsync)
                           && call.Arguments.Count == 1)
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ImportDomainAsync_CallsExecutor()
    {
        // Arrange
        var (sut, _, _, _, importExecutor) = CreateSut(Array.Empty<string>());

        A.CallTo(importExecutor)
            .Where(call => call.Method.Name == nameof(IProcessExecutor.ExecuteAsync)
                           && call.Arguments.Count == 1)
            .WithReturnType<Task>()
            .Returns(Task.CompletedTask);

        // Act
        await sut.ImportDomainAsync("org.test", "/tmp/in.plist");

        // Assert
        A.CallTo(importExecutor)
            .Where(call => call.Method.Name == nameof(IProcessExecutor.ExecuteAsync)
                           && call.Arguments.Count == 1)
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ExportDomainsAsync_CallsExportForEachDomain()
    {
        // Arrange
        var (sut, _, _, exportExecutor, _) = CreateSut(Array.Empty<string>());

        A.CallTo(exportExecutor)
            .Where(call => call.Method.Name == nameof(IProcessExecutor.ExecuteAsync)
                           && call.Arguments.Count == 1)
            .WithReturnType<Task>()
            .Returns(Task.CompletedTask);

        var dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);

        try
        {
            // Act
            await sut.ExportDomainsAsync(new[] { "com.apple", "org.test" }, dir);

            // Assert
            A.CallTo(exportExecutor)
                .Where(call => call.Method.Name == nameof(IProcessExecutor.ExecuteAsync)
                               && call.Arguments.Count == 1)
                .MustHaveHappenedANumberOfTimesMatching(n => n == 2);
        }
        finally
        {
            Directory.Delete(dir, true);
        }
    }

    [Fact]
    public async Task ImportAllDomainsAsync_CallsImportForEachPlist()
    {
        // Arrange
        var (sut, _, _, _, importExecutor) = CreateSut(Array.Empty<string>());

        A.CallTo(importExecutor)
            .Where(call => call.Method.Name == nameof(IProcessExecutor.ExecuteAsync)
                           && call.Arguments.Count == 1)
            .WithReturnType<Task>()
            .Returns(Task.CompletedTask);

        var dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        var files = new[]
        {
            Path.Combine(dir, "com.apple.plist"),
            Path.Combine(dir, "org.test.plist"),
            Path.Combine(dir, "ignore.txt")
        };
        await File.WriteAllTextAsync(files[0], "<plist/>");
        await File.WriteAllTextAsync(files[1], "<plist/>");
        await File.WriteAllTextAsync(files[2], "ignore");

        try
        {
            // Act
            await sut.ImportAllDomainsAsync(dir);

            // Assert
            A.CallTo(importExecutor)
                .Where(call => call.Method.Name == nameof(IProcessExecutor.ExecuteAsync)
                               && call.Arguments.Count == 1)
                .MustHaveHappenedANumberOfTimesMatching(n => n == 2);
        }
        finally
        {
            Directory.Delete(dir, true);
        }
    }
}
