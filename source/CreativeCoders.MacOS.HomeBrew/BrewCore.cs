namespace CreativeCoders.MacOS.HomeBrew;

public class BrewCore : IBrewCore
{
    public bool IsInstalled => false;
}

public interface IBrewCore
{
    bool IsInstalled { get; }
}
