using AwesomeAssertions;
using CreativeCoders.MacOS.HomeBrew.Cleanup;
using CreativeCoders.MacOS.HomeBrew.Export;
using CreativeCoders.MacOS.HomeBrew.Import;
using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;

namespace CreativeCoders.MacOS.HomeBrew.Tests;

public class HomeBrewServiceCollectionExtensionsTests
{
    [Fact]
    public void AddHomeBrew_RegistersAllHomeBrewServicesAsSingleton()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddHomeBrew();

        // Assert
        services.Should().ContainSingle(x => x.ServiceType == typeof(IBrewInfo)
            && x.ImplementationType == typeof(BrewInfo) && x.Lifetime == ServiceLifetime.Singleton);
        services.Should().ContainSingle(x => x.ServiceType == typeof(IBrewInstalledSoftware)
            && x.ImplementationType == typeof(BrewInstalledSoftware));
        services.Should().ContainSingle(x => x.ServiceType == typeof(IBrewUpgrader)
            && x.ImplementationType == typeof(BrewUpgrader));
        services.Should().ContainSingle(x => x.ServiceType == typeof(IBrewExporter)
            && x.ImplementationType == typeof(BrewExporter));
        services.Should().ContainSingle(x => x.ServiceType == typeof(IBrewInstaller)
            && x.ImplementationType == typeof(BrewInstaller));
        services.Should().ContainSingle(x => x.ServiceType == typeof(IBrewImporter)
            && x.ImplementationType == typeof(BrewImporter));
        services.Should().ContainSingle(x => x.ServiceType == typeof(IBrewUpdater)
            && x.ImplementationType == typeof(BrewUpdater));
        services.Should().ContainSingle(x => x.ServiceType == typeof(IBrewCleanup)
            && x.ImplementationType == typeof(BrewCleanup));
    }

    [Fact]
    public void AddHomeBrew_ReturnsSameServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddHomeBrew();

        // Assert
        result.Should().BeSameAs(services);
    }

    [Fact]
    public void AddHomeBrew_CanResolveAllServicesFromBuiltProvider()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddHomeBrew();
        using var provider = services.BuildServiceProvider();

        // Act + Assert
        provider.GetRequiredService<IBrewInfo>().Should().BeOfType<BrewInfo>();
        provider.GetRequiredService<IBrewInstalledSoftware>().Should().BeOfType<BrewInstalledSoftware>();
        provider.GetRequiredService<IBrewUpgrader>().Should().BeOfType<BrewUpgrader>();
        provider.GetRequiredService<IBrewExporter>().Should().BeOfType<BrewExporter>();
        provider.GetRequiredService<IBrewInstaller>().Should().BeOfType<BrewInstaller>();
        provider.GetRequiredService<IBrewImporter>().Should().BeOfType<BrewImporter>();
        provider.GetRequiredService<IBrewUpdater>().Should().BeOfType<BrewUpdater>();
        provider.GetRequiredService<IBrewCleanup>().Should().BeOfType<BrewCleanup>();
    }

    /// <summary>
    /// Verifies that the registration is idempotent, so calling it from several composition roots
    /// does not produce duplicate service descriptors.
    /// </summary>
    [Fact]
    public void AddHomeBrew_WhenCalledTwice_RegistersEachServiceOnlyOnce()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddHomeBrew();
        services.AddHomeBrew();

        // Assert
        services.Should().ContainSingle(x => x.ServiceType == typeof(IBrewInfo));
        services.Should().ContainSingle(x => x.ServiceType == typeof(IBrewInstalledSoftware));
        services.Should().ContainSingle(x => x.ServiceType == typeof(IBrewUpgrader));
        services.Should().ContainSingle(x => x.ServiceType == typeof(IBrewUpdater));
        services.Should().ContainSingle(x => x.ServiceType == typeof(IBrewExporter));
        services.Should().ContainSingle(x => x.ServiceType == typeof(IBrewInstaller));
        services.Should().ContainSingle(x => x.ServiceType == typeof(IBrewImporter));
        services.Should().ContainSingle(x => x.ServiceType == typeof(IBrewCleanup));
    }

    [Fact]
    public void AddHomeBrew_WhenServiceAlreadyRegistered_DoesNotOverride()
    {
        // Arrange - Uses TryAddSingleton so a pre-existing registration wins
        var services = new ServiceCollection();
        var customInfo = A.Fake<IBrewInfo>();
        services.AddSingleton(customInfo);

        // Act
        services.AddHomeBrew();
        using var provider = services.BuildServiceProvider();

        // Assert
        provider.GetRequiredService<IBrewInfo>().Should().BeSameAs(customInfo);
    }
}
