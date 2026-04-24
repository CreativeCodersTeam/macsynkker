using AwesomeAssertions;
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
